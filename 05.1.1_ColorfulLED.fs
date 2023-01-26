#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -ColorfulLED
    -ColorfulLED
[endif]
marker -ColorfulLED

include wiringPi.fs
require random.fs
rnd 
drop
wiringPiSetup drop


0 constant ledPinRed
1 constant ledPinGreen
2 constant ledPinBlue
variable rvar
variable gvar
variable bvar

: setupLedPin ledPinRed 0 100 softPwmCreate ledPinGreen 0 100 softPwmCreate ledPinBlue 0 100 softPwmCreate ;

: setLedColor  ledPinRed rvar @ softPwmWrite ledPinGreen gvar @ softPwmWrite ledPinBlue bvar @ softPwmWrite ;

: ColorfulLED
	setupLedPin

	." Program is Starting " CR

	begin
		100 random rvar !
		100 random gvar !
		100 random bvar !
	
		setLedColor
	
		." Red:   " rvar @ . CR
		." Green: " gvar @ . CR
		." Blue:  " bvar @ . CR
		CR
	
		1000 delay
	key? until
;
