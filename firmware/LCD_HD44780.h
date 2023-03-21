//*************************************************************************************************
//                                                                                                *
//    LCD_HD44780.h                                                                               *
//    This file is header for the low level driver LCD display (HD44780)                          *
//                                                                                                *
//*************************************************************************************************

#ifndef __lcd_hd44780_h_
#define __lcd_hd44780_h_

#include "p18cxxx.h" //(for port, tris definitions)  

// Pin Definitions
#define RW_PIN   LATBbits.LATB3   		/* PORT for RW */
#define TRIS_RW  TRISBbits.TRISB3    	/* TRIS for RW */

#define RS_PIN   LATBbits.LATB4   		/* PORT for RS */
#define TRIS_RS  TRISBbits.TRISB4    	/* TRIS for RS */

#define E1_PIN    LATBbits.LATB2  		/* PORT for E  */
#define TRIS_E1   TRISBbits.TRISB2    	/* TRIS for E  */

#define E2_PIN    LATBbits.LATB5  		/* PORT for E  */
#define TRIS_E2   TRISBbits.TRISB5    	/* TRIS for E  */


// I am expecting the data pins to be on the lower nibble of the port. (Pins 0-3)
#define LCD_DATA_PINS LATA
#define LCD_TRIS_PORT TRISA


// Code Constatnts
#define LCD_DELAY_LEGNTH 4 //Approx 0.8ms delay


// Macros
#define LCD_GO_LINE1()        LCD_doCommand(0x80, 1) // Move to 1st line
#define LCD_GO_LINE2()        LCD_doCommand(0xC0, 1) // Move to 2nd line
#define LCD_GO_LINE3()        LCD_doCommand(0x80, 2)
#define LCD_GO_LINE4()        LCD_doCommand(0xC0, 2)

// Increment CGRAM / DRAM counter after write, No Display Shift
#define LCD_ENTRY_MODE_SET()  LCD_doCommand(0x06, 1); LCD_doCommand(0x06, 2);

#define LCD_ON_NO_CURSOR()    LCD_doCommand(0x0C, 1); LCD_doCommand(0x0C, 2);
#define LCD_ON_BLINK_CURSOR() LCD_doCommand(0x0D, 1); LCD_doCommand(0x0D, 2);

#define LCD_CLEAR_DISPLAY()   LCD_doCommand(0x01, 1); LCD_doCommand(0x01, 2);


// Function prototypes
void LCD_initController(void);
void LCD_setNibble(char data);
void LCD_doCommand(char data, char controller);
void LCD_putChar(char data, char controller);
void LCD_putrS(const rom char *s, char line);
void LCD_putS(unsigned char *s, char line);
void LCD_writeCustomChar(unsigned char address, unsigned char* data);

#endif