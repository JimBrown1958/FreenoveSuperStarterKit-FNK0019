sudo apt-get install gforth

sudo apt-get install libtool-bin




cd /tmp
wget https://project-downloads.drogon.net/wiringpi-latest.deb
sudo dpkg -i wiringpi-latest.deb

gpio -v





https://github.com/JimBrown1958/FreenoveSuperStarterKit-FNK0019.git

The first time you use wiringPi.fs, Gforth will build a library in your /root/.gforth/libcc-named/.libs/ directory. Subsequently, Gforth will reuse that cached library. Note that Gforth will continue to use that cached library even if you make your own changes to wiringPi.fs, so if you do make changes, you need to delete the libraries in that directory or change the name for the c-library declaration in wiringPi.fs.

there is a hack for exercise 15,the wiringPi library needs to be rebuilt until I find out how to include wiringPiDev.  The files lcd.c and lcd.h have to be copied from $HOME/WiringPi/devLib to $HOME/WiringPi/wiringPi and the lines in the file Makefile changed from:
softPwm.c softTone.c
to:
softPwm.c softTone.c lcd.c

and the line: 
lcd.o: lcd.h

inserted into Makefile after line:
pseudoPins.o: wiringPi.h pseudoPins.h
and before line:
wpiExtensions.o: wiringPi.h mcp23008.h mcp23016.h mcp23017.h mcp23s08.h

Once changes are made, then cd to $HOME/WiringPi dir and run ./build.  this will add in lcd to wiringPi library.