#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -blink
    -blink
[endif]
marker -blink

include wiringPi.fs
wiringPiSetup drop

0 constant dataPin	\ DS Pin of 74HC595(Pin14)
2 constant latchPin	\ ST_CP Pin of 74HC595(Pin12)
3 constant clockPin	\ CH_CP Pin of 74HC595(Pin11)

variable x

create num 0xc0 C, 0xf9 C, 0xa4 C, 0xb0 C, 0x99 C, 0x92 C, 0x82 C, 0xf8 C, 0x80 C, 0x90 C, 0x88 C, 0x83 C, 0xc6 C, 0xa1 C, 0x86 C, 0x8e C,

: _shiftOut ( n -- )
	8 0 DO
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
;


: blink 
	." program is Starting ... " CR
	dataPin  OUTPUT pinMode
	latchPin OUTPUT pinMode
	clockPin OUTPUT pinMode
	begin
		16 0 DO 
			latchPin LOW digitalWrite \ Output low level to latchPin
			num I + c@ x ! MSBFIRST _shiftOut \ Send serial data to 74HC595
			latchPin HIGH digitalWrite \ Output high level to latchPin, and 74HC595 will update the data to the parallel output port.
			500 delay 						
		loop 
		16 0 DO
			latchPin LOW digitalWrite				
			num i + c@ 0x7f and x ! MSBFIRST _shiftOut
			latchPin HIGH digitalWrite
			500 delay
		loop
	again
;
