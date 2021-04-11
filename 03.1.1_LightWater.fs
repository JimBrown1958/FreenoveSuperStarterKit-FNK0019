#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -blink
    -blink
[endif]
marker -blink

include wiringPi.fs
wiringPiSetup drop

10 CONSTANT ledCounts
VARIABLE pins  ledCounts CELLS ALLOT

: ledInit 1 ledCounts + 0 DO i pins i CELLS + ! loop ;

: blink
	." Program is starting ... " CR
	ledInit
	1 ledCounts + 0 DO i CELLS pins + @ OUTPUT pinMode loop 
	BEGIN
		 1 ledCounts + 0 DO i CELLS pins + @ LOW digitalWrite 100 DELAY i CELLS pins + @ HIGH digitalWrite loop

		 0  1 ledCounts + DO i CELLS pins + @ LOW digitalWrite 100 DELAY i CELLS pins + @ HIGH digitalWrite -1 +loop
	again
;
