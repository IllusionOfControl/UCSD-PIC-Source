//*************************************************************************************************
//                                                                                                *
//      USB Computer Status Display                                                               *
//      Copyright Andrew Gehringer 2013                                                           *
//      Contact: projects@agehringer.com  --  www.AGehringer.com                                  *
//                                                                                                *
//------------------------------------------------------------------------------------------------*
//    This program is free software: you can redistribute it and/or modify it under the terms of  *
//    the GNU General Public License as published by the Free Software Foundation, version 3 of   *
//    the license.                                                                                *
//                                                                                                *
//    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY    *
//    without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.   *
//    See the GNU General Public License for more details.                                        *
//                                                                                                *
//    You should have received a copy of the GNU General Public License along with this program.  *
//    If not, see <http://www.gnu.org/licenses/>.                                                 *
//                                                                                                *
//*************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;

namespace UCSD_Application
{
    public partial class LineOptionControl : UserControl
    {
        private byte cycleCounter = 0;
        private byte cycleIndex = 0;
        private String[] hddNames;

        PerformanceCounter cpuCounter;      // Reads current CPU usage
        PerformanceCounter ramCounter;      // Reads current RAM usage
        PerformanceCounter uptime;          // Reads current uptime. (How long computer has been on)
        PerformanceCounter diskTimeCounter; // Calculates HDD disk usage percentage.
        List<PerformanceCounter> diskTimeCounters = new List<PerformanceCounter>(); // Calculates many HDD disk usage percentage.
        PerformanceCounter netDataRecvCounter;
        PerformanceCounter netDataSentCounter;
        

        // +--------------------------------------------------------------------------------------+
        // | LineOptionControl                                                                    |
        // | Description: This is the default constructor for a control that will let us select   |
        // |              items to be written to the LCD display.                                 |
        // | ARGS: None                                                                           |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public LineOptionControl()
        {
            InitializeComponent();

            // +--------------------------------------------------------------+
            // | Get a list of every hard drive in the system and populate    |
            // | the hard drive selection box.                                |
            // +--------------------------------------------------------------+
            PerformanceCounterCategory physicalDiscCat = new System.Diagnostics.PerformanceCounterCategory("PhysicalDisk");
            hddNames = physicalDiscCat.GetInstanceNames();
            foreach (string instance in hddNames)
            {
                if (instance != "_Total")
                {
                    HDDSelComboBox.Items.Add(instance.Substring(2,2));
                }
            }

            // Select first drive as default
            if (HDDSelComboBox.Items.Count > 0)
                HDDSelComboBox.SelectedIndex = 0;

            // +--------------------------------------------------------------+
            // | Get a list of every network card in the system and populate  |
            // | the network card selection box.                              |
            // +--------------------------------------------------------------+
            PerformanceCounterCategory networkInterfaceCat = new PerformanceCounterCategory("Network Interface");
            String[] instancename = networkInterfaceCat.GetInstanceNames();
            int maxWidth = 0, temp = 0;
            foreach (string name in instancename)
            {
                // Add network card name to combo box
                NetSelComboBox.Items.Add(name);
                
                // Measure the width of the text, we will size the dropdown according to the widest text
                temp = TextRenderer.MeasureText(name, NetSelComboBox.Font).Width;
                
                // Find longest width text
                if (temp > maxWidth)
                    maxWidth = temp;
            }
            
            // Set the width according to widest text + some margin.
            NetSelComboBox.DropDownWidth = maxWidth + 5;

            // Select first network card as default
            if (NetSelComboBox.Items.Count > 0)
                NetSelComboBox.SelectedIndex = 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | loadItems                                                                            |
        // | Description: This function is called by the main form. It will load what items the   |
        // |              user has checked and restore other saved form settings.                 |
        // | ARGS: items - Arraylist containing what items are checked, followed by cycle rate,   |
        // |               and finally the selected index of a HDD.                               |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public void loadItems(ArrayList items)
        {
            ArrayList lst = items;
            int i = 0;
            try
            {
                // Load user checked items
                for (i = 0; i < SelectionListBox.Items.Count; i++)
                    SelectionListBox.SetItemChecked(i, (bool)items[i]);

                // Load old cycle rate
                CycleRateUpDown.Value = ((decimal)items[i++]);

                // Load selected HDD index (if possible)
                if ((int)items[i] < HDDSelComboBox.Items.Count)
                    HDDSelComboBox.SelectedIndex = ((int)items[i++]);
                else
                    i++; // Skip this item

                // Load selected NIC index (if possible)
                if ((int)items[i] < NetSelComboBox.Items.Count)
                    NetSelComboBox.SelectedIndex = ((int)items[i++]);
                else
                    i++; // Skip this item

            }
            catch (Exception)
            { }

        }

        // +--------------------------------------------------------------------------------------+
        // | saveItems                                                                            |
        // | Description: This function is called by the main form. It will save what items the   |
        // |              user has checked and store other form settings.                         |
        // | ARGS: items - Arraylist containing what items are checked, followed by cycle rate,   |
        // |               and finally the selected index of a HDD. (OUTPUT)                      |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public void saveItems(ArrayList items)
        {
            // Erase any old data in the array list
            items.Clear();

            // Store what items user has checked in list box
            for (int i = 0; i < SelectionListBox.Items.Count; i++)
                items.Add(SelectionListBox.GetItemChecked(i));

            // Add additional user selected values
            items.Add(CycleRateUpDown.Value);
            items.Add(HDDSelComboBox.SelectedIndex);
            items.Add(NetSelComboBox.SelectedIndex);
        }

        // +--------------------------------------------------------------------------------------+
        // | setCheckedItem                                                                       |
        // | Description: This function is called by the main form. It will allow us to check     |
        // |              a default selection if we have no saved settings.                       |
        // | ARGS: i - index of the item to be selected from list box.                            |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public void setCheckedItem(int i)
        {
            if (i < SelectionListBox.Items.Count)
                SelectionListBox.SetItemChecked(i, true);
        }

        // +--------------------------------------------------------------------------------------+
        // | getString                                                                            |
        // | Description: This function is called by the main form. It return a 40 character      |
        // |              string that is ready to be written to the display.                      |
        // | ARGS: None                                                                           |
        // | RETS: returnStr - 40 character string representing updated system info.              |
        // +--------------------------------------------------------------------------------------+
        public string getString()
        {
            string returnStr = "";    // Final string returned by function
 
            // If we do not have any items selected, return an empty string
            if (SelectionListBox.CheckedItems.Count == 0)
                return (" ".PadRight(40));
            // If we have more than one item selected, cycle through checked items.
            else if (SelectionListBox.CheckedItems.Count > 1)
            {
                cycleCounter++;
                // Check if it is time to change display items.
                if (cycleCounter > CycleRateUpDown.Value)
                {
                    cycleCounter = 1;
                    cycleIndex++;

                    // Check if we have wrapped beyond # of checked items, restart.
                    if (cycleIndex > SelectionListBox.CheckedItems.Count - 1)
                        cycleIndex = 0;
                }
            }

            // +--------------------------------------------------------------+
            // | We now know which checked item we will report on.            |
            // | (cycleIndex) Figure out what text lives at that index and    |
            // | report back with a informational string.                     |
            // +--------------------------------------------------------------+
            switch (SelectionListBox.CheckedItems[cycleIndex].ToString())
            {
                case "CPU Usage":
                    returnStr = getCPUUsage();
                    break;
                case "RAM Usage":
                    returnStr = getRAMUsage();
                    break;
                case "HDD Usage Single":
                    returnStr = getHDDUsage();
                    break;
                case "HDD Usage List":
                    returnStr = getHDDUsageList();
                    break;
                case "Network Usage":
                    returnStr = getNetworkUsage();
                    break;
                case "Date / Time":
                    returnStr = ("Date:    " + DateTime.Now.ToString("MM/dd/yyyy - hh:mm:ss tt"));
                    break;
                case "Up Time":
                    returnStr = getUptime();
                    break;

                default:
                    break;
            }

            returnStr = returnStr.PadRight(40);

            return (returnStr);
        }

        // ------------------ PRIVATE FUNCTIONS START ---------------------------------------------

        // +--------------------------------------------------------------------------------------+
        // | getCPUUsage                                                                          |
        // | Description: This function will return a single hard drive usage % with graph.       |
        // | ARGS: None                                                                           |
        // | RETS: string representing HDD usage as "CPU:   ###% ████████████████████"            |
        // +--------------------------------------------------------------------------------------+        
        private string getCPUUsage()
        {
            string CPUUsage, retStr;
            CPUUsage = Math.Round(cpuCounter.NextValue()).ToString();
            retStr = "CPU:  " + CPUUsage.PadLeft(4, ' ') + "% " + graphText(int.Parse(CPUUsage));

            return retStr;
        }

        // +--------------------------------------------------------------------------------------+
        // | getRAMUsage                                                                          |
        // | Description: This function will return a RAM usage info                              |
        // | ARGS: None                                                                           |
        // | RETS: string representing RAM usage as "RAM  ##### ████████████████████ #####MB"     |
        // |                                              #used <      GRAPH       > #free        |
        // +--------------------------------------------------------------------------------------+
        private string getRAMUsage()
        {
            long totalRAM;
            long usedRAM;
            long availableRAM;
            decimal ramPercentFree;
            decimal ramPercentOccupied;
            string retStr;

            totalRAM = PerformanceInfo.GetTotalMemoryInMiB();                   // Amount of RAM physically installed in box
            availableRAM = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();   // Amount of RAM free to use
            usedRAM = totalRAM - availableRAM;                                  // Amount of RAM currently used
            ramPercentFree = ((decimal)availableRAM / (decimal)totalRAM) * 100;
            ramPercentOccupied = 100 - ramPercentFree;
            retStr = "RAM: " + (usedRAM).ToString().PadLeft(6) + " " + graphText((int)ramPercentOccupied) + " " + availableRAM + "MB";

            return (retStr);
        }

        // +--------------------------------------------------------------------------------------+
        // | getHDDUsage                                                                          |
        // | Description: This function will return a single hard drive usage % with graph.       |
        // | ARGS: None                                                                           |
        // | RETS: string representing HDD usage as "HDD C: ###% ████████████████████"            |
        // +--------------------------------------------------------------------------------------+
        private string getHDDUsage()
        {
            string percentUsageStr, retStr;
            double percentUsage;
            // Try and grab disk usage %. If disk is unavailable catch exception and print placeholder text.
            try
            {
                percentUsage = Math.Round(diskTimeCounter.NextValue());
                // +------------------------------------------------------------+
                // | "% Disk Time" Counters may have values > 100! So to cap    |
                // | the value at 100%, return whichever is lower the reported  |
                // | value, or 100.                                             |
                // | See: http://support.microsoft.com/kb/310067/en-us?p=1      |
                // +------------------------------------------------------------+
                percentUsage = Math.Min(percentUsage, 100);

                percentUsageStr = percentUsage.ToString();
                retStr = "HDD " + HDDSelComboBox.Text + percentUsageStr.PadLeft(4, ' ') + "% " + graphText(int.Parse(percentUsageStr));
            }
            catch
            {
                // +-----------------------------------------------------------+
                // | We caught an exception. Odds are that the user turned off |
                // | or removed this HDD. Replace normal result with NOT FOUND.|
                // +-----------------------------------------------------------+
                retStr = "HDD " + HDDSelComboBox.Text + " NOT FOUND";
            }

            return retStr;
        }

        // +--------------------------------------------------------------------------------------+
        // | getHDDUsageList                                                                      |
        // | Description: This function will return a string of hard drive usage %'s              |
        // | ARGS: None                                                                           |
        // | RETS: string representing HDD usage as "HDD Usage: C:###% D:###% E:###% F:###%"      |
        // +--------------------------------------------------------------------------------------+
        private string getHDDUsageList()
        {
            const int NUM_HDDS_TO_DISPLAY = 4;
            string retStr;
            double percentUsage;

            retStr = "HDD Usage: ";
            for (int i = 0; i < NUM_HDDS_TO_DISPLAY; i++)
            {
                if ((diskTimeCounters != null) && (i < diskTimeCounters.Count))
                {
                    // Try and grab disk usage %. If disk is unavailable catch exception and print placeholder text. ("XXX  ")
                    try
                    {
                        percentUsage = Math.Round(diskTimeCounters[i].NextValue());
                        // +------------------------------------------------------------+
                        // | "% Disk Time" Counters may have values > 100! So to cap    |
                        // | the value at 100%, return whichever is lower the reported  |
                        // | value, or 100.                                             |
                        // | See: http://support.microsoft.com/kb/310067/en-us?p=1      |
                        // +------------------------------------------------------------+
                        percentUsage = Math.Min(percentUsage, 100);

                        retStr = retStr + diskTimeCounters[i].InstanceName.Substring(2,2) +
                                    percentUsage.ToString().PadLeft(3) + "% ";
                    }
                    catch
                    {
                        // +-----------------------------------------------------------+
                        // | We caught an exception. Odds are that the user turned off |
                        // | or removed this HDD. Replace normal result with XXX.      |
                        // +-----------------------------------------------------------+
                        retStr = retStr + diskTimeCounters[i].InstanceName.Substring(2) + "XXX  ";
                    }
                }
            }
            return (retStr);
        }


        // +--------------------------------------------------------------------------------------+
        // | getNetworkUsage                                                                      |
        // | Description: This function will return a network card usage.                         |
        // | ARGS: None                                                                           |
        // | RETS: string representing Network usage: "Net Usage: Rx:##### KB/s | Tx:##### KB/s"  |
        // +--------------------------------------------------------------------------------------+
        private string getNetworkUsage()
        {
            string retStr;
            uint KBytesRx, KBytesTx;

            try
            {
                // Counters return value in bytes. Divide by 1024 to get KB. 
                KBytesRx = (uint) netDataRecvCounter.NextValue() / 1024;
                KBytesTx = (uint) netDataSentCounter.NextValue() / 1024;

                retStr = "Net Usage: Rx:" + KBytesRx.ToString().PadLeft(5) + " KB/s | Tx:" + KBytesTx.ToString().PadLeft(5) + " KB/s";
            }
            catch
            {
                // +-----------------------------------------------------------+
                // | We caught an exception. Odds are user unpowered or        |
                // | removed this HDD. Replace normal result with not found.   |
                // +-----------------------------------------------------------+
                retStr = "Net Usage: NETWORK CARD NOT FOUND";
            }

            return retStr;
        }

        // +--------------------------------------------------------------------------------------+
        // | getUptime                                                                            |
        // | Description: This function will return how long the computer has been powered.       |
        // | ARGS: None                                                                           |
        // | RETS: string representing uptime as "Up Time: ##d ##h ##m ##s"                       |
        // +--------------------------------------------------------------------------------------+
        private string getUptime()
        {
            TimeSpan tsUpTime;
            tsUpTime = TimeSpan.FromSeconds(uptime.NextValue());

            return "Up Time: " + tsUpTime.Days + "d " + tsUpTime.Hours.ToString().PadLeft(2, '0')
                   + "h " + tsUpTime.Minutes.ToString().PadLeft(2, '0') + "m " + tsUpTime.Seconds.ToString().PadLeft(2, '0') + "s";

        }

        // +--------------------------------------------------------------------------------------+
        // | graphText                                                                            |
        // | Description: This function is a bit of a hack to represent an integer value as a     |
        // |              horizontal bar graph. My LCD display will display custom characters for |
        // |              characters 0x10 -> 0x15 (Empty Block -> Full Block). This function will |
        // |              convert a percentage to a 20 character string to be displayed on the LCD|
        // | ARGS: percent - Range: 0-100.                                                        |
        // | RETS: 20 character string representing a bar graph of percent                        |
        // +--------------------------------------------------------------------------------------+
        private string graphText(int percent)
        {
            string returnStr = "";

            // Make sure user does not try to pass a large value. Don't want to return excessively large string.
            if (percent > 100)
                percent = 100;

            for (int i = 0; i < percent; i += 5)
            {
                // Check to see if we need a fraction of a full block (0x10 -> 0x14)
                if (i + 5 > percent)
                {
                    returnStr += (char)(0x10 + (percent - i));
                }
                else
                {
                    // Use a full block. (0x15)
                    returnStr += (char)0x15;
                }
            }

            // Pad the graph out with empty blocks.
            returnStr = returnStr.PadRight(20, (char)0x10);

            return returnStr;
        }



        // +--------------------------------------------------------------------------------------+
        // | HDDSelComboBox_SelectedIndexChanged                                                  |
        // | Description: This function is called when user changes which single HDD should be    |
        // |              graphed. Its job is to create a new performance counter for the updated |
        // |              value.                                                                  |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void HDDSelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // +--------------------------------------------------------------+
            // | Loop through all known hard drive names. Since we only       |
            // | display the drive letter in the combo box, we will need to   |
            // | find the full path needed to create the performance counter. |
            // +--------------------------------------------------------------+
            foreach (string instance in hddNames)
            {
                // Does this path contain our drive letter?
                if (instance.Contains(HDDSelComboBox.Text))
                {
                    if (diskTimeCounter != null)
                    {
                        diskTimeCounter.Dispose();
                        diskTimeCounter = null;
                    }
                    diskTimeCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", instance);
                    diskTimeCounter.NextValue(); // First value is always zero. Throw it away.
                    return;
                }
            }
        }

        // +--------------------------------------------------------------------------------------+
        // | NetSelComboBox_SelectedIndexChanged                                                  |
        // | Description: This function is called when user changes which network card should be  |
        // |              monitored. Its job is to create a new performance counter for the       |
        // |              updated selection.                                                      |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void NetSelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (netDataRecvCounter != null)
            {
                netDataRecvCounter.Dispose();
                netDataRecvCounter = null;
            }
            if (netDataSentCounter != null)
            {
                netDataSentCounter.Dispose();
                netDataSentCounter = null;
            }
            netDataRecvCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", NetSelComboBox.Text);
            netDataSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", NetSelComboBox.Text);

            // Update tool tip so user can read whole name of card
            NetworkCardToolTip.SetToolTip(NetSelComboBox, NetSelComboBox.Text);

            // First value from this performance counter is always 0. Throw this value away.
            netDataRecvCounter.NextValue();
            netDataSentCounter.NextValue();
        }


        // +--------------------------------------------------------------------------------------+
        // | SelectionListBox_ItemCheck                                                           |
        // | Description: This function is called when user checks or unchecks an item from the   |
        // |              list box. Its job is to create & destroy program counter objects.       |
        // | ARGS: Unused                                                                         |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        private void SelectionListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Reset counters to prevent invalid item from being referenced.
            cycleCounter = 0;
            cycleIndex   = 0;

            switch (SelectionListBox.Items[e.Index].ToString())
            {
                case "CPU Usage":
                    if (e.NewValue == CheckState.Checked)
                        cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    else
                    {
                        cpuCounter.Dispose();
                        cpuCounter = null;
                    }
                    break;
                case "RAM Usage":
                    if (e.NewValue == CheckState.Checked)
                        ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                    else
                    {
                        ramCounter.Dispose();
                        ramCounter = null;
                    }
                    break;
                case "HDD Usage Single":
                    if (e.NewValue == CheckState.Checked)
                        HDDSelComboBox_SelectedIndexChanged(null, null);
                    else
                    {
                        diskTimeCounter.Dispose();
                        diskTimeCounter = null;
                    }
                    break;
                case "HDD Usage List":
                    if (e.NewValue == CheckState.Checked)
                    {
                        var cat = new System.Diagnostics.PerformanceCounterCategory("PhysicalDisk");
                        hddNames = cat.GetInstanceNames();
                        foreach (string instance in hddNames)
                        {
                            if (instance != "_Total")
                            {
                                diskTimeCounters.Add(new PerformanceCounter("PhysicalDisk", "% Disk Time", instance));
                                diskTimeCounters[diskTimeCounters.Count - 1].NextValue();
                            }
                        }

                        diskTimeCounters = diskTimeCounters.OrderBy(s => s.InstanceName.Substring(2)).ToList();
                    }
                    else
                    {
                        diskTimeCounters.Clear();
                    }
                    break;
                case "Network Usage":
                    if (e.NewValue == CheckState.Checked)
                    {
                        NetSelComboBox_SelectedIndexChanged(null, null);
                    }
                    else
                    {
                        netDataRecvCounter.Dispose();
                        netDataSentCounter.Dispose();
                        netDataRecvCounter = null;
                        netDataSentCounter = null;
                    }
                    break;
                case "Date / Time":
                    break;
                case "Up Time":
                    if (e.NewValue == CheckState.Checked)
                    {
                        uptime = new PerformanceCounter("System", "System Up Time");
                        uptime.NextValue(); // First value is always zero, throw it away.
                    }
                    else
                    {
                        uptime.Dispose();
                        uptime = null;
                    }
                    break;
                default:
                    break;
            }

        }

    }

    // +--------------------------------------------------------------------------------------+
    // | PerformanceInfo Class                                                                |
    // | Description: This class will be used to read RAM information from the computer.      |
    // +--------------------------------------------------------------------------------------+
    public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 0x100000));
            }
            else
            {
                return -1;
            }

        }

        public static Int64 GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 0x100000));
            }
            else
            {
                return -1;
            }

        }
    }
}
