# Building and Using the Reference FMUs

## Prerequisites
1. C compiler (Clang, GCC, MSVC)
2. Cmake 
3. make (The windows version uses MSBuild)

## Cloning and building
1. Make sure to clone the repo, there are two main ways to do this
    1. Use recursive when you clone this repo.
    2. Clone directly from [here](https://github.com/modelica/Reference-FMUs/tree/e4ec2c4776ee4def4f4ca8d0422f18da1146275f) into this folder.
2. Run the build-reference-FMUs.sh (Or build-reference-FMUs.cmd if on windows).
3. If successful you should find your models in the bin/dist folder.


## Troubleshooting
Here are some mistakes I've made, maybe they can help you:

### Unhandled exception. System.IO.FileNotFoundException: Could not find or load the native library: BouncingBall.fmu\binaries\win64\BouncingBall.dll
This most likely because you build it for the wrong architecture, make sure that you build for your platform.

### Couldn't load MOS files
Make sure they are in your working directory (in this case it would be demos/BouncingBallGif/bin/Debug/net5.0/)
