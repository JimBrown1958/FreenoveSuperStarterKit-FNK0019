#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -ButtonLED
    -ButtonLED
[endif]
marker -ButtonLED

include wiringPi.fs
wiringPiSetup drop

0 constant ledPin
1 constant buttonPin


ledPin OUTPUT pinMode
buttonPin INPUT pinMode
buttonPin PUD_UP pullUpDnControl

: ledOn ledPin HIGH digitalWrite ;
: ledOff ledPin LOW digitalWrite ;
: buttonRead buttonPin digitalRead ;


: ButtonLED
	." Program is starting... " CR 
	begin 
		buttonRead 0= IF  
			ledOn ." Button pressed, led turned on >>> " CR 
		ELSE ledOff ." Button is released, led turned off " CR 
		THEN 
	again 
;
