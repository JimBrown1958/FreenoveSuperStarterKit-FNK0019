#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -blinker
    -blinker
[endif]
marker -blinker

include /usr/share/gforth/0.7.9_20231012/arch/arm64/gpios.fs


11 constant ledPin
ledPin output-pin



: startMge    ." Program is starting ..." CR ;
: pinUsedMge  ." Using pin " ledPin . CR ;
: onMge       ." led turned on >>>" CR ;
: offMge      ." led turned off >>>" CR ;

: ledOn       ledPin pinset ;
: ledOff      ledPin pinclr ;

: blinker     startMge 
			pinUsedMge 
			begin 
				ledOn 
				onMge 
				1000 delay 
				ledOff 
				offMge 
				1000 delay 
			key? until
  ledOff 
;