#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -Tablelamp
    -Tablelamp
[endif]
marker -Tablelamp

include ./gpios.fs

11 constant ledPin
12 constant buttonPin
50 constant captureTime 
0 constant LOW
1 constant HIGH


variable ledState 
variable buttonState 
variable lastbuttonState 
variable lastChangeTime 
variable reading

LOW ledState !  
HIGH buttonState ! 
HIGH lastbuttonState ! 

ledPin output-pin
buttonPin input-pin
HIGH buttonPin pin-resmode

: ledOn ( -- )      ledPin pinset ;
: ledOff ( -- )     ledPin pinclr ;
: buttonRead ( -- n ) buttonPin pin@ ;
: ledSwitch ( -- ) ledState @ 0= IF 1 ledState ! ELSE 0 ledState ! THEN ;
: millis ( -- n )cputime drop drop drop ;

: Tablelamp ( -- )    ." Program is starting... " CR 
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
							ledOn
						ELSE
							." turn off LED ... " CR
							ledOff
						THEN
					THEN
				ELSE
\					." Button is released!" CR
				THEN
			THEN
			ledPin ledState @ pin!
			reading @ lastbuttonState !
		key? until
		ledOff
;
