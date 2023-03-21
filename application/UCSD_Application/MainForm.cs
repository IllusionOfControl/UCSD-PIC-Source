using System;
using System.Windows.Forms;
using System.Diagnostics;             // Performance counters
using System.Runtime.InteropServices; // DLL Imports, Marshalls
using System.Reflection;              // Getting file info for version / build date
using System.IO;                      // Reading file info for version / build date
using Microsoft.Win32;                // Detecting system sleep / resume

namespace UCSD_Application
{
    public partial class MainForm : Form
    {
        // +------------------------------------+
        // | Constants                          |
        // +------------------------------------+
        const int VID = 0x04D8; // Microchips USB Vendor ID
        const int PID = 0x4147; // My arbitrary product ID for this LCD display (ASCII 'AG' represented in hex)

        // +------------------------------------+
        // | Global Variables                   |
        // +------------------------------------+
        HIDInterface lcdDisplay;            // Instance of the USB HID LCD we will be playing with
        Boolean idleActive  = false;        // Flag used to toggle backlight after computer has been idle.
        Boolean isMonitorOn = true;
        IntPtr  hMonitorOn  = IntPtr.Zero;
        String origWindowText;

        // +--------------------------------------------------------------------------------------+
        // | MainForm                                                                             |
        // | Description: This is the default constructor for our main form. It will init a new   |
        // |              LCD display, performance counters, and add version info.                |
        // | ARGS: None                                                                           |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public MainForm()
        {
            //Get operating system version
            Version osVersion = Environment.OSVersion.Version;

            InitializeComponent();

            // +--------------------------------------------------------------+
            // | Register a new event handler to detect system sleep / resume |
            // +--------------------------------------------------------------+
            SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);

            // +--------------------------------------------------------------+
            // | Check if user is running a windows version less than Vista   |
            // | (2000, XP, ...) We will need to disable functionality.       |
            // +--------------------------------------------------------------+
            if (osVersion.Major < 6)
            {
                WinVerWaringLabel.Text = "Requires Vista+ to function";
                MonitorStandbyEnable.Enabled = false;
                Properties.Settings.Default.monStandyDetectIndex = 1;
            }
            else
            {
                WinVerWaringLabel.Text = "";
                registerForPowerSettingNotification();
            }

            // +--------------------------------------------------------------+
            // | Init our display with VID and PID,                           |
            // | Add event handlers to call functions when we see the device  |
            // | connect or disconnect,                                       |
            // | Try to find the device if it was connected at startup        |
            // +--------------------------------------------------------------+
            lcdDisplay = new HIDInterface(VID, PID);
            lcdDisplay.deviceDisconnected += new EventHandler(deviceDisconnectEvent);
            lcdDisplay.deviceConnected += new EventHandler(deviceConnectEvent);
            lcdDisplay.findDevice();

            // Add a link to my website and add the current version info to the form.
            WebsiteLinkLabel.Links.Add(0, 50, "www.AGehringer.com");
            versionLabel.Text = getVersionAndBuildDate();

            // Restore user selected timeout and refresh values from settings
            BacklightTimeoutBox.Value = Properties.Settings.Default.idleTimeout;
            RefreshRateUpDown.Value = Properties.Settings.Default.refreshRate;
            MonitorStandbyEnable.SelectedIndex = Properties.Settings.Default.monStandyDetectIndex;

            // +--------------------------------------------------------------+
            // | Check if settings exist for our line options tabs. If they   |
            // | do not exist, create them and set a default item to use. If  |
            // | they do exist, use them to load user choices.                |
            // +--------------------------------------------------------------+
            // Check for settings line 1
            if (Properties.Settings.Default.line1Checked == null)
            {
                Properties.Settings.Default.line1Checked = new System.Collections.ArrayList();
                lineOptionControl1.setCheckedItem(0);
            }
            else
            {
                lineOptionControl1.loadItems(Properties.Settings.Default.line1Checked);
            }

            // Check for settings line 2
            if (Properties.Settings.Default.line2Checked == null)
            {
                Properties.Settings.Default.line2Checked = new System.Collections.ArrayList();
                lineOptionControl2.setCheckedItem(1);
            }
            else
            {
                lineOptionControl2.loadItems(Properties.Settings.Default.line2Checked);
            }

            // Check for settings line 3
            if (Properties.Settings.Default.line3Checked == null)
            {
                Properties.Settings.Default.line3Checked = new System.Collections.ArrayList();
                lineOptionControl3.setCheckedItem(3);
            }
            else
            {
                lineOptionControl3.loadItems(Properties.Settings.Default.line3Checked);
            }

            // Check for settings line 4
            if (Properties.Settings.Default.line4Checked == null)
            {
                Properties.Settings.Default.line4Checked = new System.Collections.ArrayList();
                lineOptionControl4.setCheckedItem(5);
            }
            else
            {
                lineOptionControl4.loadItems(Properties.Settings.Default.line4Checked);
            }

            // +--------------------------------------------------------------+
            // | Add a tooltip explaining the idle timeout box. In .NET a     |
            // | NumericUpDown control consists of three controls... loop     |
            // | through each adding the tooltip.                             |
            // +--------------------------------------------------------------+
            foreach (Control c in BacklightTimeoutBox.Controls)
                BacklightTimeoutToolTip.SetToolTip(c, "Will turn off backlight after # idle time...\n0 - No timeout \n≥1 - Idle timeout in # minutes");

        }


        // +--------------------------------------------------------------------------------------+
        // | MainForm_Load                                                                        |
        // | Description: This function is called after the form has been initialized. Its job is |
        // |              to read and act on command line arguments.                              |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void MainForm_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();

            origWindowText = this.Text;

            // +--------------------------------------------------------------+
            // | Run through the arguments passed to the program. If one of   |
            // | them was "minimize" then start the form minimized.           |
            // +--------------------------------------------------------------+
            foreach (string arg in args)
            {
                if ((arg.ToString() == "minimize") || (arg.ToString() == "min"))
                {
                    HideWindowButton_Click(null, null);
                }
            }
        }


        // +--------------------------------------------------------------------------------------+
        // | MainForm_Resize                                                                      |
        // | Description: This function is resizes the form. I want to watch if they minimize it. |
        // |              If they do, I will minimize to the tray and pop up a balloon.           |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                HideWindowButton_Click(null, null);
                UCSDTrayIcon.ShowBalloonTip(500);
            }
        }


        // +--------------------------------------------------------------------------------------+
        // | MainForm_FormClosing                                                                 |
        // | Description: This function is called right before the form closes. I will clear the  |
        // |              display, write a short message, and save users settings.                |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lcdDisplay.clearScreen();

            string str1 = "Support program closed.";
            string str2 = "Please relaunch program.";
            lcdDisplay.writeString(str1, 1);
            lcdDisplay.writeString(str2, 2);

            // Save user settings
            Properties.Settings.Default.idleTimeout = BacklightTimeoutBox.Value;
            Properties.Settings.Default.refreshRate = RefreshRateUpDown.Value;
            Properties.Settings.Default.monStandyDetectIndex = MonitorStandbyEnable.SelectedIndex;
            lineOptionControl1.saveItems(Properties.Settings.Default.line1Checked);
            lineOptionControl2.saveItems(Properties.Settings.Default.line2Checked);
            lineOptionControl3.saveItems(Properties.Settings.Default.line3Checked);
            lineOptionControl4.saveItems(Properties.Settings.Default.line4Checked);
            Properties.Settings.Default.Save();

            // If we previously called for notification handle, unregister it.
            if (hMonitorOn != IntPtr.Zero)
                PowerNotification.UnregisterPowerSettingNotification(hMonitorOn);
        }


        // +--------------------------------------------------------------------------------------+
        // | WndProc                                                                              |
        // | Description: This overridden function is responsible for handling messages sent to   |
        // |              the window. I will be looking for monitor on/off messages.              |
        // | ARGS: m - message to be processed.                                                   |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        protected override void WndProc(ref Message m)
        {
            if ((m.Msg == PowerNotification.WM_POWERBROADCAST) &&
                ((int)m.WParam == PowerNotification.PBT_POWERSETTINGCHANGE))
            {
                PowerNotification.POWERBROADCAST_SETTING ps = (PowerNotification.POWERBROADCAST_SETTING)Marshal.PtrToStructure(
                                                                m.LParam, typeof(PowerNotification.POWERBROADCAST_SETTING));
                IntPtr pData = (IntPtr)((int)m.LParam + Marshal.SizeOf(ps));
                Int32  iData = (Int32)Marshal.PtrToStructure(pData, typeof(Int32));

                if (ps.PowerSetting == PowerNotification.GUID_MONITOR_POWER_ON)
                {
                    switch (iData)
                    {
                        case 0:
                            // Monitor OFF
                            if (MonitorStandbyEnable.SelectedIndex == 0)
                            {
                                lcdDisplay.turnOffBacklight();
                                isMonitorOn = false;
                            }
                            break;
                        case 1:
                            // Monitor ON
                            if (MonitorStandbyEnable.SelectedIndex == 0)
                            {
                                lcdDisplay.turnOnBacklight();
                                isMonitorOn = true;
                            }
                            break;
                        default:
                            // Unknown Event
                            break;
                    }
                }
            }

            base.WndProc(ref m);
        }


        // +--------------------------------------------------------------------------------------+
        // | SystemEvents_PowerModeChanged                                                        |
        // | Description: This function is called by the system when the power mode changes. I    |
        // |              use it to detect when the system goes to sleep / hibernate and resumes. |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            // +--------------------------------------------------------------+
            // | Check if system is going to sleep. If I try to use ReadFile  |
            // | during this time, the call will hang and so will my program. |
            // | So I will suspend the refresh timer until system resumes.    |
            // +--------------------------------------------------------------+
            if (e.Mode == PowerModes.Suspend)
            {
                refreshLcdDisplayTimer.Enabled = false;
            }
            else if (e.Mode == PowerModes.Resume)
            {
                refreshLcdDisplayTimer.Enabled = true;
                isMonitorOn = true;
                lcdDisplay.clearScreen();
            }

        }


        // +--------------------------------------------------------------------------------------+
        // | registerForPowerSettingNotification                                                  |
        // | Description: This function will try to register for this window handle to be         |
        // |              notified when the monitor changes power states. (Standby / on).         |
        // | Notes:       When changing the ShowInTaskbar value the window handle will change!    |
        // |              (This.Handle) So we will need to re-register for the notifications.     |
        // | ARGS: None                                                                           |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        void registerForPowerSettingNotification()
        {
            try
            {
                // RegisterPowerSettingNotification requires at least Vista to work.
                hMonitorOn = PowerNotification.RegisterPowerSettingNotification(this.Handle,
                                                ref PowerNotification.GUID_MONITOR_POWER_ON,
                                             PowerNotification.DEVICE_NOTIFY_WINDOW_HANDLE);
            }
            catch (System.EntryPointNotFoundException)
            {
                // Odds are user is using an older version of windows, just set pointer to zero
                hMonitorOn = IntPtr.Zero;
            }
        }


        // +--------------------------------------------------------------------------------------+
        // | getVersionAndBuildDate                                                               |
        // | Description: This function will assemble our version and date string that is placed  |
        // |              at the bottom of the program. It gets version info it needs by reading  |
        // |              sections out of AssemblyInfo.vb. It gets the build date from the        |
        // |              TimeDateStamp field of the IMAGE_FILE_HEADER section the Portable       |
        // |              Executable (PE) file.                                                   |
        // | Notes      : There are two other ways I can get the build date:                      |
        // |               1) Exe file modification date... not reliable.                         |
        // |               2) Modify the AssemblyVersion var in AssemblyInfo.vb to follow the     |
        // |                  following scheme: AssemblyVersion("MAJOR.MINOR.*")                  |
        // |                    Example: [assembly: AssemblyVersion("1.0.*")]                     |
        // |                  The asterisk allows VB to place build date in build/version info.   |
        // |                  I didn't want to mod my AssemblyVersion for this purpose.           |
        // |                                                                                      |
        // | ARGS:   None                                                                         |
        // | RETS:   VersionText - The string representation of the version and build date        |
        // |         date.                                                                        |
        // | - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -|
        // | Idea From:                                                                           |
        // | http://www.codinghorror.com/blog/2005/04/determining-build-date-the-hard-way.html    |
        // +--------------------------------------------------------------------------------------+
        public string getVersionAndBuildDate()
        {
            const int e_lfanew = 0x3C;           // Offset to 'e_lfanew' in the PE File. The next four bytes contain the offset to the PE Header
            const int LinkerTimestampOffset = 8; // Offset to TimeDateStamp field in the IMAGE_FILE_HEADER (PE Header)

            string VersionText = "";    // Our completed version label will be stuffed into this var
            Stream peFileStream = null; // Will be used as file handle to read PE file
            Byte[] fileBuff = new byte[2048];  // Buffer to read the first 2k of the file
            DateTime linkTime = new DateTime(1970, 1, 1, 0, 0, 0); // Base time to be added to the TimeDateStamp

            string pathToExe = System.Reflection.Assembly.GetCallingAssembly().Location;
            Assembly aAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Version AssemblyVersion = aAssembly.GetName().Version;

            // +--------------------------------------------------------------+
            // | Read the first 2k bytes of 'this' exe.                       |
            // +--------------------------------------------------------------+
            try
            {
                peFileStream = new FileStream(pathToExe, FileMode.Open, FileAccess.Read);
                peFileStream.Read(fileBuff, 0, 2048);
            }
            finally
            {
                if (peFileStream != null)
                    peFileStream.Close();
            }

            // +--------------------------------------------------------------+
            // | Read the location of the IMAGE_FILE_HEADER section. Then     |
            // | read the TimeDateStamp field.                                |
            // +--------------------------------------------------------------+
            int PEHeaderOffset = BitConverter.ToInt32(fileBuff, e_lfanew);
            int LinkerTimeDateStamp = BitConverter.ToInt32(fileBuff, PEHeaderOffset + LinkerTimestampOffset);

            // +--------------------------------------------------------------+
            // | Convert the LinkerTimeDateStamp into the current time.       |
            // +--------------------------------------------------------------+
            linkTime = linkTime.AddSeconds(LinkerTimeDateStamp);
            linkTime = linkTime.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(linkTime).Hours);

            //Assemble our final string
            VersionText = string.Format("Version: {0} - Build Date: {1}", AssemblyVersion.Major.ToString() + "." + AssemblyVersion.Minor.ToString(), linkTime.ToString("MMM dd yyyy"));

            return (VersionText);
        }


        // +--------------------------------------------------------------------------------------+
        // | deviceDisconnectEvent                                                                |
        // | Description: This function is called by the HIDInterface when the device we were     |
        // |              connected to disconnect.                                                |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public void deviceDisconnectEvent(object sender, EventArgs e)
        {
            connectionStatusLabel.Text = "disconnected";
        }


        // +--------------------------------------------------------------------------------------+
        // | deviceConnectEvent                                                                   |
        // | Description: This function is called by the HIDInterface when the device we are      |
        // |              looking for is found (connected).                                       |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public void deviceConnectEvent(object sender, EventArgs e)
        {
            connectionStatusLabel.Text = "connected";
            lcdDisplay.clearScreen();

            // Quick demo of updating one of the custom characters (Alternating dots pattern)
            byte[] charData = { 0x0A, 0x15, 0x0A, 0x15, 0x0A, 0x15, 0x0A, 0x15};
            lcdDisplay.writeCustomChar(6, charData);
        }


        // +--------------------------------------------------------------------------------------+
        // | refreshLcdDisplayTimer_Tick                                                          |
        // | Description: This function is called by the timer event. Its job is to retrieve and  |
        // |              update the display with new info.                                       |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void refreshLcdDisplayTimer_Tick(object sender, EventArgs e)
        {
            string str1;
            string str2;
            string str3;
            string str4;
            string textBoxStr;

            // +--------------------------------------------------------------+
            // | String processing. Get all of the data to be displayed ready |
            // +--------------------------------------------------------------+
            str1 = lineOptionControl1.getString();
            str2 = lineOptionControl2.getString();
            str3 = lineOptionControl3.getString();
            str4 = lineOptionControl4.getString();

            
            // +--------------------------------------------------------------+
            // | I've got all of my strings, lets write them to the LCD.      |
            // +--------------------------------------------------------------+
            lcdDisplay.writeString(str1, 1);
            lcdDisplay.writeString(str2, 2);
            lcdDisplay.writeString(str3, 3);
            lcdDisplay.writeString(str4, 4);

            // +--------------------------------------------------------------+
            // | Replace my custom characters with windows printable chars to |
            // | be displayed in the GUI.                                     |
            // +--------------------------------------------------------------+
            textBoxStr = str1 + "\r\n" + str2 + "\r\n" + str3 + "\r\n" + str4;
            textBoxStr = textBoxStr.Replace((char)0x10, '_'); // Empty block
            textBoxStr = textBoxStr.Replace((char)0x11, 'x'); // Partial block
            textBoxStr = textBoxStr.Replace((char)0x12, 'x');
            textBoxStr = textBoxStr.Replace((char)0x13, 'x');
            textBoxStr = textBoxStr.Replace((char)0x14, 'x');
            textBoxStr = textBoxStr.Replace((char)0x15, 'X'); // Full block
            textBoxStr = textBoxStr.Replace((char)0x16, '?'); // Custom Char 1
            textBoxStr = textBoxStr.Replace((char)0x17, '?'); // Custom Char 2
            DisplayTextLabel.Text = textBoxStr;

            // +--------------------------------------------------------------+
            // | Determine how long we have been idling and turn off back     |
            // | light if necessary.                                          |
            // +--------------------------------------------------------------+
            uint idleTime = IdleTime.GetIdleTime();

            if ((BacklightTimeoutBox.Value == 0) && (MonitorStandbyEnable.SelectedIndex == 1))
            {
                // Do nothing, user does not want backlight timeout / detection.
            }
            else if (((BacklightTimeoutBox.Value > 0) && (idleTime >= BacklightTimeoutBox.Value * 1000 * 60)) ||
                     (isMonitorOn == false))
            {
                // We have been idle for longer than X minutes or we have detected
                // the monitor going into a low power mode. Turn off backlight.
                lcdDisplay.turnOffBacklight();
                idleActive = true;
            }
            else if (idleActive == true)
            {
                // We have resumed from being idle/off, turn on backlight
                lcdDisplay.turnOnBacklight();
                idleActive = false;
            }

            // +--------------------------------------------------------------+
            // | Read current backlight status and update GUI.                |
            // +--------------------------------------------------------------+
            if (lcdDisplay.isBacklightOn())
                backlightStatusPlaceholder.Text = "On";
            else
                backlightStatusPlaceholder.Text = "Off";
        }


        // +--------------------------------------------------------------------------------------+
        // | UCSDTrayIcon_MouseDoubleClick                                                        |
        // | Description: This function is called when user double clicks the tray icon. We want  |
        // |              to restore the window.                                                  |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void UCSDTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            openMenuItem_Click(null, null);
        }


        // +--------------------------------------------------------------------------------------+
        // | openMenuItem_Click                                                                   |
        // | Description: This function is called when user clicks the open item from the context |
        // |              menu. We want to restore the GUI window.                                |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void openMenuItem_Click(object sender, EventArgs e)
        {
            Show();                               // Make the form visible again
            WindowState = FormWindowState.Normal; // Make sure we are not minimized
            ShowInTaskbar = true;                 // Enable task-bar display
            this.Activate();                      // Set us as the topmost, focused window
            this.Text = origWindowText;           // Restore window title.
            
            // Note: When changing ShowInTaskbar value, the current window handle is destroyed!
            //       (this.Handle) We will need to re-register for monitor events.
            registerForPowerSettingNotification();
        }


        // +--------------------------------------------------------------------------------------+
        // | exitMenuItem_Click                                                                   |
        // | Description: This function is called when user clicks the exit item from the context |
        // |              menu. We want to close the program gracefully.                          |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        // +--------------------------------------------------------------------------------------+
        // | HideWindowButton_Click                                                               |
        // | Description: This function is called when user clicks the hide window button in the  |
        // |              GUI. We want to hide and only display the tray icon.                    |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void HideWindowButton_Click(object sender, EventArgs e)
        {
            Hide();
            ShowInTaskbar = false; // Disable task-bar display, only show tray icon.
            this.Text = null;      // Hide from "Applications" tab in Task Manager

            // Note: When changing ShowInTaskbar value, the current window handle is destroyed!
            //       (this.Handle) We will need to re-register for monitor events.
            registerForPowerSettingNotification();
        }


        // +--------------------------------------------------------------------------------------+
        // | ToggleBacklightButton_Click                                                          |
        // | Description: This function is called when user clicks the toggle backlight button in |
        // |              the GUI. Guess what it does?!... toggles the LCD backlight!             |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void ToggleBacklightButton_Click(object sender, EventArgs e)
        {
            lcdDisplay.toggleBacklight();

            // +--------------------------------------------------------------+
            // | Read current backlight status and update GUI.                |
            // +--------------------------------------------------------------+
            if (lcdDisplay.isBacklightOn())
                backlightStatusPlaceholder.Text = "On";
            else
                backlightStatusPlaceholder.Text = "Off";
        }


        // +--------------------------------------------------------------------------------------+
        // | RefreshRateUpDown_ValueChanged                                                       |
        // | Description: This function is called when user changes the refresh rate. It will     |
        // |              update the refresh LCD timer interval.                                  |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void RefreshRateUpDown_ValueChanged(object sender, EventArgs e)
        {
            refreshLcdDisplayTimer.Interval = (int)RefreshRateUpDown.Value;
        }


        // +--------------------------------------------------------------------------------------+
        // | BacklightTimeoutBox_Click                                                            |
        // | Description: This function is called when user clicks the Backlight idle timeout     |
        // |              control. It will select all of the text in the control to allow easy    |
        // |              updating.                                                               |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void BacklightTimeoutBox_Click(object sender, EventArgs e)
        {            
            // Select all text in control
            BacklightTimeoutBox.Select(0, BacklightTimeoutBox.Text.Length);
        }
    }


    // +--------------------------------------------------------------------------------------+
    // | IdleTime Class                                                                       |
    // | Description: This class will be used to check how long the computer has been idle.   |
    // +--------------------------------------------------------------------------------------+
    public class IdleTime
    {
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }
    }

    public class PowerNotification
    {
        [DllImport("User32.dll")]
        public static extern IntPtr RegisterPowerSettingNotification(IntPtr   hRecipient,
                                                                     ref Guid PowerSettingGuid,
                                                                     Int32    Flags);
        [DllImport("User32.dll")]
        public static extern bool UnregisterPowerSettingNotification(IntPtr handle);

        // This structure is sent when the PBT_POWERSETTINGSCHANGE message is sent.
        // It describes the power setting that has changed and contains data about the change
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public Int32 DataLength;
        }

        public const int WM_POWERBROADCAST           = 0x0218;
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        public const int PBT_POWERSETTINGCHANGE      = 0x8013; 
        public static Guid GUID_MONITOR_POWER_ON = new Guid(0x02731015, 0x4510, 0x4526, 0x99, 0xE6, 0xE5, 0xA1, 0x7E, 0xBD, 0x1A, 0xEA);
        
    }
}
