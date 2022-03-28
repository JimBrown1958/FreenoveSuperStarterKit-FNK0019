#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -ALERTOR
    -ALERTOR
[endif]
marker -ALERTOR

include wiringPi.fs
wiringPiSetup drop


0 constant buzzerPin
1 constant buttonPin
fvariable sinVal
fvariable toneVal

: buttonRead buttonPin digitalRead ;

: alert_noise 
	360 0 do 
		pi 180e f/ i s>f f* fsin sinVal f!
		sinVal f@ 500e f* 2000e f+ toneVal f!
		." Toneval: " toneVal f@ f.
		buzzerPin toneVal f@ f>s softToneWrite
		1 delay
	loop
;

: stopAlert_noise
	buzzerPin 0 softToneWrite
;

: buttonRead buttonPin digitalRead ;

: ALERTOR
	." Program is starting ... " CR
	
	buzzerPin OUTPUT pinMode
	buttonPin INPUT pinMode
	buzzerPin softToneCreate DROP
	buttonPin PUD_UP pullUpDnControl
	
	begin
		buttonRead LOW = IF  
			alert_noise
			." Alertor turned on >>> " CR 
		ELSE 
			stopAlert_noise 
			." Alertor turned off <<< " CR 
		THEN 
	again
;	
