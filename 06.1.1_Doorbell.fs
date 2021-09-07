#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -blink
    -blink
[endif]
marker -blink

include wiringPi.fs
wiringPiSetup drop


0 constant buzzerPin
1 constant buttonPin

: buttonDown buzzerPin HIGH digitalWrite ;
: buttonUp buzzerPin LOW digitalWrite ;
: buttonRead buttonPin digitalRead ;

: blink
	." Program is starting ... " CR
	
	buzzerPin OUTPUT pinMode
	buttonPin INPUT pinMode
	
	buttonPin PUD_UP pullUpDnControl
	
	begin
		buttonRead LOW = IF  
				buttonDown ." Buzzer turned on >>> " CR 
			ELSE 
				buttonUp ." Buzzer turned off " CR 
			THEN 
	again 
;
