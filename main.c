//*************************************************************************************************
//                                                                                                *
//    Main.c                                                                                      *
//    This file is used to do all high level processing for USB support. It will make calls to    *
//    the other files to do the actual work.                                                      *
//                                                                                                *
//*************************************************************************************************

// Defines
#define BACKLIGHT     LATCbits.LATC0 // Pin where FET that controls backlight is
#define BACKLIGHT_OFF 0              // FET off == backlight off
#define BACKLIGHT_ON  1              // FET on  == backlight on
#define FW_STRING     "Firmware: v1.2 - Feb 16 2012"
#define NAME_STRING   "Illusion of control"


// Includes
#include "./USB/usb.h"
#include "./USB/usb_function_hid.h"
#include "LCD_HD44780.h"

// PIC Configuration Settings
#pragma config PLLDIV   = 5         // 20 MHz crystal
#pragma config CPUDIV   = OSC1_PLL2
#pragma config USBDIV   = 2         // Clock source from 96MHz PLL/2
#pragma config FOSC     = HSPLL_HS
#pragma config FCMEN    = OFF
#pragma config IESO     = OFF
#pragma config PWRT     = OFF
#pragma config BOR      = ON
#pragma config BORV     = 3
#pragma config VREGEN   = ON        //USB Voltage Regulator
#pragma config WDT      = OFF
#pragma config WDTPS    = 32768
#pragma config MCLRE    = ON
#pragma config LPT1OSC  = OFF
#pragma config PBADEN   = OFF
//#pragma config CCP2MX   = ON
#pragma config STVREN   = ON
#pragma config LVP      = OFF
//#pragma config ICPRT    = OFF     // Dedicated In-Circuit Debug/Programming
#pragma config XINST    = OFF       // Extended Instruction Set
#pragma config CP0      = OFF
#pragma config CP1      = OFF
//#pragma config CP2      = OFF
//#pragma config CP3      = OFF
#pragma config CPB      = OFF
//#pragma config CPD      = OFF
#pragma config WRT0     = OFF
#pragma config WRT1     = OFF
//#pragma config WRT2     = OFF
//#pragma config WRT3     = OFF
#pragma config WRTB     = OFF       // Boot Block Write Protection
#pragma config WRTC     = OFF
//#pragma config WRTD     = OFF
#pragma config EBTR0    = OFF
#pragma config EBTR1    = OFF
//#pragma config EBTR2    = OFF
//#pragma config EBTR3    = OFF
#pragma config EBTRB    = OFF


// Global Variables
#pragma udata USB_VARIABLES=0x500
unsigned char ReceivedDataBuffer[64];
unsigned char ToSendDataBuffer[64];
#pragma udata

USB_HANDLE USBOutHandle = 0; // USB handle.  Must be initialized to 0 at startup.
USB_HANDLE USBInHandle  = 0; // USB handle.  Must be initialized to 0 at startup.

// Function Prototypes
static void InitializeSystem(void);
void ProcessIO(void);
void UserInit(void);
void HighPriorityISRCode();
void LowPriorityISRCode();

//- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

// Interrupt Service Routies

#pragma code HIGH_INTERRUPT_VECTOR = 0x08
void High_ISR (void)
{
     _asm goto HighPriorityISRCode _endasm
}
#pragma code LOW_INTERRUPT_VECTOR = 0x18
void Low_ISR (void)
{
     _asm goto LowPriorityISRCode _endasm
}

#pragma interrupt HighPriorityISRCode
void HighPriorityISRCode()
{
    #if defined(USB_INTERRUPT)
        USBDeviceTasks();
    #endif
}

#pragma interruptlow LowPriorityISRCode
void LowPriorityISRCode()
{
}

//- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

// +----------------------------------------------------------------------------------------------+
// | main                                                                                         |
// | Description: This function will serve as the main loop for the program. It should sit in a   |
// |              loop forever and process USB commands.                                          |
// | Arguments:   None                                                                            |
// | Returns:     None                                                                            |
// +----------------------------------------------------------------------------------------------+
#pragma code
void main(void)
{
   InitializeSystem();
   LCD_initController();

   #if defined(USB_INTERRUPT)
      USBDeviceAttach();
   #endif

   while(1)
   {
      #if defined(USB_POLLING)
         // Check bus status and service USB interrupts.
         USBDeviceTasks();
      #endif

      ProcessIO();
   }
}

// +----------------------------------------------------------------------------------------------+
// | InitializeSystem                                                                             |
// | Description: This function will init all IO pins and make calls to init USB.                 |
// | Arguments:   None                                                                            |
// | Returns:     None                                                                            |
// +----------------------------------------------------------------------------------------------+
static void InitializeSystem(void)
{
   // Default all pins to digital
   ADCON1 = 0x0F;

   // Set ports to be outputs
   TRISA = 0;
   TRISB = 0;
   TRISC = 0;

   // Clear all ports
   PORTA = 0;
   PORTB = 0;
   PORTC = 0;

   //initialize the variable holding the handle for the last transmission
   USBOutHandle = 0;
   USBInHandle  = 0;

   USBDeviceInit();	// usb_device.c.  Initializes USB module SFRs and firmware
   					   // variables to known states.
}

// +----------------------------------------------------------------------------------------------+
// | ProcessIO                                                                                    |
// | Description: This function process all USB tasks. (Reading and responding to HID requests)   |
// | Arguments:   None                                                                            |
// | Returns:     None                                                                            |
// +----------------------------------------------------------------------------------------------+
void ProcessIO(void)
{
   static char connect = 0;
   static char firstCommandSent = 0;

   // +----------------------------------------------------------------------+
   // | If device is not configured, then display a message and leave. We    |
   // | won't be processing any messages.                                    |
   // +----------------------------------------------------------------------+
   if((USBDeviceState < CONFIGURED_STATE) || (USBSuspendControl == 1))
   {
      // We are not configured
      if (connect == 0)
      {
         BACKLIGHT = BACKLIGHT_OFF; // Turn off backlight to reduce power consumption.
         connect = 1;               // Don't need to continuously clear and write message, just do it once.
         LCD_CLEAR_DISPLAY();
         LCD_putrS("Connect USB", 1);

         LCD_GO_LINE3();
         LCD_putrS(FW_STRING, 3);
      
         LCD_GO_LINE4();
         LCD_putrS(NAME_STRING, 3);
      }

      return;
   }

   // +-----------------------------------------------------------------------+
   // | If we get here, device is configured. Yay! Now user needs to launch   |
   // | the supporting software to display info on screen. Post a hint.       |
   // +-----------------------------------------------------------------------+
   if (connect == 1)
   {
      connect = 0;
      firstCommandSent = 0;
      LCD_CLEAR_DISPLAY();
      LCD_putrS("Connected. Launch support program.", 1);

      // LCD_GO_LINE2(); //USELESS ??
      LCD_putrS("Hello World " , 2);

      BACKLIGHT = BACKLIGHT_ON;

   }

   //Check if we have received an OUT data packet from the host
   if(!HIDRxHandleBusy(USBOutHandle))
   {
      // +------------------------------------------------------------------+
      // | If this is the first command we find over USB, clear the display |
      // | of my startup message. (So I don't have to rely on support       |
      // | software to do this when waking from a suspend.                  |
      // +------------------------------------------------------------------+
      if (firstCommandSent = 0)
      {
         firstCommandSent = 1;
         LCD_CLEAR_DISPLAY();
         LCD_GO_LINE1();
      }

      // Command mode
      switch(ReceivedDataBuffer[0])
      {
         case 0x10:
            LCD_CLEAR_DISPLAY();
            LCD_GO_LINE1();
            break;

         case 0x11:
            LCD_GO_LINE1();
            LCD_putS(&ReceivedDataBuffer[1], 1);
            break;

         case 0x12:
            LCD_GO_LINE2();
            LCD_putS(&ReceivedDataBuffer[1], 2);
            break;

         case 0x13:
            LCD_GO_LINE3();
            LCD_putS(&ReceivedDataBuffer[1], 3);
            break;

         case 0x14:
            LCD_GO_LINE4();
            LCD_putS(&ReceivedDataBuffer[1], 4);
            break;

         case 0x20:
            BACKLIGHT = BACKLIGHT_OFF;
            break;
         case 0x21:
            BACKLIGHT = BACKLIGHT_ON;
            break;
         case 0x22:  // Toggle the backlight
            BACKLIGHT ^= 1;
            break;
         case 0x23:
            ToSendDataBuffer[0] = BACKLIGHT;
            // Transmit the response to the host
            if(!HIDTxHandleBusy(USBInHandle))
            {
               USBInHandle = HIDTxPacket(HID_EP,(BYTE*)&ToSendDataBuffer[0],64);
            }
            break;

         case 0x30:
            LCD_writeCustomChar((ReceivedDataBuffer[1]*8)+0x40, &ReceivedDataBuffer[2]);
            break;

         default:	// Unknown command received
            break;
      }

      // Re-arm the OUT endpoint for the next packet
      USBOutHandle = HIDRxPacket(HID_EP,(BYTE*)&ReceivedDataBuffer,64);
   }
}

// +----------------------------------------------------------------------------------------------+
// | USBCBSuspend                                                                                 |
// | Description: Call back that is invoked when a USB suspend is detected. Power consumption     |
// |              should be reduced to < 2.5mA.                                                   |
// | Arguments:   None                                                                            |
// | Returns:     None                                                                            |
// +----------------------------------------------------------------------------------------------+
void USBCBSuspend(void)
{
}

// +----------------------------------------------------------------------------------------------+
// | USBCBWakeFromSuspend                                                                         |
// | Description: The host may put USB peripheral devices in low power suspend mode (by "sending" |
// |              3+ms of idle).  Once in suspend mode, the host may wake the device back up by   |
// |              sending non-idle state signalling.                                              |
// | Arguments:   None                                                                            |
// | Returns:     None                                                                            |
// +----------------------------------------------------------------------------------------------+
void USBCBWakeFromSuspend(void)
{
}

// +----------------------------------------------------------------------------------------------+
// | USBCB_SOF_Handler                                                                            |
// | Description: The USB host sends out a SOF packet to full-speed devices every 1 ms. This      |
// |              interrupt may be useful for isochronous pipes. End designers should implement   |
// |              callback routine as necessary.                                                  |
// | Arguments:   None                                                                            |
// | Returns:     None                                                                            |
// +----------------------------------------------------------------------------------------------+
void USBCB_SOF_Handler(void)
{
   // No need to clear UIRbits.SOFIF to 0 here. Callback caller is already doing that.
}

// +----------------------------------------------------------------------------------------------+
// | USBCBErrorHandler                                                                            |
// | Description: The purpose of this callback is mainly for debugging during development. Check  |
// |              UEIR to see which error causes the interrupt.                                   |
// | Arguments:   None                                                                            |
// | Returns:     None                                                                            |
// +----------------------------------------------------------------------------------------------+
void USBCBErrorHandler(void)
{
   // No need to clear UEIR to 0 here. Callback caller is already doing that.
}

// +----------------------------------------------------------------------------------------------+
// | USBCBCheckOtherReq                                                                           |
// | Description: When SETUP packets arrive from the host, some firmware must process the request |
// |              and respond appropriately to fulfill the request. A HID class device needs to   |
// |              be able to respond to "GET REPORT" type of requests.                            |
// | Arguments:   None                                                                            |
// | Returns:     None                                                                            |
// +----------------------------------------------------------------------------------------------+
void USBCBCheckOtherReq(void)
{
   USBCheckHIDRequest();
}

// +----------------------------------------------------------------------------------------------+
// | USBCBInitEP                                                                                  |
// | Description: This function is called when the device becomes initialized, which occurs after |
// |              the host sends a 	SET_CONFIGURATION (wValue not = 0) request.  This callback    |
// |              function should initialize the endpoints for the device.                        |
// | Arguments:   None                                                                            |
// | Returns:     None                                                                            |
// +----------------------------------------------------------------------------------------------+
void USBCBInitEP(void)
{
   //enable the HID endpoint
   USBEnableEndpoint(HID_EP,USB_IN_ENABLED|USB_OUT_ENABLED|USB_HANDSHAKE_ENABLED|USB_DISALLOW_SETUP);
   //Re-arm the OUT endpoint for the next packet
   USBOutHandle = HIDRxPacket(HID_EP,(BYTE*)&ReceivedDataBuffer,64);
}

// +----------------------------------------------------------------------------------------------+
// | USER_USB_CALLBACK_EVENT_HANDLER                                                              |
// | Description: This function is called from the USB stack to notify a user application that a  |
// |              USB event occured. This callback is in interrupt context when the USB_INTERRUPT |
// |              option is selected.                                                             |
// | Arguments:   event  - the type of event                                                      |
// |              *pdata - pointer to the event data                                              |
// |              size   - size of the event data                                                 |
// | Returns:     None                                                                            |
// +----------------------------------------------------------------------------------------------+
BOOL USER_USB_CALLBACK_EVENT_HANDLER(int event, void *pdata, WORD size)
{
   switch(event)
   {
      case EVENT_TRANSFER:
         //Add application specific callback task or callback function here if desired.
         break;
      case EVENT_SOF:
         USBCB_SOF_Handler();
         break;
      case EVENT_SUSPEND:
         USBCBSuspend();
         break;
      case EVENT_RESUME:
         USBCBWakeFromSuspend();
         break;
      case EVENT_CONFIGURED:
         USBCBInitEP();
         break;
      case EVENT_SET_DESCRIPTOR:
         break; //Optional SET_DESCRIPTOR request. Not Implemented
      case EVENT_EP0_REQUEST:
         USBCBCheckOtherReq();
         break;
      case EVENT_BUS_ERROR:
         USBCBErrorHandler();
         break;
      case EVENT_TRANSFER_TERMINATED:
         //EVENT_TRANSFER_TERMINATED event occurs when the host performs a CLEAR FEATURE (endpoint
         //halt) request on an application endpoint which was previously armed (UOWN was = 1).
         //Here would be a good place to:
         //1.  Determine which endpoint the transaction that just got terminated was on, by
         //    checking the handle value in the *pdata.
         //2.  Re-arm the endpoint if desired (typically would be the case for OUT endpoints)
         break;
      default:
         break;
   }
   return TRUE;
}
