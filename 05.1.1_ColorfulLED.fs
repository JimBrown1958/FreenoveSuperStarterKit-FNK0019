#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -blink
    -blink
[endif]
marker -blink

include wiringPi.fs
require random.fs
rnd 
drop

0 constant ledPinRed
1 constant ledPinGreen
2 constant ledPinBlue
variable r
variable g
variable b

: setupLedPin ledPinRed 0 100 softPwmCreate ledPinGreen 0 100 softPwmCreate ledPinBlue 0 100 softPwmCreate ;

: setLedColor  ledPinRed r @ softPwmWrite ledPinGreen g @ softPwmWrite ledPinBlue b @ softPwmWrite ;

wiringPiSetup drop

: blink
	setupLedPin

	." Program is Starting " CR

	begin
		100 random r !
		100 random g !
		100 random b !
	
		setLedColor
	
		." Red:   " r @ . CR
		." Green: " g @ . CR
		." Blue:  " b @ . CR
		CR
	
		1000 delay
	again
;
