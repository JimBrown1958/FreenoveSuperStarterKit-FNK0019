#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -blinker
    -blinker
[endif]
marker -blinker

include ./gpios.fs


11 constant ledPin
ledPin output-pin


: delay ( n -- ) 2000 ms ;
: startMessage ( -- )   CR ." Program is starting ..." CR ;
: pinUsedMessage ( -- )  ." Using physical pin " ledPin . CR ;
: onMessage ( -- )      ." led turned on >>>" CR ;
: offMessage ( -- )     ." led turned off >>>" CR ;

: ledOn ( -- )      ledPin pinset onMessage delay ;
: ledOff ( -- )     ledPin pinclr offMessage delay ;

: blinker  startMessage 
			pinUsedMessage 
			begin 
				ledOn 
				ledOff 
			key? until
  ledOff 
;
