#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -blink
    -blink
[endif]
marker -blink

include wiringPi.fs
wiringPiSetup drop

variable fd_
variable pcf8574_address
variable lcdhd

64 CONSTANT BASE1

BASE1 0 + CONSTANT RS
BASE1 1 + CONSTANT RW
BASE1 2 + CONSTANT EN
BASE1 3 + CONSTANT LED
BASE1 4 + CONSTANT D4
BASE1 5 + CONSTANT D5
BASE1 6 + CONSTANT D6
BASE1 7 + CONSTANT D7

0x27 pcf8574_address !
pcf8574_address @ wiringPiI2CSetup fd_ !  
\ PCF8574T:0x27, PCF8574AT:0x3F

\ fd_ @ 0 wiringPiI2CWrite  drop  \ found valid device if stack = 0

0 Value fd-in
0 Value fd-out
: open-input ( addr u -- )  r/o open-file throw to fd-in ;
: open-output ( addr u -- )  w/o create-file throw to fd-out ;
: close-input ( -- )  fd-in close-file throw ;
: close-output ( -- )  fd-out close-file throw ;
256 CONSTANT max-line
VARIABLE LineLen
fVARIABLE CPUtemp

VARIABLE hour
VARIABLE minute
VARIABLE second

: ?temp
	s" /sys/class/thermal/thermal_zone0/temp" open-input
	pad max-line fd-in read-line THROW
	drop
	LineLen !
	pad LineLen @ >float 1000e f/ CPUtemp f!
	." CPU temperature : " CPUtemp f@ 6 3 1 f.rdp
	lcdhd @ 0 0 lcdPosition
	lcdhd @  pad LineLen @ lcdPrintf
	close-input
;

: printDateTime 
	time&date
	DROP  
	DROP 
	DROP
	hour !
	minute ! 
	second !
	CR ." Time: " hour ? ." :" minute ? ." :" second ? CR
	lcdhd @ 0 1 lcdPosition
	lcdhd @ s" Time" lcdPrintf
;


: ?PCF8574T 		fd_ @ 0 wiringPiI2CWrite 0= if ." PCF8574T found" ELSE ." PCF8574T not found" THEN ;
\ : readState 	fd_ @ cmd @ wiringPiI2CWrite  drop fd_ @ wiringPiI2CRead  adcValue ! ;
: blink			
	BASE1 pcf8574_address @ pcf8574Setup drop

	64 OUTPUT pinMode
	65 OUTPUT pinMode
	66 OUTPUT pinMode
	67 OUTPUT pinMode
	68 OUTPUT pinMode
	69 OUTPUT pinMode
	70 OUTPUT pinMode
	71 OUTPUT pinMode
	LED HIGH digitalWrite
	RW LOW digitalWrite
	2 16 4 RS EN D4 D5 D6 D7 0 0 0 0 lcdInit lcdhd !
	." lcdhd: " lcdhd ? CR


	begin 
		?temp 
		printDateTime
		500 delay
	again 
;

