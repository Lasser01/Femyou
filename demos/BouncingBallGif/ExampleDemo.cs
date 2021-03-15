using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Femyou;
using Femyou.internals;

namespace BouncingBallGif {
	internal static class ExampleDemo {
		internal static void RunExample() {
			string path = Path.Combine(
				Tools.GetBaseFolder(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath,
					nameof(Femyou)), "FMU", "bin", "dist");
			using IModel model = Model.Load($"{path}/Feedthrough.fmu");
			Console.WriteLine("Loaded the model!");
			using IInstance instance = model.CreateCoSimulationInstance("instance1", new ConsoleCallbacks());
			Console.WriteLine("Created an instance of the model!");

			Console.WriteLine("Reading a variable and then changing it's values and reading it again.");
			IVariable exampleString = model.Variables["string_param"];
			Console.WriteLine($"default string_param value = {instance.ReadString(exampleString)}");
			instance.WriteString((exampleString, "10.0 Fellows"));
			Console.WriteLine($"New value for string_param = {instance.ReadString(exampleString)}");
			Console.WriteLine("Reading and writing over.\n\n");


			Console.WriteLine("Printing all input variables and their values.");
			foreach ((string key, IVariable value) in model.Variables.Where(pair =>
				pair.Value.Causality == Causality.Input)) {
				switch (value.VariableType.VariableType) {
					case VariableType.Real:
						Console.WriteLine($"{key} - {value.Description} - {instance.ReadReal(value)}");
						break;
					case VariableType.Integer:
						Console.WriteLine($"{key} - {value.Description} - {instance.ReadInteger(value)}");
						break;
					case VariableType.String:
						Console.WriteLine($"{key} - {value.Description} - {instance.ReadString(value)}");
						break;
					case VariableType.Boolean:
						Console.WriteLine($"{key} - {value.Description} - {instance.ReadBoolean(value)}");
						break;
					case VariableType.Enumeration:
					case VariableType.Unknown:
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			Console.WriteLine("Done printing variables.");
		}

		internal static void DoStepping() {
			string path = Path.Combine(
				Tools.GetBaseFolder(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath,
					nameof(Femyou)), "FMU", "bin", "dist");
			using IModel model = Model.Load($"{path}/BouncingBall.fmu");
			Console.WriteLine("Loaded the model!");
			using IInstance instance = model.CreateCoSimulationInstance("instance1", new ConsoleCallbacks());
			Console.WriteLine("Created an instance of the model!");
			IVariable velocity = model.Variables["v"];
			instance.WriteReal(velocity, 10);

			Console.WriteLine($"v = {instance.ReadReal(velocity)} at time: {instance.CurrentTime}");
			instance.StartTime();
			instance.AdvanceTime(1f);
			Console.WriteLine($"v = {instance.ReadReal(velocity)} at time: {instance.CurrentTime}");
			instance.AdvanceTime(1f);
			Console.WriteLine($"v = {instance.ReadReal(velocity)} at time: {instance.CurrentTime}");
		}

		internal static void DoSimStepping() {
			string path = Path.Combine(
				Tools.GetBaseFolder(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath,
					nameof(Femyou)), "FMU", "bin", "dist");
			using IModel model = Model.Load($"{path}/BouncingBall.fmu");
			Console.WriteLine("Loaded the model!");
			using IInstance instance = model.CreateCoSimulationInstance("instance1", new ConsoleCallbacks());
			Console.WriteLine("Created an instance of the model!");

			IVariable velocity = model.Variables["v"];
			instance.WriteReal(velocity, 10);

			Console.WriteLine($"v = {instance.ReadReal(velocity)} at time: {instance.CurrentTime}");

			instance.StartTime(stopTime: 7.23);
			instance.AdvanceTime(1f);
			Console.WriteLine($"v = {instance.ReadReal(velocity)} at time: {instance.CurrentTime}");
			instance.Simulate(onStep: () =>
				Console.WriteLine($"v = {instance.ReadReal(velocity)} at time: {instance.CurrentTime}"));
			Console.WriteLine($"v = {instance.ReadReal(velocity)} at time: {instance.CurrentTime}");
		}
	}
}