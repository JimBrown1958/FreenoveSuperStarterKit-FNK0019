#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -THERMOMETER
    -THERMOMETER
[endif]
marker -THERMOMETER

include wiringPi.fs

variable fd_
variable address
variable cmd
variable chn
variable adcValue
fvariable voltage
fvariable Rt
fvariable tempK
fvariable tempC

wiringPiSetup drop
0 chn !  
0x84 cmd !
0x4b address !

address @ wiringPiI2CSetup fd_ !  \ looking for ADS7830 


fd_ @ 0 wiringPiI2CWrite  drop  \ found valid device if stack = 0



: ?ADS7830      fd_ @ 0 wiringPiI2CWrite 0= if ." ADS7830 found" ELSE ." ADS7830 not found" THEN ;
: readState     fd_ @ cmd @ wiringPiI2CWrite  drop fd_ @ wiringPiI2CRead  adcValue ! ;
: THERMOMETER         begin 
					  readState 
                      adcValue @ s>f 255e f/ 3.3e f* voltage f!
                      10e voltage f@ f* 3.3e voltage f@ f- f/ Rt f!
                      1e 1e 298.15e f/ Rt f@ 10e f/ flog 3950e f/ f+ f/ tempK f!
                      tempK f@ 273.15e f- tempC f!    
                      ." ADC value : " adcValue @ 4 u.r ."   ,  Voltage : " voltage f@ 5 2 1 f.rdp ." v" ." ,  Temperature: " tempC f@ 5 2 1 f.rdp CR
                      100 delay 
                again
;
