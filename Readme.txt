To install gforth  
=================
If you are using the pre Bookworm version of raspberry Pi OS, use the gforth version in the repo: 
    sudo apt intall gforth*
 This will install an old version of gforth but it should still work.
 The latest version of gforth relies on at least version GLIBC 2.33
 the version on older RPI is 2.31

If you are using the latest Bookworm version of raspberry Pi OS, use the following to install gforth dev version:
1)  sudo apt install apt-transport-https

2a) Note: use armhf for 32 bit PI OS and arm64 for 64 bit PI OS 
    sudo cat >/etc/apt/sources.list.d/net2o.list <<EOF
    deb [arch=armhf] https://net2o.de/debian stable main
    EOF

or

2b) if previous steps fail then edit file and add the line to:
    sudo vi /etc/apt/sources.list.d/net2o.list
    deb [arch=armhf] https://net2o.de/debian stable main

3)  sudo wget -O - https://net2o.de/bernd@net2o.de-yubikey.pgp.asc | sudo apt-key add -

4)  sudo apt update
5)  sudo apt install gforth*

To install Forth examples
=========================
git clone https://github.com/JimBrown1958/FreenoveSuperStarterKit-FNK0019.git
sudo apt-get install libtool-bin

To install WiringPi for Freenove kit
====================================
git clone https://github.com/WiringPi/WiringPi
cd WiringPi
./build

gpio -v

To install Freenove Project code (C, Python, Java, Scratch)
===========================================================
cd
git clone --depth 1 https://github.com/freenove/Freenove_Super_Starter_Kit_for_Raspberry_Pi
mv Freenove_Super_Starter_Kit_for_Raspberry_Pi/ Freenove_Kit/


The first time you use wiringPi.fs, Gforth will build a library in your /root/.gforth/libcc-named/.libs/ directory (on 64bit PI OS the path is /home/pi/.cache/gforth/arm64/libcc-tmp/.libs). Subsequently, Gforth will reuse that cached library. Note that Gforth will continue to use that cached library even if you make your own changes to wiringPi.fs, so if you do make changes, you need to delete the libraries in that directory or change the name for the c-library declaration in wiringPi.fs.

To run examples
===============
cd to directory containing forth example file (FreenoveSuperStarterKit-FNK0019)
gforth <program name>
example:
gforth 01.1.1_Blinker.fs

Update
======
Work on removing dependency on wiringPi is now underway.  I hope to use the gpios.fs file included in gforth.
(Update) 2 modules have now been updated to use the gpios.fs file.  For RPi 4 and RPi400 please use the gpios.fs file in the program directory, there is a small change to allow RPi 400 to be identified correctly.  
As of now RPi 5 does not work with gpios.fs so use the WiringPi version in the meantime.(future work) 

Deprecated as issue now fixed? (kept for future work)
===================================
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
