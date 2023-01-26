#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -Tablelamp
    -Tablelamp
[endif]
marker -Tablelamp

include wiringPi.fs
wiringPiSetup drop

0 constant ledPin 
1 constant buttonPin 
50 constant captureTime 


variable ledState 
variable buttonState 
variable lastbuttonState 
variable lastChangeTime 
variable reading

LOW ledState !  
HIGH buttonState ! 
HIGH lastbuttonState ! 

ledPin    OUTPUT pinMode 
buttonPin INPUT pinMode 
buttonPin PUD_UP pullUpDnControl 

: ledOn      ledPin HIGH digitalWrite ;
: ledOff     ledPin LOW digitalWrite ;
: buttonRead buttonPin digitalRead ;
: ledSwitch  ledState @ 0= IF 1 ledState ! ELSE 0 ledState ! THEN ;

: Tablelamp     ." Program is starting... " CR 
		begin
			buttonRead reading ! 
			reading @ lastbuttonState @ <>  IF
				millis lastChangeTime !
			THEN

			millis lastChangeTime @ - captureTime > IF
				reading @ buttonState @ <> IF
					reading @  buttonState !
					buttonState @ LOW = IF
						." Button is pressed!" CR
						ledSwitch
						ledState @  if
							." turn on LED ... " CR
						ELSE
							." turn off LED ... " CR
						THEN
					THEN
				ELSE
					." Button is released!" CR
				THEN
			THEN
			ledPin ledState @ digitalWrite
			reading @ lastbuttonState !
		key? until
;
