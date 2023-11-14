\ gpios.fs	GPIO access
\
\ Authors: Bernd Paysan
\ Copyright (C) 2021,2022 Free Software Foundation, Inc.

\ This file is part of Gforth.

\ Gforth is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation, either version 3
\ of the License, or (at your option) any later version.

\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

\ You should have received a copy of the GNU General Public License
\ along with this program. If not, see http://www.gnu.org/licenses/.

require unix/libc.fs
require unix/mmap.fs

0 Value gpio-base

: reg: ( offset -- )
    Create sfloats ,
  DOES> @ gpio-base + ;

\ register access

: lor! ( l addr -- )
    dup >r l@ or r> l! ;
: land! ( l addr -- )
    dup >r l@ and r> l! ;
: lmask! ( l mask addr -- )
    \G set the bits where @var{mask} is 1
    \G in @var{addr} by the corresponding
    \G bits in @var{l}.
    dup >r l@ swap mux r> l! ;

\ mask generation

1 Constant 1bit ( addr gpio -- addr gpio mask )
: 2bit ( addr gpio -- addr' gpio' mask )
    2* tuck 5 rshift sfloats + swap $1F and 3 ;
: 3bit ( addr gpio -- addr' gpio' mask )
    #10 /mod swap >r sfloats + r> dup 2* + 7 ;
: 4bit ( addr gpio -- addr' gpio' mask )
    2* 2* tuck 5 rshift sfloats + swap $1F and $F ;

\ device specific stuff

s" /sys/firmware/devicetree/base/model" ' slurp-file catch
[IF] 2drop s" unknown" [THEN] 2Constant model
: model?" ( "name"<"> -- flag )
    model '"' parse search nip nip ;

model?" ODROID-N2" [IF]
    model?" ODROID-N2Plus" [IF]
	: odroid-n2+ ;
    [ELSE]
	: odroid-n2 ;
    [THEN]
    $FF634000 Constant GPIO-Base-map

    \ actual offset is $FF634400, i.e. starts at reg $100

    \ mux is 0 for IO, 1..F for other functions
    \ FSEL bit is 0 for Output, 1 for Input
    \ pins have pullups/downs, which can be enabled
    
    $116 reg: N2_GPIOX_FSEL_REG
    $117 reg: N2_GPIOX_OUTP_REG
    $118 reg: N2_GPIOX_INP_REG

    $120 reg: N2_GPIOA_FSEL_REG
    $121 reg: N2_GPIOA_OUTP_REG
    $122 reg: N2_GPIOA_INP_REG

    $13C reg: N2_GPIOX_PUPD_REG
    $13F reg: N2_GPIOA_PUPD_REG

    $14A reg: N2_GPIOX_PUEN_REG
    $14D reg: N2_GPIOA_PUEN_REG

    $1B3 reg: N2_GPIOX_MUX_3_REG \ GPIOX[0:7]
    $1B4 reg: N2_GPIOX_MUX_4_REG \ GPIOX[8:15]
    $1B5 reg: N2_GPIOX_MUX_5_REG \ GPIOX[16:19]
    
    $1BD reg: N2_GPIOA_MUX_D_REG \ GPIOA[0:7]
    $1BE reg: N2_GPIOA_MUX_E_REG \ GPIOA[8:15]

    $1D2 reg: N2_GPIOX_DS_REG_2A \ GPIOX[0:15]
    $1D3 reg: N2_GPIOX_DS_REG_2B \ GPIOX[16:19]
    $1D6 reg: N2_GPIOA_DS_REG_5A

    Create shift/type
    ' 1bit , ' 1bit , ' 1bit , ' 1bit , ' 1bit , ' 2bit , ' 4bit ,

    Variable gpio-dummy
    
    : gpio>mask ( gpio type table -- shift mask addr )
	third -1 = IF  2drop drop 0 0 gpio-dummy gpio-base -  EXIT  THEN
	swap { s/t }
	s/t 2* cells + over 5 rshift cells + @
	swap $1F and shift/type s/t cells + perform over lshift rot ;

    -1
    1+ dup Constant fsel#
    1+ dup Constant outp#
    1+ dup Constant inp#
    1+ dup Constant pupd#
    1+ dup Constant puen#
    1+ dup Constant ds#
    1+ dup Constant mux#
    drop
    
    Create gpio-reg[]
    N2_GPIOX_FSEL_REG  , N2_GPIOA_FSEL_REG  ,
    N2_GPIOX_OUTP_REG  , N2_GPIOA_OUTP_REG  ,
    N2_GPIOX_INP_REG   , N2_GPIOA_INP_REG   ,
    N2_GPIOX_PUPD_REG  , N2_GPIOA_PUPD_REG  ,
    N2_GPIOX_PUEN_REG  , N2_GPIOA_PUEN_REG  ,
    N2_GPIOX_DS_REG_2A , N2_GPIOA_DS_REG_5A ,
    N2_GPIOX_MUX_3_REG , N2_GPIOA_MUX_D_REG ,
      DOES> ( pin type -- shift mask addr )
	gpio>mask gpio-base + ;
    [: lits# 2 u>= IF  2lits> rot >body gpio>mask >3lits ]] gpio-base + [[
	ELSE  does,  THEN ;] optimizes gpio-reg[]
    
    \ pins to GPIO table: X=$000+, A=$020+
    Create pin>gpio ( pin -- gpio )
    -1   , -1   ,
    $011 , -1   ,
    $012 , -1   ,
    $02D , $00C ,
    -1   , $00D ,
    $003 , $010 ,
    $004 , -1   ,
    $007 , $000 ,
    -1   , $001 ,
    $008 , -1   ,
    $009 , $002 ,
    $00B , $00A ,
    -1   , $024 ,
    $02E , $02F ,
    $00E , -1   ,
    $00F , $02C ,
    $005 , -1   ,
    $006 , $013 ,
    -1   , -1   ,
    -1   , -1   ,
      DOES> swap 1- #39 umin cells + @ ;
    [: lits# 1 u>= IF  >body lits> 1- #39 umin cells + @ >lits
	ELSE  does,  THEN ;] optimizes pin>gpio
[THEN]
model?" ODROID-C2" [IF]
    : odroid-c2 ;
    $C8834000 Constant GPIO-Base-map

    $118 reg: C2_GPIOX_FSEL_REG
    $119 reg: C2_GPIOX_OUTP_REG
    $11A reg: C2_GPIOX_INP_REG
    $13E reg: C2_GPIOX_PUPD_REG
    $14C reg: C2_GPIOX_PUEN_REG
    
    $10F reg: C2_GPIOY_FSEL_REG
    $110 reg: C2_GPIOY_OUTP_REG
    $111 reg: C2_GPIOY_INP_REG
    $13B reg: C2_GPIOY_PUPD_REG
    $149 reg: C2_GPIOY_PUEN_REG
    
\    $10C reg: C2_GPIODV_FSEL_REG
\    $10D reg: C2_GPIODV_OUTP_REG
\    $10E reg: C2_GPIODV_INP_REG
\    $148 reg: C2_GPIODV_PUPD_REG
\    $13A reg: C2_GPIODV_PUEN_REG
    
    $12C reg: C2_MUX_REG_0
    $12D reg: C2_MUX_REG_1
    $12E reg: C2_MUX_REG_2
    $12F reg: C2_MUX_REG_3
    $130 reg: C2_MUX_REG_4
    $131 reg: C2_MUX_REG_5
    $133 reg: C2_MUX_REG_7
    $134 reg: C2_MUX_REG_8
 
    Create shift/type
    ' 1bit , ' 1bit , ' 1bit , ' 1bit , ' 1bit , ' 2bit , ' 4bit ,

    Variable gpio-dummy
    
    : gpio>mask ( gpio type table -- shift mask addr )
	third -1 = IF  2drop drop 0 0 gpio-dummy gpio-base -  EXIT  THEN
	swap { s/t }
	s/t 2* cells + over 5 rshift cells + @
	swap $1F and shift/type s/t cells + perform over lshift rot ;

    -1
    1+ dup Constant fsel#
    1+ dup Constant outp#
    1+ dup Constant inp#
    1+ dup Constant pupd#
    1+ dup Constant puen#
    1+ dup Constant mux#
    drop
    
    Create gpio-reg[]
    C2_GPIOX_FSEL_REG  , C2_GPIOY_FSEL_REG  ,
    C2_GPIOX_OUTP_REG  , C2_GPIOY_OUTP_REG  ,
    C2_GPIOX_INP_REG   , C2_GPIOY_INP_REG   ,
    C2_GPIOX_PUPD_REG  , C2_GPIOY_PUPD_REG  ,
    C2_GPIOX_PUEN_REG  , C2_GPIOY_PUEN_REG  ,
    C2_MUX_REG_0       , C2_MUX_REG_4       ,
      DOES> ( gpio type -- shift mask addr )
	gpio>mask gpio-base + ;
    [: lits# 2 u>= IF  2lits> rot >body gpio>mask >3lits ]] gpio-base + [[
	ELSE  does,  THEN ;] optimizes gpio-reg[]
    
    \ pins to GPIO table: X=$000+, Y=$020+
    Create pin>gpio ( pin -- gpio )
    -1   , -1   ,
    -1   , -1   ,
    -1   , -1   ,
    $015 , -1   ,
    -1   , -1   ,
    $013 , $00A ,
    $00B , -1   ,
    $009 , $008 ,
    -1   , $005 ,
    $007 , -1   ,
    $004 , $003 ,
    $002 , $001 ,
    -1   , $02E ,
    -1   , -1   ,
    $000 , -1   ,
    $028 , $02D ,
    $006 , -1   ,
    $023 , $027 ,
    -1   , -1   ,
    -1   , -1   ,
    DOES> swap 1- #39 umin cells + @ ;
    [: lits# 1 u>= IF  >body lits> 1- #39 umin cells + @ >lits
	ELSE  does,  THEN ;] optimizes pin>gpio
[THEN]
model?" Raspberry Pi" [IF]
    $00200000 Constant GPIO-Base-map

    \ fsel are 3 bits per function, 000 is input, 001 is output

    $000 reg: RPI_GPFSEL0
    $001 reg: RPI_GPFSEL1
    $002 reg: RPI_GPFSEL2
    $003 reg: RPI_GPFSEL3
    $004 reg: RPI_GPFSEL4
    $005 reg: RPI_GPFSEL5

    $007 reg: RPI_GPSET0
    $008 reg: RPI_GPSET1

    $00A reg: RPI_GPCLR0
    $00B reg: RPI_GPCLR1

    $00D reg: RPI_GPLEV0
    $00E reg: RPI_GPLEV1

    $010 reg: RPI_GPEDS0
    $011 reg: RPI_GPEDS1

    $013 reg: RPI_GPREN0
    $014 reg: RPI_GPREN1

    $016 reg: RPI_GPFEN0
    $017 reg: RPI_GPFEN1

    $019 reg: RPI_GPHEN0
    $01A reg: RPI_GPHEN1

    $01C reg: RPI_GPLEN0
    $01D reg: RPI_GPLEN1

    $01F reg: RPI_GPAREN0
    $020 reg: RPI_GPAREN1

    $022 reg: RPI_GPAFEN0
    $023 reg: RPI_GPAFEN1

    \ BCM2835 variant
    $025 reg: RPI_GPPUD
    $026 reg: RPI_GPPUDCLK0
    $027 reg: RPI_GPPUDCLK1
    
    \ BCM2711 variant
    $039 reg: RPI_GPIO_PUP_PDN_CNTRL_REG0
    $03A reg: RPI_GPIO_PUP_PDN_CNTRL_REG1
    $03B reg: RPI_GPIO_PUP_PDN_CNTRL_REG2
    $03C reg: RPI_GPIO_PUP_PDN_CNTRL_REG3
    
    -1
    1+ dup Constant fsel#
    1+ dup Constant set#
    1+ dup Constant clr#
    1+ dup Constant inp#
    model s" Raspberry Pi 4" search nip nip [IF]
	1+ dup Constant pudmode#
	drop
	
	Create shift/type
	' 3bit , ' 1bit , ' 1bit , ' 1bit , ' 2bit ,
	
	: rpi-4 ;
    [ELSE]
	1+ dup Constant puclk#
	drop
	
	Create shift/type
	' 3bit , ' 1bit , ' 1bit , ' 1bit , ' 1bit ,

	: rpi ; \ model 0-3
    [THEN]
 
    Variable gpio-dummy
    
    : gpio>mask ( gpio type table -- shift mask addr )
	third -1 = IF  2drop drop 0 0 gpio-dummy gpio-base -  EXIT  THEN
	swap { s/t }
	s/t cells + over 5 rshift cells + @
	swap $1F and shift/type s/t cells + perform over lshift rot ;

    Create gpio-reg[]
    RPI_GPFSEL0   ,
    RPI_GPSET0    ,
    RPI_GPCLR0    ,
    RPI_GPLEV0    ,
    model s" Raspberry Pi 4" search nip nip [IF]
	RPI_GPIO_PUP_PDN_CNTRL_REG0 ,
    [ELSE]
	RPI_GPPUDCLK0 ,
    [THEN]
      DOES> ( gpio type -- shift mask addr )
	gpio>mask gpio-base + ;
    [: lits# 2 u>= IF  2lits> rot >body gpio>mask >3lits ]] gpio-base + [[
	ELSE  does,  THEN ;] optimizes gpio-reg[]
   
    \ pins to GPIO table:
    Create pin>gpio ( pin -- gpio )
    -1   , -1   ,
    $002 , -1   ,
    $003 , -1   ,
    $004 , $00E ,
    -1   , $00F ,
    $011 , $012 ,
    $01B , -1   ,
    $016 , $017 ,
    -1   , $018 ,
    $00A , -1   ,
    $009 , $019 ,
    $00B , $008 ,
    -1   , $007 ,
    $000 , $001 ,
    $005 , -1   ,
    $006 , $00C ,
    $00D , -1   ,
    $013 , $010 ,
    $01A , $014 ,
    -1   , $015 ,
    DOES> swap 1- #39 umin cells + @ ;
    [: lits# 1 u>= IF  >body lits> 1- #39 umin cells + @ >lits
	ELSE  does,  THEN ;] optimizes pin>gpio
[THEN]

\ optimizing helpers

: lmask!, ( lits:n mode -- ) >r
    lits> pin>gpio r> gpio-reg[] rot >lits ]] lshift [[
    gpio-base - >2lits ]] gpio-base + lmask! [[ ;
: l@, ( lits:n mode -- )  >r
    lits> pin>gpio r> gpio-reg[]
    gpio-base - >2lits ]] gpio-base + l@ and [[
    >lits ]] rshift [[ ;
: l!, ( lits:n mode -- )  >r
    lits> pin>gpio r> gpio-reg[] rot drop
    gpio-base - >2lits ]] gpio-base + l! [[ ;
[IFDEF] fsel#
    : fsel! ( val n -- ) pin>gpio fsel# gpio-reg[] 2>r lshift 2r> lmask! ;
    opt: lits# 1 u>= IF  drop  fsel# lmask!,  ELSE  :,  THEN ;
    : fsel@ ( n -- val ) pin>gpio fsel# gpio-reg[] l@ and swap rshift ;
    opt: lits# 1 u>= IF  drop  fsel# l@,  ELSE  :,  THEN ;
[THEN]
: pin@ ( pin -- val )
    \G get input level of pin
    pin>gpio inp# gpio-reg[] l@ and swap rshift ;
opt: lits# 1 u>= IF  drop inp# l@,  ELSE  :,  THEN ;
[IFDEF] set#
    : pinset ( n -- ) pin>gpio set# gpio-reg[] l! drop ;
    opt: lits# 1 u>= IF  drop set# l!,  ELSE  :,  THEN ;
[THEN]
[IFDEF] clr#
    : pinclr ( n -- ) pin>gpio clr# gpio-reg[] l! drop ;
    opt: lits# 1 u>= IF  drop clr# l!,  ELSE  :,  THEN ;
[THEN]
: pin! ( val pin -- )
    \G set output level of pin to @var{val}
    [IFDEF] outp#
	pin>gpio outp# gpio-reg[] 2>r lshift 2r> lmask! ;
    opt: lits# 1 u>= IF  drop outp# lmask!,  ELSE  :,  THEN
    [ELSE] [IFDEF] pinset
	    swap 1 and IF  pinset  ELSE  pinclr  THEN ;
	opt: lits# 2 u>= IF  drop
		lits> lits> swap >lits
		1 and IF  ]] pinset [[  ELSE  ]] pinclr [[  THEN
	    ELSE
		lits# 1 u>= IF  drop
		    lits> >r
		    ]] 1 and IF [[ r@ >lits ]] pinset  ELSE [[
			r> >lits  ]] pinclr  THEN [[
		ELSE
		    :,
		THEN
	    THEN
    [THEN] [THEN] ;
[IFUNDEF] pinset
    : pinset ( pin -- )
	\G set pin to high
	1 swap pin! ;
    opt: drop ]] 1 swap pin! [[ ;
[THEN]
[IFUNDEF] pinclr
    : pinclr ( pin -- )
	\G set pin to low
	0 swap pin! ;
    opt: drop ]] 0 swap pin! [[ ;
[THEN]
[IFDEF] mux#
    : mux! ( val pin -- ) pin>gpio mux# gpio-reg[] 2>r lshift 2r> lmask! ;
    opt: lits# 1 u>= IF  drop  mux# lmask!,  ELSE  :,  THEN ;
    : mux@ ( pin -- val ) pin>gpio mux# gpio-reg[] l@ and swap rshift ;
    opt: lits# 1 u>= IF  drop  mux# l@,  ELSE  :,  THEN ;
[THEN]
: input-pin ( pin -- )
    \G set pin mode to input
    [IFDEF] mux!
	0 over mux!  1 swap fsel! ;
    opt: drop lits# 1 u>= IF
	    lits> dup ]] 0 Literal mux!  1 Literal fsel! [[
	ELSE  ]] 0 over mux!  1 swap fsel! [[  THEN
    [ELSE]
	0 swap fsel! ;
    opt: drop ]] 0 swap fsel! [[
    [THEN] ;
: output-pin ( pin -- )
    \G set pin mode to output
    [IFDEF] mux!
	0 over mux!  0 swap fsel! ;
    opt: drop lits# 1 u>= IF
	    lits> dup ]] 0 Literal mux!  0 Literal fsel! [[
	ELSE  ]] 0 over mux!  0 swap fsel! [[  THEN
    [ELSE]
	1 swap fsel! ;
    opt: drop ]] 1 swap fsel! [[
    [THEN] ;
[IFDEF] puen#
    : puen! ( val pin -- ) pin>gpio puen# gpio-reg[] 2>r lshift 2r> lmask! ;
    opt: lits# 1 u>= IF  drop  puen# lmask!,  ELSE  :,  THEN ;
    : puen@ ( pin -- val ) pin>gpio puen# gpio-reg[] l@ and swap rshift ;
    opt: lits# 1 u>= IF  drop  puen# l@,  ELSE  :,  THEN ;
[THEN]
[IFDEF] pupd#
    : pupd! ( val pin -- ) pin>gpio pupd# gpio-reg[] 2>r lshift 2r> lmask! ;
    opt: lits# 1 u>= IF  drop  pupd# lmask!,  ELSE  :,  THEN ;
    : pupd@ ( pin -- val ) pin>gpio pupd# gpio-reg[] l@ and swap rshift ;
    opt: lits# 1 u>= IF  drop  pupd# l@,  ELSE  :,  THEN ;
[THEN]
[IFDEF] puclk#
    : puclk! ( val pin -- ) pin>gpio puclk# gpio-reg[] 2>r lshift 2r> lmask! ;
    opt: lits# 1 u>= IF  drop  puclk# lmask!,  ELSE  :,  THEN ;
    : 150cyc  25 0 DO  LOOP ; \ assumes 5 cycles per iteration
    : 0<>1 ( pin -- n' ) dup 1 rshift swap 1 lshift or 3 and ;
[THEN]
: pin-resmode ( mode pin -- )
    \G set pullup/down mode of pin @var{n} to:
    \G @enumerate 0
    \G @item none
    \G @item pullup
    \G @item pulldown
    \G @end enumerate
    [IFDEF] pupd#
	>r case
	    0 of  0 r> puen!  endof
	    1 of  1 r@ puen!  1 r> pupd!  endof
	    2 of  1 r@ puen!  0 r> pupd!  endof
	    rdrop  endcase ;
    opt: lits# 2 u>= IF  drop
	    2lits> >r  case
		0 of  0 r> >2lits ]] puen! [[  endof
		1 of  1 r@ >2lits ]] puen! [[  1 r> >2lits ]] pupd! [[  endof
		2 of  1 r@ >2lits ]] puen! [[  0 r> >2lits ]] pupd! [[  endof
		rdrop  endcase
	ELSE  :,  THEN
    [THEN]
    [IFDEF] puclk#
	>r 0<>1 RPI_GPPUD l!
	150cyc  1 r@ puclk!  150cyc  0 RPI_GPPUD l!  0 r> puclk! ;
    opt: lits# 1 u>= IF  drop lits> >r ]] 0<>1 RPI_GPPUD l!  150cyc  1 [[
	    r@ >lits ]] puclk!  150cyc  0 [[
	    r> >lits ]] puclk!  0 RPI_GPPUD l! [[
	ELSE  :,  THEN
    [THEN]
[IFDEF] pudmode#
	pin>gpio pudmode# gpio-reg[] 2>r lshift 2r> lmask! ;
    opt: lits# 1 u>= IF  drop  pudmode# lmask!,  ELSE  :,  THEN
[THEN]
;
[IFDEF] pudmode#
    : pin-resmode@ ( pin -- val )
	pin>gpio pudmode# gpio-reg[] l@ and swap rshift ;
    opt: lits# 1 u>= IF  drop  pudmode# l@,  ELSE  :,  THEN ;
[THEN]
    
: map-gpio ( -- )
    s" /dev/gpiomem" r/w open-file throw dup >r fileno >r
    0 $1000 PROT_READ PROT_WRITE or MAP_SHARED r> GPIO-Base-map mmap
    r> close-file throw dup 0= ?ior to gpio-base ;

map-gpio

: pin-show { mode -- }
    41 1 DO
	I pin>gpio dup -1 = IF drop ." -"
	ELSE  mode gpio-reg[] l@ and swap rshift 0 .r  THEN
    LOOP ;
: .pin#s ( -- ) cr
    ." pin " 41 1 DO  I 10 / 0 .r  LOOP cr
    ." pin " 41 1 DO  I 10 mod 0 .r  LOOP ;
: .pins ( -- ) .pin#s cr
    [IFDEF] fsel# ." fsel" fsel# pin-show cr [THEN]
    [IFDEF] inp#  ." inp " inp#  pin-show cr [THEN]
    [IFDEF] outp# ." outp" outp# pin-show cr [THEN]
    [IFDEF] puen# ." puen" puen# pin-show cr [THEN]
    [IFDEF] pupd# ." pupd" pupd# pin-show cr [THEN]
    [IFDEF] pudmode# ." pud " pudmode# pin-show cr [THEN]
    [IFDEF] mux#  ." mux " mux#  pin-show cr [THEN] ;
: inps@ ( -- u ) 0 41 1 DO  I pin@ I lshift or  LOOP ;

: pin-connect? ( pin -- )
    dup pin>gpio -1 = IF  ." -/-"  drop  EXIT  THEN
    dup >r output-pin
    r@ pinclr  inps@ invert
    r@ pinset  inps@ and
    dup 2/ #40 ['] .r 2 base-execute space
    41 1 DO
	dup 1 I lshift and  IF  I .  THEN
    LOOP  drop
    r> input-pin ;

: .pin-matrix ( -- )
    41 1 DO cr I pin-connect? LOOP cr ;
