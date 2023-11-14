#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -blinker
    -blinker
[endif]
marker -blinker

include ./gpios.fs


11 constant ledPin
ledPin output-pin


: delay ( n -- ) ms ;
: startMessage ( -- )   CR ." Program is starting ..." CR ;
: pinUsedMessage ( -- )  ." Using physical pin " ledPin . CR ;
: onMessage ( -- )      ." led turned on >>>" CR ;
: offMessage ( -- )     ." led turned off >>>" CR ;

: ledOn ( -- )      ledPin pinset ;
: ledOff ( -- )     ledPin pinclr ;

: blinker  startMessage 
			pinUsedMessage 
			begin 
				ledOn 
				onMessage 
				2000 delay 
				ledOff 
				offMessage 
				2000 delay 
			key? until
  ledOff 
;
