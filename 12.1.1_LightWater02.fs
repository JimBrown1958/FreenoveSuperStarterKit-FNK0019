#! /usr/bin/env gforth

\ Forget and reload definitions if this file is re-included.
[ifdef] -blink
    -blink
[endif]
marker -blink

include wiringPi.fs
wiringPiSetup drop

0 constant dataPin	\ DS Pin of 74HC595(Pin14)
2 constant latchPin	\ ST_CP Pin of 74HC595(Pin12)
3 constant clockPin	\ CH_CP Pin of 74HC595(Pin11)

variable x

: arshift 0 ?do 2/ loop ;            \ 2/ is an arithmetic shift right by one bit
: alshift 0 ?do 2* loop ;            \ 2* shifts left one bit

: _shiftOut 
    8 0 DO
        clockPin LOW digitalWrite
        LSBFIRST 0= IF
            0x01 x @ I arshift and 0x01 = IF dataPin HIGH digitalWrite ELSE dataPin LOW digitalWrite THEN
            10 delayMicroseconds
        ELSE  \ MSBFIRST = 0
            0x80 x @ I alshift and 0x80 = IF dataPin HIGH digitalWrite ELSE dataPin LOW digitalWrite THEN
            10 delayMicroseconds
        THEN
        clockPin HIGH digitalWrite
        10 delayMicroseconds
    loop
;                                                                                                                                                           
: blink 
    ." program is Starting ... " CR
    dataPin  OUTPUT pinMode
    latchPin OUTPUT pinMode
    clockPin OUTPUT pinMode
    begin
        0x01 x !
        8 0 DO 
            latchPin LOW digitalWrite \ Output low level to latchPin
            _shiftOut \ Send serial data to 74HC595
            latchPin HIGH digitalWrite \ Output high level to latchPin, and 74HC595 will update the data to the parallel output port.
            x @ 2* x ! \ make the variable move one bit to left once, then the bright LED move one step to the left once.
            100 delay                       
        loop 
        0x80 x !
        8 0 DO
            latchPin LOW digitalWrite
            _shiftOut
            latchPin HIGH digitalWrite
            x @ 2/ x ! 
            100 delay
        loop
    again
;
