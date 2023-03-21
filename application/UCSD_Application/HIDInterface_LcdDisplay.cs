using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCSD_Application
{
    // +------------------------------------------------------------------------------------------+
    // | This class will extend the generic HID class. It adds functionality to work with the LCD |
    // +------------------------------------------------------------------------------------------+
    public partial class HIDInterface
    {
        // +--------------------------------------------------------------------------------------+
        // | writeString                                                                          |
        // | Description: This function will format and send a message to write a string of text  |
        // |              to the connected LCD HID Device.                                        |
        // | ARGS: Message - The text we wish to write to the LCD                                 |
        // |       LineNum - Line number of the display where text should go. (1 -> 4)            |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public void writeString(String Message, int LineNum)
        {
            byte[] command = new byte[Message.Length + 1]; // Add one to length because I will be modding first byte
            
            // First byte tells LCD this is a write to LineNum
            //  Ex: 0x11 = write to line 1, 0x12 = write to line 2...
            command[0] = (byte)(0x10 + LineNum); 

            // Copy the rest of the string onto the array
            System.Buffer.BlockCopy(Encoding.ASCII.GetBytes(Message), 0, command, 1, Message.Length);

            this.writeByteArr(command);
        }

        // +--------------------------------------------------------------------------------------+
        // | writeCustomChar                                                                      |
        // | Description: This function will write a new custom character to be used on the       |
        // |              display.                                                                |
        // | ARGS: charNum - Character number to be updated. (0 -> 7)                             |
        // |       data    - Array containing 8 entries with range 0x0 -> 0x1F representing one   |
        // |                 line of a 5x8 pixel character.                                       |
        // |                 Example: (Music Note)                                                |
        // |                    data[0] = 0x03; // [   ██]                                        |
        // |                    data[1] = 0x02; // [   █ ]                                        |
        // |                    data[2] = 0x02; // [   █ ]                                        |
        // |                    data[3] = 0x02; // [   █ ]                                        |
        // |                    data[4] = 0x02; // [   █ ]                                        |
        // |                    data[5] = 0x0E; // [ ███ ]                                        |
        // |                    data[6] = 0x1E; // [████ ]                                        |
        // |                    data[7] = 0x0E; // [ ███ ]                                        |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public void writeCustomChar(byte charNum, byte[] data)
        {
            byte[] command = new byte[10];

            command[0] = 0x30;
            command[1] = charNum;
            System.Buffer.BlockCopy(data, 0, command, 2, 8);
            this.writeByteArr(command);
        }

        // +--------------------------------------------------------------------------------------+
        // | clearScreen                                                                          |
        // | Description: This function send a command to erase the LCD of all text.              |
        // | ARGS: None                                                                           |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public void clearScreen()
        {
            byte[] data = { 0x10 };
            this.writeByteArr(data);
        }

        // +--------------------------------------------------------------------------------------+
        // | turnOffBacklight                                                                     |
        // | Description: This function send a command to turn off the FET that controls the      |
        // |              backlight.                                                              |
        // | ARGS: None                                                                           |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public void turnOffBacklight()
        {
            byte[] data = { 0x20 };
            this.writeByteArr(data);
        }

        // +--------------------------------------------------------------------------------------+
        // | turnOnBacklight                                                                      |
        // | Description: This function send a command to turn on the FET that controls the       |
        // |              backlight.                                                              |
        // | ARGS: None                                                                           |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public void turnOnBacklight()
        {
            byte[] data = { 0x21 };
            this.writeByteArr(data);
        }

        // +--------------------------------------------------------------------------------------+
        // | turnOffBacklight                                                                     |
        // | Description: This function send a command to toggle the FET that controls the        |
        // |              backlight.                                                              |
        // | ARGS: None                                                                           |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public void toggleBacklight()
        {
            byte[] data = { 0x22 };
            this.writeByteArr(data);
        }

        // +--------------------------------------------------------------------------------------+
        // | turnOffBacklight                                                                     |
        // | Description: This function send a command to read the status of the backlight. It    |
        // |              will then read the response back from the device.                       |
        // | ARGS: None                                                                           |
        // | RETS: None                                                                           |
        // +--------------------------------------------------------------------------------------+
        public bool isBacklightOn()
        {
            Boolean success = false;
            byte[] data = { 0x23 };
            byte[] data2 = { 0, 0 };
            this.writeByteArr(data);
            success = this.readByteArr(ref data2);
            if (data2[1] == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



    }
}
