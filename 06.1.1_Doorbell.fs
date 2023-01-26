#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -DOORBELL
    -DOORBELL
[endif]
marker -DOORBELL

include wiringPi.fs
wiringPiSetup drop


0 constant buzzerPin
1 constant buttonPin

: buttonDown buzzerPin HIGH digitalWrite ;
: buttonUp buzzerPin LOW digitalWrite ;
: buttonRead buttonPin digitalRead ;

: DOORBELL
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
	key? until
;
