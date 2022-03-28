\ Gforth interface for the wiringPi library.
\ 
\ On Raspbian, you need "sudo apt-get install gforth libtool-bin"
\ and then follow instructions for installing wiringPi library at
\ <http://wiringpi.com/download-and-install/>.

marker -wiringPiDev

\ Update this identifier when the API changes, so that Gforth
\ will build a new cached library.
c-library wiringPiDev_2_70

s" wiringPiDev" add-lib

\c #include <lcd.h>


\ LCD constant
8 constant MAX_LCDS

\ lcd.h
\ Not fully working yet
c-function lcdHome lcdHome n -- void
c-function lcdClear lcdClear n -- void
c-function lcdDisplay lcdDisplay n n -- void
c-function lcdCursor lcdCursor n n -- void
c-function lcdCursorBlink lcdCursorBlink n n -- void
c-function lcdSendCommand lcdSendCommand n n -- void
c-function lcdPosition lcdPosition n n n -- void
c-function lcdCharDef lcdCharDef n n a -- void
c-function lcdPutChar lcdPutchar n n -- void
c-function lcdPuts lcdPuts n a -- void
c-function lcdPrintf lcdPrintf n a a -- void
c-function lcdInit lcdInit n n n n n n n n n n n n n -- n

end-c-library

