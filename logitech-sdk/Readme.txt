Logitech Gaming G-key SDK
Copyright (C) 2014 Logitech Inc. All Rights Reserved


Introduction
--------------------------------------------------------------------------

This package enables developers to easily add support in their games
for checking G-key and extra mouse button presses on compatible Logitech 
gaming mice and keyboards.


Contents of the package
--------------------------------------------------------------------------

- Logitech Gaming G-key SDK header, libs 32 and 64 bit
- Logitech Gaming G-key SDK game engines wrapper dll
- Demo executable
- Documentation
- Sample program using the SDK


The environment for use of the package
--------------------------------------------------------------------------

Visual Studio 2012 to build and run the sample program


List of currently supported devices
--------------------------------------------------------------------------

Mice

- G300
- G400 / G400s
- G600
- G700 / G700s
- 

Keyboards

- G11
- G13
- G15 v1
- G15 v2
- G103
- G105
- G105 Call Of Duty
- G110
- G510 / G510s
- G19 / G19s
- G710

Headsets:

- G35
- G930


Disclaimer
--------------------------------------------------------------------------

This is work in progress. If you find anything wrong with either
documentation or code, please let us know so we can improve on it.


Where to start
--------------------------------------------------------------------------

For a demo program to change lighting on devices:

Execute Demo/DisplayGkeys.exe.

Or:

1. Go to Samples/DisplayGkeys folder and open the project in
   Visual Studio.

2. Compile and run.

3. Plug in one or multiple compatible mice or keyboards at any time.


To implement game controller support in your game:

1. Include the following header file in your game:

- Include/LogitechGkeyLib.h

2. Include the following libraryin your game:

- Lib\x86\LogitechGkeyLib.lib
- Lib\x64\LogitechGkeyLib.lib


3. Read and follow instructions from Doc/LogitechGamingGkeySDK.pdf


For questions/problems/suggestions email to:
tpigliucci@logitech.com
