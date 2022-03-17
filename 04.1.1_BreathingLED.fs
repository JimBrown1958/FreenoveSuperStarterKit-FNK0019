#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -BreathingLED
    -BreathingLED
[endif]
marker -BreathingLED

include wiringPi.fs
wiringPiSetup drop


1 constant ledPin
ledPin 0 100 softPwmCreate drop

." Program is Starting " CR

: BreathingLED
	begin
		100 0 do ledPin i softPwmWrite 120 delay loop
		300 delay
		0 100 do ledPin i softPwmWrite 120 delay -1 +loop
		300 delay
	again
;
