using System;

namespace BouncingBallGif {
	public class Program {
		/// <summary>
		/// Runs simple examples of how to use Femyou
		/// </summary>
		/// <remarks>You need to build the models in the FMU folder to run these.
		/// Furthermore they need to be for your OS e.g. if you are on linux they should be linux fmu's and same for windows.
		/// See Readme in FMU folder for more infomation.
		/// </remarks>
		/// <param name="args"></param>
		public static void Main(string[] args) {
			Console.WriteLine("Creating a gif");
			GIFDemo.CreateGIF();
			Console.WriteLine("Created the gif\n\n\n\n");
			Console.WriteLine("Running Simple Example");
			ExampleDemo.RunExample();
			Console.WriteLine("Simple Example over\n\n\n\n");
			Console.WriteLine("Simple Stepping Example");
			ExampleDemo.DoStepping();
			Console.WriteLine("Simple Stepping over\n\n\n\n");
		}
	}
}