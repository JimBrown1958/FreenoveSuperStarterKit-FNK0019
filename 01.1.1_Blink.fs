#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -blink
    -blink
[endif]
marker -blink

include wiringPi.fs
wiringPiSetup drop

0 constant ledPin
ledPin OUTPUT pinMode



: startMge    ." Program is starting ..." CR ;
: pinUsedMge  ." Using pin " ledPin . ;
: onMge       ." led turned on >>>" CR ;
: offMge      ." led turned off >>>" CR ;

: ledOn       ledPin HIGH digitalWrite ;
: ledOff      ledPin LOW digitalWrite ;

: blink     startMge 
			pinUsedMge 
			begin 
				ledOn 
				onMge 
				1000 delay 
				ledOff 
				offMge 
				1000 delay 
			again 
;
