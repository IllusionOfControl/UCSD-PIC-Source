using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;

namespace UCSD_Application
{
    public partial class HIDInterface : Form
    {
        // +--------------------------------------------------------------------------------------+
        // | Global Variables                                                                     |
        // +--------------------------------------------------------------------------------------+
        private String DevPathName;     // Path to USB HID Device, used to detect when device is removed
        private HIDP_CAPS Capabilities; // Used to determine the length of reports HID device can use
        private SafeFileHandle deviceHandle; // Handle used to communicate with device. (Read / write operations)
        private const int HID_READ_REPORT_TIMEOUT = 500; // Read timeout in ms

        public int VID; // Vendor ID of the HID device we are searching for
        public int PID; // Product ID of the HID device we are searching for
        public Boolean isConnected;
        public event EventHandler deviceConnected;
        public event EventHandler deviceDisconnected;


        // +--------------------------------------------------------------------------------------+
        // | HIDInterface                                                                         |
        // | Description: This is the default constructor for our (invisible) form. It will set   |
        // |              some variables to hide the form and init some of our global variables.  |
        // | ARGS: None                                                                           |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public HIDInterface(int _VID, int _PID)
        {
            // Make our form less visible.
            InitializeComponent();
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Text = null; // Hide window from "Applications" tab in task manager

            // Make a Show() call so HIDInterface_Load is called and we can get a window handle to
            // use for RegisterDeviceNotification.
            this.Show();

            // Initialize our global variables
            DevPathName = "";
            isConnected = false;
            VID = _VID;
            PID = _PID;
        }


        // +--------------------------------------------------------------------------------------+
        // | HIDInterface_Load                                                                    |
        // | Description: This function will be called when the form is first loaded, it will     |
        // |              setup and make a call to RegisterDeviceNotification so we will be       |
        // |              notified when a USB HID device has been plugged in / unplugged.         |
        // | Notes:       This is done in this function because we need a window handle           |
        // |              (this.Handle) for us in the RegisterDeviceNotification call.            |
        // | ARGS: sender - unused.                                                               |
        // |       e      - unused.                                                               |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void HIDInterface_Load(object sender, EventArgs e)
        {
            IntPtr devBroadcastDeviceInterfaceBuffer;
            IntPtr deviceNotificationHandle;
            Int32 size = 0;

            // +----------------------------------------------------------------------------------+
            // | Get the system HID GUID so we can setup a notification for changes on this GUID  |
            // +----------------------------------------------------------------------------------+
            System.Guid hidGuid = new System.Guid();
            HidD_GetHidGuid(ref hidGuid);

            // +----------------------------------------------------------------------------------+
            // | Setup RegisterDeviceNotification to so we can be notified of USB device changes  |
            // +----------------------------------------------------------------------------------+
            DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE();

            size = Marshal.SizeOf(devBroadcastDeviceInterface);
            devBroadcastDeviceInterface.dbcc_size = size;
            devBroadcastDeviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
            devBroadcastDeviceInterface.dbcc_reserved = 0;
            devBroadcastDeviceInterface.dbcc_classguid = hidGuid;
            devBroadcastDeviceInterfaceBuffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(devBroadcastDeviceInterface, devBroadcastDeviceInterfaceBuffer, true);

            // +----------------------------------------------------------------------------------+
            // | This command will setup the notification, we will intercept it in the WndProc    |
            // | function of this form.                                                           |
            // +----------------------------------------------------------------------------------+
            deviceNotificationHandle = RegisterDeviceNotification
                                        (this.Handle,
                                        devBroadcastDeviceInterfaceBuffer,
                                        DEVICE_NOTIFY_WINDOW_HANDLE);

            Marshal.FreeHGlobal(devBroadcastDeviceInterfaceBuffer); //Registered the handle, we're done with this interface
        }


        // +--------------------------------------------------------------------------------------+
        // | HIDInterface_Activated                                                               |
        // | Description: This function will be called when the form is first activated, its only |
        // |              job is to make the form invisible. (Since no useful data is on the form |
        // |              we're only abusing it for the window handle)                            |
        // | ARGS: None                                                                           |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void HIDInterface_Activated(object sender, EventArgs e)
        {
            //this.Visible = false;
            this.Size = new System.Drawing.Size(0, 0);
        }


        // +--------------------------------------------------------------------------------------+
        // | WndProc                                                                              |
        // | Description: We're overriding the default from WndProc function here with our own.   |
        // |              This function processes window messages. We're interested in            |
        // |              intercepting the DeviceChange message and passing it on to our own      |
        // |              function. All other messages are passed back to the default WndProc.    |
        // | Notes:       This function will be activated because of RegisterDeviceNotification   |
        // |              that we did in the _Load function.                                      |
        // | ARGS: m - Message passed to the function from windows.                               |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        protected override void WndProc(ref Message m)
        {
            // Did we get a USB HID device change message? (RegisterDeviceNotification)
            if (m.Msg == WM_DEVICECHANGE)
            {
                OnDeviceChange(m); // Yes we did, pass it along to our own processing function.
            }
            base.WndProc(ref m);
        }


        // +--------------------------------------------------------------------------------------+
        // | OnDeviceChange                                                                       |
        // | Description: This function will be responsible for handling a USB HID device change  |
        // |              state. (Plugged in / unplugged) If a device was plugged in, then        |
        // |              we will make a call to try to connect to it. If it was unplugged, we    |
        // |              see if it was our USB HID device and update the global variables.       |
        // | ARGS: m - Message passed to the function from windows.                               |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        internal void OnDeviceChange(Message m)
        {
            try
            {
                DEV_BROADCAST_DEVICEINTERFACE_1 devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE_1();
                DEV_BROADCAST_HDR devBroadcastHeader = new DEV_BROADCAST_HDR();
                Marshal.PtrToStructure(m.LParam, devBroadcastHeader);



                // Check if the notification is coming from a device interface
                if ((devBroadcastHeader.dbch_devicetype == DBT_DEVTYP_DEVICEINTERFACE))
                {
                    // Get the device name string
                    Int32 stringSize = Convert.ToInt32((devBroadcastHeader.dbch_size - 32) / 2);
                    Array.Resize(ref devBroadcastDeviceInterface.dbcc_name, stringSize);
                    Marshal.PtrToStructure(m.LParam, devBroadcastDeviceInterface);
                    String DeviceNameString =
                    new String(devBroadcastDeviceInterface.dbcc_name, 0, stringSize);

                    // +------------------------------------------------------------------------------+
                    // | Check if a new USB HID device has been plugged in                            |
                    // +------------------------------------------------------------------------------+
                    if ((m.WParam.ToInt32() == DBT_DEVICEARRIVAL))
                    {
                        // We have a new device, if we're currently not connected to our device, go out and find it.
                        if (!isConnected)
                        {
                            findDevice();
                        }
                    }
                    // +------------------------------------------------------------------------------+
                    // | Check if a USB HID device has been removed                                   |
                    // +------------------------------------------------------------------------------+
                    else if ((m.WParam.ToInt32() == DBT_DEVICEREMOVECOMPLETE))
                    {
                        // Device has been removed, check if it was our device.
                        if ((String.Compare(DeviceNameString, DevPathName, true) == 0))
                        {
                            // User removed device, update flags, close handle.
                            isConnected = false;
                            deviceHandle.Close();

                            // If user has attached a function to the event handler, invoke that function
                            if (deviceDisconnected != null)
                            {
                                deviceDisconnected(this, EventArgs.Empty);
                            }
                        }

                    }
                }
            }
            catch (ArgumentNullException)
            {
                //Our message was null, exit our function.
            }
        }


        // +--------------------------------------------------------------------------------------+
        // | writeByteArr                                                                         |
        // | Description: This function will attempt to write one report out to the HID device.   |
        // | Notes:       The first byte of the output buffer MUST be 0. This function will       |
        // |              automatically pad the data with this byte.                              |
        // | ARGS: data - Byte array to be transferred to the device.                             |
        // | RETS: true  - write has succeeded                                                    |
        // |       false - write has failed                                                       |
        // +--------------------------------------------------------------------------------------+
        public Boolean writeByteArr(Byte[] data)
        {
            Boolean success = false;
            Int32 numberOfBytesWritten = 0;
            Byte[] outputReportBuffer = null;

            // Make sure our device is connected, else bail out early
            if (!isConnected)
                return false;

            // We will send out a report with the the max capable size
            Array.Resize(ref outputReportBuffer, Capabilities.OutputReportByteLength);

            // +--------------------------------------------------------------+
            // | Note: The first byte of an HID report MUST be 0. Else the    |
            // |       send will fail. I pad the report here.                 |
            // +--------------------------------------------------------------+
            outputReportBuffer[0] = 0;

            // Copy the rest of the users data to the report
            //ToDo: Check the users data length to make sure we won't overflow the buffer
            System.Buffer.BlockCopy(data, 0, outputReportBuffer, 1, data.Length);

            // Try to send the report out to the device
            success = WriteFile
                (deviceHandle,
                outputReportBuffer,
                outputReportBuffer.Length,
                ref numberOfBytesWritten,
                IntPtr.Zero);

            return success;
        }


        // +--------------------------------------------------------------------------------------+
        // | readByteArr                                                                          |
        // | Description: This function will attempt to read one report from the HID device. It   |
        // |              will try for a fixed amount of time. (Currently 3 seconds) then return  |
        // |              the success or failure of the read along with the data.                 |
        // | ARGS: data - Byte array to be read from the device.                                  |
        // | RETS: true  - read has succeeded                                                     |
        // |       false - read has failed                                                        |
        // +--------------------------------------------------------------------------------------+
        public Boolean readByteArr(ref Byte[] data)
        {
            Boolean success = false;
            IntPtr eventObject = IntPtr.Zero;
            IntPtr unManagedBuffer = IntPtr.Zero;
            IntPtr unManagedOverlapped = IntPtr.Zero;
            Int32 numberOfBytesRead = 0;
            Int32 result = 0;
            Byte[] inputReportBuffer = null;
            NativeOverlapped HidOverlapped = new NativeOverlapped();


            // Make sure our device is connected, else bail out early
            if (!isConnected)
                return false;

            // Resize array to fix max report length
            Array.Resize(ref inputReportBuffer, Capabilities.InputReportByteLength);

            eventObject = CreateEvent
                (IntPtr.Zero,
                false,
                false,
                String.Empty);

            HidOverlapped.OffsetLow = 0;
            HidOverlapped.OffsetHigh = 0;
            HidOverlapped.EventHandle = eventObject;
            unManagedBuffer = Marshal.AllocHGlobal(inputReportBuffer.Length);
            unManagedOverlapped = Marshal.AllocHGlobal(Marshal.SizeOf(HidOverlapped));
            Marshal.StructureToPtr(HidOverlapped, unManagedOverlapped, false);

            success = ReadFile
                (deviceHandle,
                unManagedBuffer,
                inputReportBuffer.Length,
                ref numberOfBytesRead,
                unManagedOverlapped);

            // If we have a success, we already read the report, else we're going to wait for it
            if (!success)
            {
                // Wait up to 'HID_READ_REPORT_TIMEOUT' milliseconds for the report
                result = WaitForSingleObject(eventObject, HID_READ_REPORT_TIMEOUT);

                switch (result)
                {
                    case WAIT_OBJECT_0:
                        success = true;
                        GetOverlappedResult(deviceHandle, unManagedOverlapped,
                                            ref numberOfBytesRead, false);
                        break;
                    case WAIT_TIMEOUT:
                        CancelIo(deviceHandle);
                        break;
                    default:
                        CancelIo(deviceHandle);
                        break;
                }
            }

            if (success)
            {
                // A report was received.
                // Copy the received data to inputReportBuffer
                Marshal.Copy(unManagedBuffer, inputReportBuffer, 0, numberOfBytesRead);
                data = inputReportBuffer;
            }

            Marshal.FreeHGlobal(unManagedOverlapped);
            Marshal.FreeHGlobal(unManagedBuffer);

            return success;
        }


        // +--------------------------------------------------------------------------------------+
        // | findDevice                                                                           |
        // | Description: This function will try to find and connect to our HID device based on   |
        // |              the global VID and PID.                                                 |
        // | ARGS: None                                                                           |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public Boolean findDevice()
        {
            // Check if we're already connected. If we are... we're done!
            if (isConnected)
                return true;

            Boolean foundMatch = false; // Set if we find a device matching our VID and PID
            Boolean success = false;    // Success of an individual call to API function
            IntPtr detailDataBuffer = IntPtr.Zero;
            Int32 memberIndex = 0;
            Int32 bufferSize = 0;
            System.Guid hidGuid = new System.Guid();
            SP_DEVICE_INTERFACE_DATA MyDeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            HIDD_ATTRIBUTES Device_ID_Attributes = new HIDD_ATTRIBUTES();   // Structure Holds: VID, PID, and VersionNumber

            // Get the  systems HID GUID
            HidD_GetHidGuid(ref hidGuid);

            // Here we populate a list of plugged-in devices matching our class GUID (DIGCF_PRESENT specifies that the list
            // should only contain devices which are plugged in)
            deviceInfoSet = SetupDiGetClassDevs(ref hidGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);          

            // +---------------------+
            // | Initialize our data |
            // +---------------------+
            MyDeviceInterfaceData.cbSize = Marshal.SizeOf(MyDeviceInterfaceData);

            for (; ; )
            {
                //  +---------------------------------------------------------------------------------+
                //  | Enumerate The "Device Interfaces" For Each Device In Our Device Information Set |
                //  +---------------------------------------------------------------------------------+
                success = SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref hidGuid, memberIndex, ref MyDeviceInterfaceData);

                if (!success)
                    break;

                //  +---------------------------+
                //  | If We Found A Good Device |
                //  +---------------------------+                
                if (success)
                {
                    // First call is just to get the required buffer size for the real request
                    success = SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref MyDeviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);
                    // Allocate some memory for the buffer
                    detailDataBuffer = Marshal.AllocHGlobal(bufferSize);
                    Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

                    // Second call gets the detailed data buffer
                    success = SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref MyDeviceInterfaceData, detailDataBuffer, bufferSize, ref bufferSize, IntPtr.Zero);

                    if (success)
                    {
                        //  +---------------------------------+
                        //  | Can now try to open this Device |
                        //  +---------------------------------+                    
                        String devicePathName = "";
                        // Skip over cbsize (4 bytes) to get the address of the devicePathName.
                        IntPtr pDevicePathName = new IntPtr(detailDataBuffer.ToInt32() + 4);
                        devicePathName = Marshal.PtrToStringAuto(pDevicePathName);
                        Marshal.FreeHGlobal(detailDataBuffer);

                        deviceHandle = CreateFile
                            (devicePathName,
                            (GENERIC_WRITE | GENERIC_READ),
                            FILE_SHARE_READ | FILE_SHARE_WRITE,
                            IntPtr.Zero,
                            OPEN_EXISTING,
                            0,
                            0);

                        // Check if the open succeeded
                        if (!deviceHandle.IsInvalid)
                        {
                            //The open worked. Lets get the attributes so we can read the PID and VID
                            Device_ID_Attributes.Size = Marshal.SizeOf(Device_ID_Attributes);
                            success = HidD_GetAttributes(deviceHandle, ref Device_ID_Attributes);

                            //Debug.WriteLine("VID: " + Convert.ToString(Device_ID_Attributes.VendorID, 16) + ", PID: " + Convert.ToString(Device_ID_Attributes.ProductID, 16) + ", Ver: " + Device_ID_Attributes.VersionNumber + ", Size: " + Device_ID_Attributes.Size);

                            // +------------------------------------+
                            // | Check if the device we opened      |
                            // | matches our PID and VID.           |
                            // +------------------------------------+
                            if ((Device_ID_Attributes.VendorID == VID) && (Device_ID_Attributes.ProductID == PID))
                            {
                                // We found a match! Get device capabilities.
                                IntPtr preparsedData = new IntPtr();
                                success = HidD_GetPreparsedData(deviceHandle, ref preparsedData);
                                Int32 result = 0;
                                result = HidP_GetCaps(preparsedData, ref Capabilities);

                                foundMatch = true;
                                isConnected = true;
                                DevPathName = devicePathName;

                                // If user has attached a function to this event handler, call it.
                                if (deviceConnected != null)
                                {
                                    deviceConnected(this, EventArgs.Empty);
                                }

                                break;
                            }

                        }
                        deviceHandle.Close();
                        memberIndex++;

                    }

                } // End if

            } // End for

            SetupDiDestroyDeviceInfoList(deviceInfoSet);

            return foundMatch;
        }



        // +--------------------------------------------------------------------------------------+
        // | Windows APIs included below                                                          |
        // +--------------------------------------------------------------------------------------+
        #region Native Win32 API

        [DllImport("hid.dll", SetLastError = true)]
        public static extern void HidD_GetHidGuid(ref System.Guid HidGuid);

        internal const Int32 DBT_DEVTYP_DEVICEINTERFACE = 5;
        internal const Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        internal const Int32 WM_DEVICECHANGE = 0X219;
        internal const Int32 DBT_DEVICEARRIVAL = 0X8000;
        internal const Int32 DBT_DEVICEREMOVECOMPLETE = 0X8004;
        internal const Int32 FILE_ATTRIBUTE_NORMAL = 0X80;
        internal const Int32 FILE_SHARE_READ = 1;
        internal const Int32 FILE_SHARE_WRITE = 2;
        internal const Int32 OPEN_EXISTING = 3;
        internal const Int32 DIGCF_PRESENT = 2;
        internal const Int32 DIGCF_DEVICEINTERFACE = 0X10;
        internal const Int32 FILE_FLAG_OVERLAPPED = 0X40000000;
        internal const UInt32 GENERIC_READ = 0X80000000;
        internal const UInt32 GENERIC_WRITE = 0X40000000;
        internal const Int32 WAIT_OBJECT_0 = 0;
        internal const Int32 WAIT_TIMEOUT = 0X102;

        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_DEVICEINTERFACE
        {
            internal Int32 dbcc_size;
            internal Int32 dbcc_devicetype;
            internal Int32 dbcc_reserved;
            internal Guid dbcc_classguid;
            internal Int16 dbcc_name;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification
            (IntPtr hRecipient,
            IntPtr NotificationFilter,
            Int32 Flags);

        [StructLayout(LayoutKind.Sequential)]
        internal class DEV_BROADCAST_HDR
        {
            internal Int32 dbch_size;
            internal Int32 dbch_devicetype;
            internal Int32 dbch_reserved;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class DEV_BROADCAST_DEVICEINTERFACE_1
        {
            internal Int32 dbcc_size;
            internal Int32 dbcc_devicetype;
            internal Int32 dbcc_reserved;
            [MarshalAs(UnmanagedType.ByValArray,
           ArraySubType = UnmanagedType.U1,
           SizeConst = 16)]
            internal Byte[] dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray,
            SizeConst = 255)]
            internal Char[] dbcc_name;

        }

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SetupDiGetClassDevs
            (ref System.Guid ClassGuid,
             IntPtr Enumerator,
             IntPtr hwndParent,
             Int32 Flags);


        IntPtr deviceInfoSet;

        internal struct SP_DEVICE_INTERFACE_DATA
        {
            internal Int32 cbSize;
            internal Guid InterfaceClassGuid;
            internal Int32 Flags;
            internal IntPtr Reserved;
        }

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern Boolean SetupDiEnumDeviceInterfaces
            (IntPtr DeviceInfoSet,
             IntPtr DeviceInfoData,
             ref System.Guid InterfaceClassGuid,
             Int32 MemberIndex,
             ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);



        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern Boolean SetupDiGetDeviceInterfaceDetail
            (IntPtr DeviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
            IntPtr DeviceInterfaceDetailData,
            Int32 DeviceInterfaceDetailDataSize,
            ref Int32 RequiredSize,
            IntPtr DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern Int32 SetupDiDestroyDeviceInfoList
            (IntPtr DeviceInfoSet);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern SafeFileHandle CreateFile
            (String lpFileName,
            UInt32 dwDesiredAccess,
            Int32 dwShareMode,
            IntPtr lpSecurityAttributes,
            Int32 dwCreationDisposition,
            Int32 dwFlagsAndAttributes,
            Int32 hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Boolean WriteFile(
            SafeFileHandle hFile,
            Byte[] lpBuffer,
            Int32 nNumberOfBytesToWrite,
            ref Int32 lpNumberOfBytesWritten,
            IntPtr lpOverlapped
            );

        [DllImport("hid.dll", SetLastError = true)]
        static extern Boolean HidD_GetAttributes(SafeFileHandle HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

        [StructLayout(LayoutKind.Sequential)]

        public struct HIDD_ATTRIBUTES
        {

            public Int32 Size;
            public Int16 VendorID;
            public Int16 ProductID;
            public Int16 VersionNumber;

        }

        [DllImport("hid.dll", SetLastError = true)]
        internal static extern Boolean HidD_GetPreparsedData
        (SafeFileHandle HidDeviceObject,
        ref IntPtr PreparsedData);

        internal struct HIDP_CAPS
        {
            internal Int16 Usage;
            internal Int16 UsagePage;
            internal Int16 InputReportByteLength;
            internal Int16 OutputReportByteLength;
            internal Int16 FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            internal Int16[] Reserved;
            internal Int16 NumberLinkCollectionNodes;
            internal Int16 NumberInputButtonCaps;
            internal Int16 NumberInputValueCaps;
            internal Int16 NumberInputDataIndices;
            internal Int16 NumberOutputButtonCaps;
            internal Int16 NumberOutputValueCaps;
            internal Int16 NumberOutputDataIndices;
            internal Int16 NumberFeatureButtonCaps;
            internal Int16 NumberFeatureValueCaps;
            internal Int16 NumberFeatureDataIndices;
        }
        [DllImport("hid.dll", SetLastError = true)]
        internal static extern Int32 HidP_GetCaps
        (IntPtr PreparsedData,
        ref HIDP_CAPS Capabilities);


        // Read report //
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Int32 CancelIo
            (SafeFileHandle hFile);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr CreateEvent
            (IntPtr SecurityAttributes,
            Boolean bManualReset,
            Boolean bInitialState,
            String lpName);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Boolean GetOverlappedResult
            (SafeFileHandle hFile,
            IntPtr lpOverlapped,
            ref Int32 lpNumberOfBytesTransferred,
            Boolean bWait);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Boolean ReadFile
            (SafeFileHandle hFile,
            IntPtr lpBuffer,
            Int32 nNumberOfBytesToRead,
            ref Int32 lpNumberOfBytesRead,
            IntPtr lpOverlapped);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Int32 WaitForSingleObject
            (IntPtr hHandle,
            Int32 dwMilliseconds);

        #endregion

    }
}
