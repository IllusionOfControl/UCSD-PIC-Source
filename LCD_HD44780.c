//*************************************************************************************************
//                                                                                                *
//    LCD_HD44780.c                                                                               *
//    This file is the low level driver for the LCD display (HD44780)                             *
//                                                                                                *
//*************************************************************************************************

#include "LCD_HD44780.h"
#include <delays.h>

// +----------------------------------------------------------------------------------------------+
// | LCD_initController                                                                           |
// | Description: This function will initialize the HD44780 controller. It needs a special        |
// |              sequence of bits at power up to init and learn about interface and controller   |
// |              settings.                                                                       |
// | ARGS: None                                                                                   |
// | RETS: None                                                                                   |
// +----------------------------------------------------------------------------------------------+
void LCD_initController(void)
{
    unsigned char customChar[8];
    
    // Following lines set the LCD data pins to be output.
    unsigned char LcdTrisState = LCD_TRIS_PORT; // Read old TRIS state from the port
    LcdTrisState &= 0b11110000;                 // Set lower nibble to 0. (output)
    LCD_TRIS_PORT = LcdTrisState;               // Write the new state out to TRIS

    // Following lines set the LCD control pins to be output, and init them to known state (0)
    TRIS_RW = 0;
    TRIS_RS = 0;
    TRIS_E1 = 0;
    TRIS_E2 = 0;
    E1_PIN = 0;
    E2_PIN = 0;
    RW_PIN = 0;
    RS_PIN = 0;

    // I need to wait > 15ms after power up to start sending commands.
    Delay10KTCYx(20); //Approx 16ms Delay

    // LCD Initialization. The following sequance of commands will initialize
    // the controller and allow it to accept commands.
    LCD_doCommand(0x33, 1); //Wake up.
    LCD_doCommand(0x32, 1); //Wake up, part 2.
    LCD_doCommand(0x28, 1); // 4-bit interface, 2-line display


    LCD_doCommand(0x33, 2); //Wake up.
    LCD_doCommand(0x32, 2); //Wake up, part 2.
    LCD_doCommand(0x28, 2); // 4-bit interface, 2-line display

    LCD_ENTRY_MODE_SET();
    LCD_CLEAR_DISPLAY(); // clear display, return cursor
    LCD_ON_NO_CURSOR();

    // Write 0% char
    customChar[0] = 0x00;// [     ]
    customChar[1] = 0x00;// [     ]
    customChar[2] = 0x00;// [     ]
    customChar[3] = 0x00;// [     ]
    customChar[4] = 0x00;// [     ]
    customChar[5] = 0x00;// [     ]
    customChar[6] = 0x00;// [     ]
    customChar[7] = 0x1F;// [XXXXX]
    LCD_writeCustomChar(0x40, customChar);

    // Write 1% Char
    customChar[0] = 0x10;// [X    ]
    customChar[1] = 0x10;// [X    ]
    customChar[2] = 0x10;// [X    ]
    customChar[3] = 0x10;// [X    ]
    customChar[4] = 0x10;// [X    ]
    customChar[5] = 0x10;// [X    ]
    customChar[6] = 0x10;// [X    ]
    customChar[7] = 0x1F;// [XXXXX]
    LCD_writeCustomChar(0x48, customChar);

    // Write 2% Char
    customChar[0] = 0x18;// [XX   ]
    customChar[1] = 0x18;// [XX   ]
    customChar[2] = 0x18;// [XX   ]
    customChar[3] = 0x18;// [XX   ]
    customChar[4] = 0x18;// [XX   ]
    customChar[5] = 0x18;// [XX   ]
    customChar[6] = 0x18;// [XX   ]
    customChar[7] = 0x1F;// [XXXXX]
    LCD_writeCustomChar(0x50, customChar);

    // Write 3% Char
    customChar[0] = 0x1C;// [XXX  ]
    customChar[1] = 0x1C;// [XXX  ]
    customChar[2] = 0x1C;// [XXX  ]
    customChar[3] = 0x1C;// [XXX  ]
    customChar[4] = 0x1C;// [XXX  ]
    customChar[5] = 0x1C;// [XXX  ]
    customChar[6] = 0x1C;// [XXX  ]
    customChar[7] = 0x1F;// [XXXXX]
    LCD_writeCustomChar(0x58, customChar);

    // Write 4% Char
    customChar[0] = 0x1E;// [XXXX ]
    customChar[1] = 0x1E;// [XXXX ]
    customChar[2] = 0x1E;// [XXXX ]
    customChar[3] = 0x1E;// [XXXX ]
    customChar[4] = 0x1E;// [XXXX ]
    customChar[5] = 0x1E;// [XXXX ]
    customChar[6] = 0x1E;// [XXXX ]
    customChar[7] = 0x1F;// [XXXXX]
    LCD_writeCustomChar(0x60, customChar);

    // Write 5% Char
    customChar[0] = 0x1F; // [XXXXX]
    customChar[1] = 0x1F; // [XXXXX]
    customChar[2] = 0x1F; // [XXXXX]
    customChar[3] = 0x1F; // [XXXXX]
    customChar[4] = 0x1F; // [XXXXX]
    customChar[5] = 0x1F; // [XXXXX]
    customChar[6] = 0x1F; // [XXXXX]
    customChar[7] = 0x1F; // [XXXXX]
    LCD_writeCustomChar(0x68, customChar);

    // Write Other Char 1 (Music Note)
    customChar[0] = 0x03; // [   XX]
    customChar[1] = 0x02; // [   X ]
    customChar[2] = 0x02; // [   X ]
    customChar[3] = 0x02; // [   X ]
    customChar[4] = 0x02; // [   X ]
    customChar[5] = 0x0E; // [ XXX ]
    customChar[6] = 0x1E; // [XXXX ]
    customChar[7] = 0x0E; // [ XXX ]
    LCD_writeCustomChar(0x70, customChar);

    // Write Other Char 2 (Happy Face)
    customChar[0] = 0x00; // [     ]  
    customChar[1] = 0x0A; // [ X X ]
    customChar[2] = 0x0A; // [ X X ]
    customChar[3] = 0x0A; // [ X X ]
    customChar[4] = 0x00; // [     ]
    customChar[5] = 0x11; // [X   X]
    customChar[6] = 0x0E; // [ XXX ]
    customChar[7] = 0x00; // [     ]
    LCD_writeCustomChar(0x78, customChar);
}

// +----------------------------------------------------------------------------------------------+
// | LCD_writeCustomChar                                                                          |
// | Description: This function will write a custom character to both display controllers,        |
// | ARGS: address - Memory address to begin writing from.                                        |
// |                 0x40 -> 0x47 = Char 0                                                        |
// |                 0x48 -> 0x4F = Char 1                                                        |
// |                 ...                                                                          |
// |                 0x78 -> 0x7F = Char 7                                                        |
// |       data    - Array where eight lines of data will be used to define the character.        |
// | RETS: None                                                                                   |
// +----------------------------------------------------------------------------------------------+
void LCD_writeCustomChar(unsigned char address, unsigned char* data)
{
    unsigned char displayNum, charNum;
   
    // Only allow user to write to CGRAM character positions
    if ((address < 0x40) || (address > 0x78))
        return;
    
    // We will write the custom characters to both halfs of the display
    for (displayNum = 1; displayNum <= 2; displayNum++)
    {
        // Setup to address to write custom character on display
        LCD_doCommand(address, displayNum);
        
        for (charNum = 0; charNum < 8; charNum++)
        {
            // Push a line of the character onto display
            LCD_putChar(data[charNum],displayNum);
        }
    }
}

// +----------------------------------------------------------------------------------------------+
// | LCD_doCommand                                                                                |
// | Description: This function sends an 8-bit command to the display. The delay length will need |
// |              to be adjusted for osc speed.                                                   |
// | ARGS: data - data to be sent to the controller.                                              |
// | RETS: None                                                                                   |
// +----------------------------------------------------------------------------------------------+
void LCD_doCommand(char data, char controller)
{

    LCD_setNibble(data >> 4);       // Send high nibble to port
    Delay1KTCYx(LCD_DELAY_LEGNTH);
    if (controller == 1)
        E1_PIN = 1;                 // Strobe the enable pin
    else
        E2_PIN = 1;
    Delay1KTCYx(LCD_DELAY_LEGNTH);
    E1_PIN = 0;
    E2_PIN = 0;

    LCD_setNibble(data & 0x0F);     // Send low nibble to port
    Delay1KTCYx(LCD_DELAY_LEGNTH);
    if (controller == 1)
        E1_PIN = 1;                 // Strobe the enable pin
    else
        E2_PIN = 1;                 // Strobe the enable pin
    Delay1KTCYx(LCD_DELAY_LEGNTH);
    E1_PIN = 0;
    E2_PIN = 0;
}

// +----------------------------------------------------------------------------------------------+
// | LCD_putChar                                                                                  |
// | Description: This function sends one character to the display.                               |
// | ARGS: data - data to be sent to the controller.                                              |
// | RETS: None                                                                                   |
// +----------------------------------------------------------------------------------------------+
void LCD_putChar(char data, char controller)
{
    RS_PIN = 1; // Tell LCD we will be sending characters
    LCD_doCommand(data, controller);
    RS_PIN = 0; // Back to control mode
}

// +----------------------------------------------------------------------------------------------+
// | LCD_putS                                                                                     |
// | Description: This function sends a string of characters stored in RAM to the display.        |
// | ARGS: s - pointer to a null terminated character array (RAM)                                 |
// | RETS: None                                                                                   |
// +----------------------------------------------------------------------------------------------+
void LCD_putS(unsigned char *s, char line)
{
    int i;
    char controller = (line <= 2 ? 1 : 2); // First two lines = controler 1, second two lines = controller 2

    // Output up to max of 40 characters incase string isn't null terminated
    for (i = 0; i < 40; i++)
    {
       // If we find a null character, we're done.
        if (!(*s))
            break;
        // Check for my custom characters and replace them with the percentage blocks
        else if(*s <= 0x18)
        {
            LCD_putChar(*s - 0x10, controller);
            *s++;
        }
        // We have a normal character, print it out.
        else
        {
            LCD_putChar(*s++, controller);
        }
    }
}

// +----------------------------------------------------------------------------------------------+
// | LCD_putrS                                                                                    |
// | Description: This function sends a string of characters stored in ROM to the display.        |
// | ARGS: s - pointer to a null terminated character array (ROM)                                 |
// | RETS: None                                                                                   |
// +----------------------------------------------------------------------------------------------+
void LCD_putrS(const rom char *s, char line)
{
    int i;
    char controller = (line <= 2 ? 1 : 2);

    // Output up to max of 40 characters incase string isn't null terminated
    for (i = 0; i < 40; i++)
    {
        if (!(*s))
            break;

        LCD_putChar(*s++, controller);
    }
}

// +----------------------------------------------------------------------------------------------+
// | LCD_setNibble                                                                                |
// | Description: This function sets the lower nibble of a port to data. It is used because we    |
// |              send nibbles of data in 4-bit mode.                                             |
// | ARGS: data - info to be put on the lcd data port.                                            |
// | RETS: None                                                                                   |
// +----------------------------------------------------------------------------------------------+
void LCD_setNibble(char data)
{
    unsigned char temp = LCD_DATA_PINS; // Read old pin values
    temp &= 0xf0;                       // Erase lower nibble
    temp |= (data & 0x0f);              // Set lower nibble to data
    LCD_DATA_PINS = temp;               // Push result on pins
}