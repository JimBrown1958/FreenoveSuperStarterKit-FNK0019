#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -ButtonLED
    -ButtonLED
[endif]
marker -ButtonLED

include ../gpios.fs

11 constant ledPin
12 constant buttonPin


ledPin output-pin
buttonPin input-pin
1 buttonPin pin–resmode 

: ledOn       ledPin pinset ;
: ledOff      ledPin pinclr ;
: buttonRead buttonPin pin@ ;


: ButtonLED
	." Program is starting... " CR 
	begin 
		buttonRead 0= IF  
			ledOn ." Button pressed, led turned on >>> " CR 
		ELSE ledOff ." Button is released, led turned off " CR 
		THEN 
	 key? until
  ledOff
;
