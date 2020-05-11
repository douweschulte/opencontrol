# OpenControl

A command line application to control the backlight and other functions of my 
laptop. The code is reverse engineered based on the control center 3.0 software.
It needs the dll from the original software "InsydeDCHU.dll" which can easily be
copied from the original location. But I will not include this for copyright
reasons.

## Features (highlights)
- Key backlight control:
  - Colour (full RGB)
  - Brightness (256 steps, both more and less bright)
  - Some modes (strobing etc)
- Fan control
  - Power mode
  - Full fan curve control
  
## How does it work
Just run the program with the right command and maybe subcommand. There is help 
available to find all commands and their use.

## How to build
It uses [.NET core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) so a new version of the .NET runtime is needed to compile 
and run the software.

_To run the software_
```
dotnet run help
```

_To compile the software into a stand alone exe_
```
dotnet publish -c release /p:PublishReadyToRun=true
```
