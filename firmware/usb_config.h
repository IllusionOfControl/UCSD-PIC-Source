//*************************************************************************************************
//                                                                                                *
//    usb_config.h                                                                                *
//    This file is used to define USB communication settings.                                     *
//                                                                                                *
//*************************************************************************************************

#ifndef USBCFG_H
#define USBCFG_H

#define self_power          1

/** DEFINITIONS ****************************************************/
#define USB_EP0_BUFF_SIZE		8	// Valid Options: 8, 16, 32, or 64 bytes.
#define USB_MAX_NUM_INT     	1  // For tracking Alternate Setting
#define USB_MAX_EP_NUMBER     1

//Device descriptor
#define USB_USER_DEVICE_DESCRIPTOR &device_dsc
#define USB_USER_DEVICE_DESCRIPTOR_INCLUDE extern ROM USB_DEVICE_DESCRIPTOR device_dsc

//Configuration descriptors
#define USB_USER_CONFIG_DESCRIPTOR USB_CD_Ptr
#define USB_USER_CONFIG_DESCRIPTOR_INCLUDE extern ROM BYTE *ROM USB_CD_Ptr[]

#define USB_PING_PONG_MODE USB_PING_PONG__FULL_PING_PONG

//#define USB_POLLING
#define USB_INTERRUPT

/* Parameter definitions are defined in usb_device.h */
#define USB_PULLUP_OPTION USB_PULLUP_ENABLE
//#define USB_PULLUP_OPTION USB_PULLUP_DISABLED

#define USB_TRANSCEIVER_OPTION USB_INTERNAL_TRANSCEIVER

#define USB_SPEED_OPTION USB_FULL_SPEED

//Option to enable auto-arming of the status stage of control transfers, if no
#define USB_ENABLE_STATUS_STAGE_TIMEOUTS    //Comment this out to disable this feature.  

#define USB_STATUS_STAGE_TIMEOUT     (BYTE)45   //Approximate timeout in milliseconds
//------------------------------------------------------------------------------------------------------------------

#define USB_SUPPORT_DEVICE

#define USB_NUM_STRING_DESCRIPTORS 3

//#define USB_INTERRUPT_LEGACY_CALLBACKS
#define USB_ENABLE_ALL_HANDLERS
//#define USB_ENABLE_SUSPEND_HANDLER
//#define USB_ENABLE_WAKEUP_FROM_SUSPEND_HANDLER
//#define USB_ENABLE_SOF_HANDLER
//#define USB_ENABLE_ERROR_HANDLER
//#define USB_ENABLE_OTHER_REQUEST_HANDLER
//#define USB_ENABLE_SET_DESCRIPTOR_HANDLER
//#define USB_ENABLE_INIT_EP_HANDLER
//#define USB_ENABLE_EP0_DATA_HANDLER
//#define USB_ENABLE_TRANSFER_COMPLETE_HANDLER

/** DEVICE CLASS USAGE *********************************************/
#define USB_USE_HID

/** ENDPOINTS ALLOCATION *******************************************/

/* HID */
#define HID_INTF_ID           0x00
#define HID_EP 					   1
#define HID_INT_OUT_EP_SIZE      3
#define HID_INT_IN_EP_SIZE       3
#define HID_NUM_OF_DSC           1
#define HID_RPT01_SIZE          28

/** DEFINITIONS ****************************************************/

#endif //USBCFG_H
