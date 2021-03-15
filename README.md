# Femyou
loading and running [fmi-standard](https://fmi-standard.org/) FMUs in dotnet core

![CI](https://github.com/Oaz/Femyou/workflows/CI/badge.svg)

[The binary package is available from NuGet](https://www.nuget.org/packages/Femyou/)

## Features

This is a limited implementation of the fmi-standard.

Available features :
* Load FMU model
* Get model variables
* Create co-simulation instance
* Read and write variable values from instance
* Simulate by advancing time

The features are verified with the [reference FMUs](https://github.com/modelica/Reference-FMUs).

## Example

```C#
using var model = Model.Load("BouncingBall.fmu");
var height = model.Variables["h"];
var velocity = model.Variables["v"];
double h = 60.0, v = 0.0;
using var instance = model.CreateCoSimulationInstance("demo");
instance.WriteReal((height, h), (velocity, v));
instance.StartTime(0.0);
while (h > 0 || Math.Abs(v) > 0)
{
  var values = instance.ReadReal(height, velocity).ToArray();
  h = values[0];
  v = values[1];
  instance.AdvanceTime(0.1);
}
```

## Demos

The [Bouncing Ball](https://github.com/modelica/Reference-FMUs/tree/master/BouncingBall) reference FMU visualized with GIF animation.

![BouncingBall](BouncingBall.gif?raw=true)


## Troubleshooting
Here are some mistakes I've made, maybe they can help you:

### Unhandled exception. System.IO.FileNotFoundException: Could not find or load the native library: BouncingBall.fmu\binaries\win64\BouncingBall.dll
This most likely because you build it for the wrong architecture, make sure that you build for your platform.

### Couldn't load MOS files
Make sure they are in your working directory (in this case it would be demos/BouncingBallGif/bin/Debug/net5.0/)
