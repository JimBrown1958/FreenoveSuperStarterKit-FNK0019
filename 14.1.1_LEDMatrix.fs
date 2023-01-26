#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -LEDMATRIX
    -LEDMATRIX
[endif]
marker -LEDMATRIX

include wiringPi.fs
wiringPiSetup drop

0 constant dataPin	\ DS Pin of 74HC595(Pin14)
2 constant latchPin	\ ST_CP Pin of 74HC595(Pin12)
3 constant clockPin	\ CH_CP Pin of 74HC595(Pin11)

variable x
variable x1

create pic 0x1c C, 0x22 C, 0x51 C, 0x45 C, 0x45 C, 0x51 C, 0x22 C, 0x1c C, 
create data 0x00 C, 0x00 C, 0x00 C, 0x00 C, 0x00 C, 0x00 C, 0x00 C, 0x00 C, \ " "
0x00 C, 0x00 C, 0x3E C, 0x41 C, 0x41 C, 0x3E C, 0x00 C, 0x00 C, \ "0"
0x00 C, 0x00 C, 0x21 C, 0x7F C, 0x01 C, 0x00 C, 0x00 C, 0x00 C, \ "1"
0x00 C, 0x00 C, 0x23 C, 0x45 C, 0x49 C, 0x31 C, 0x00 C, 0x00 C, \ "2"
0x00 C, 0x00 C, 0x22 C, 0x49 C, 0x49 C, 0x36 C, 0x00 C, 0x00 C, \ "3"
0x00 C, 0x00 C, 0x0E C, 0x32 C, 0x7F C, 0x02 C, 0x00 C, 0x00 C, \ "4"
0x00 C, 0x00 C, 0x79 C, 0x49 C, 0x49 C, 0x46 C, 0x00 C, 0x00 C, \ "5"
0x00 C, 0x00 C, 0x3E C, 0x49 C, 0x49 C, 0x26 C, 0x00 C, 0x00 C, \ "6"
0x00 C, 0x00 C, 0x60 C, 0x47 C, 0x48 C, 0x70 C, 0x00 C, 0x00 C, \ "7"
0x00 C, 0x00 C, 0x36 C, 0x49 C, 0x49 C, 0x36 C, 0x00 C, 0x00 C, \ "8"
0x00 C, 0x00 C, 0x32 C, 0x49 C, 0x49 C, 0x3E C, 0x00 C, 0x00 C, \ "9"
0x00 C, 0x00 C, 0x3F C, 0x44 C, 0x44 C, 0x3F C, 0x00 C, 0x00 C, \ "A"
0x00 C, 0x00 C, 0x7F C, 0x49 C, 0x49 C, 0x36 C, 0x00 C, 0x00 C, \ "B"
0x00 C, 0x00 C, 0x3E C, 0x41 C, 0x41 C, 0x22 C, 0x00 C, 0x00 C, \ "C"
0x00 C, 0x00 C, 0x7F C, 0x41 C, 0x41 C, 0x3E C, 0x00 C, 0x00 C, \ "D"
0x00 C, 0x00 C, 0x7F C, 0x49 C, 0x49 C, 0x41 C, 0x00 C, 0x00 C, \ "E"
0x00 C, 0x00 C, 0x7F C, 0x48 C, 0x48 C, 0x40 C, 0x00 C, 0x00 C, \ "F"
0x00 C, 0x00 C, 0x00 C, 0x00 C, 0x00 C, 0x00 C, 0x00 C, 0x00 C, \ " "


: _shiftOut ( n -- )
8 0 ?DO
	DUP
	clockPin LOW digitalWrite
	LSBFIRST = IF
		0x01 x @ I rshift and 0x01 = IF dataPin HIGH digitalWrite ELSE dataPin LOW digitalWrite THEN
		10 delayMicroseconds
	ELSE  \ MSBFIRST 
		0x80 x @ I lshift and 0x80 = IF dataPin HIGH digitalWrite ELSE dataPin LOW digitalWrite THEN
		10 delayMicroseconds
	THEN
	clockPin HIGH digitalWrite
	10 delayMicroseconds
loop
DROP
;


: LEDMATRIX 
." program is Starting ... " CR
dataPin  OUTPUT pinMode
latchPin OUTPUT pinMode
clockPin OUTPUT pinMode
begin
	500 0 ?DO \ Repeat enough times to display the smiling face a period of time
		0x80 x1 !
		8 0 ?DO 
			latchPin LOW digitalWrite 
			pic i + c@ x ! MSBFIRST _shiftOut \ first shift data of line information to the first stage 74HC959
			x1 @  invert x ! MSBFIRST _shiftOut \ then shift data of column information to the second stage 74HC959
			latchPin HIGH digitalWrite \ Output data of two stage 74HC595 at the same time
			x1 @ 1 rshift x1 ! \ display the next column			
			1 delay 						
		LOOP
	LOOP
        144 0 ?DO \  total number of "0-F" data values
		100 0 ?DO \ times of repeated displaying LEDMatrix in every frame, the bigger the first number, the longer the display time
                	0x80 x1 ! \ Set the column information to start from the first column
                	j 8 + j ?DO 
                        	latchPin LOW digitalWrite 
                        	data i + c@ x ! MSBFIRST _shiftOut 
                        	x1 @  invert x ! MSBFIRST _shiftOut 
                        	latchPin HIGH digitalWrite 
                        	x1 @ 1 rshift x1 !                      
                        	1 delay                                                 
                	LOOP
		LOOP
	8 +LOOP
key? until
;

