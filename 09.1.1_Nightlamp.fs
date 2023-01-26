#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -NIGHTLAMP
    -NIGHTLAMP
[endif]
marker -NIGHTLAMP

include wiringPi.fs
wiringPiSetup drop

variable fd_
variable address
variable cmd
variable chn
variable adcValue
fvariable voltage

0 constant ledPin
0 chn !  
0x84 cmd !
0x4b address !
ledPin 0 100 softPwmCreate
address @ wiringPiI2CSetup fd_ !  \ looking for ADS7830 


fd_ @ 0 wiringPiI2CWrite  drop  \ found valid device if stack = 0



: ?ADS7830 		fd_ @ 0 wiringPiI2CWrite 0= if ." ADS7830 found" ELSE ." ADS7830 not found" THEN ;
: readState 	fd_ @ cmd @ wiringPiI2CWrite  drop fd_ @ wiringPiI2CRead  adcValue ! ;
: NIGHTLAMP
	begin 
              readState 
	      adcValue @ s>f 255e f/ 3.3e f* voltage f!
	      ledPin 100e 255e f/ adcValue @ s>f f* f>s softPwmWrite
	      ." ADC value : " adcValue @ 4 u.r ."   ,  Voltage : " voltage f@ 5 2 1 f.rdp ." v" CR
	      30 delay 
	key? until 
;

