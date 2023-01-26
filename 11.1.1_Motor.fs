#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -MOTOR
    -MOTOR
[endif]
marker -MOTOR

include wiringPi.fs
wiringPiSetup drop

2 constant motorPin1
0 constant motorPin2
3 constant enablePin

variable fd_
variable address
variable cmd
variable chn
variable adcValue

100e fconstant toHigh
0e fconstant toLow
0e fconstant fromLow
128e fconstant fromhigh

fvariable readvalue

0 chn !  
0x84 cmd !
0x4b address !

address @ wiringPiI2CSetup fd_ !  \ looking for ADS7830 


fd_ @ 0 wiringPiI2CWrite  drop  \ found valid device if stack = 0


: map toHigh toLow f- readvalue f@ fromLow f- f* fromHigh fromLow f- f/ toLow f+ f>s abs ;	

: motor_movement adcValue @ s>f fabs 128e f- readValue f!
	readValue f@ f>s 0> if 
			motorPin1 HIGH digitalWrite 
			motorPin2 LOW digitalWrite 
			." turn Forward..." CR 
		ELSE 
			readValue f@ f>s 0< if 
				motorPin1 LOW digitalWrite 
				motorPin2 HIGH digitalWrite 	
				." turn Back..." CR
			ELSE
				motorPin1 LOW digitalWrite 
				motorPin2 LOW digitalWrite 
				." Motor Stop..." CR
			THEN
		THEN
	enablePin map softPwmWrite 
	." The PWM duty cycle is " readValue f@ fabs 100e f* 127e f/ f>s . ." %" CR
;

: ?ADS7830 	fd_ @ 0 wiringPiI2CWrite 0= if ." ADS7830 found" ELSE ." ADS7830 not found" THEN ;

: readState 	fd_ @ cmd @ wiringPiI2CWrite  drop fd_ @ wiringPiI2CRead  adcValue ! ;

: MOTOR 
	enablePin OUTPUT pinMode 
	motorPin1 OUTPUT pinMode 
	motorPin2 OUTPUT pinMode
	
	enablePin 0 100 softPwmCreate
	
	Begin 
                readstate 
		." ADC value : " adcValue @ . CR
		motor_movement
		100 delay
	key? until
;
