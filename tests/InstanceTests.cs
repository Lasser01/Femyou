using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Femyou.Tests {
	public class InstanceTests {
		private static readonly (string, string, Func<IInstance, IEnumerable<IVariable>, IEnumerable<object>>, object,
			Action<IInstance, IEnumerable<IVariable>>, object)[] DefaultValuesTestCases = {
				("Feedthrough.fmu", "bool_in", (i, v) => i.ReadBoolean(v).Cast<object>(), false,
					(i, vs) => i.WriteBoolean(vs.Select(v => (v, true))), true),
				("Feedthrough.fmu", "string_param", (i, v) => i.ReadString(v), "Set me!",
					(i, vs) => i.WriteString(vs.Select(v => (v, "Foo"))), "Foo"),
				("BouncingBall.fmu", "g", (i, v) => i.ReadReal(v).Cast<object>(), -9.81,
					(i, vs) => i.WriteReal(vs.Select(v => (v, 3.46))), 3.46),
				("Stair.fmu", "counter", (i, v) => i.ReadInteger(v).Cast<object>(), 1, (i, vs) => { }, 1),
				("Feedthrough.fmu", "int_in", (i, v) => i.ReadInteger(v).Cast<object>(), 0,
					(i, vs) => i.WriteInteger(vs.Select(v => (v, 28))), 28)
			};


		private readonly FTPoint[] feedthroughScenario = {
			FTPoint.At(0.0, 0, FTData.Is(0.0, 0, 0, false), FTData.Is(1.0, 0, 0, false)),
			FTPoint.At(0.2, 0, FTData.Is(0.0, 0, 0, false), FTData.Is(1.0, 0, 0, false)),
			FTPoint.At(0.4, 0, FTData.Is(0.0, 0, 0, false), FTData.Is(1.0, 0, 0, false)),
			FTPoint.At(0.5, 0, FTData.Is(0.0, 0, 0, false), FTData.Is(1.0, 0, 0, false)),
			FTPoint.At(0.5, 0, FTData.Is(2.0, 0, 0, false), FTData.Is(3.0, 0, 0, false)),
			FTPoint.At(0.6, 0, FTData.Is(1.8, 0, 0, false), FTData.Is(2.8, 0, 0, false)),
			FTPoint.At(0.8, 0, FTData.Is(1.4, 0, 0, false), FTData.Is(2.4, 0, 0, false)),
			FTPoint.At(1.0, 0, FTData.Is(1.0, 0, 0, false), FTData.Is(2.0, 0, 0, false)),
			FTPoint.At(1.0, 0, FTData.Is(1.0, 1, 1, true), FTData.Is(2.0, 1, 1, true)),
			FTPoint.At(1.2, 0, FTData.Is(1.0, 1, 1, true), FTData.Is(2.0, 1, 1, true)),
			FTPoint.At(1.4, 0, FTData.Is(1.0, 1, 1, true), FTData.Is(2.0, 1, 1, true)),
			FTPoint.At(1.5, 0, FTData.Is(1.0, 1, 1, true), FTData.Is(2.0, 1, 1, true)),
			FTPoint.At(1.5, -1, FTData.Is(1.0, 1, 1, true), FTData.Is(1.0, 1, 1, true)),
			FTPoint.At(1.6, -1, FTData.Is(1.0, 1, 1, true), FTData.Is(1.0, 1, 1, true)),
			FTPoint.At(1.8, -1, FTData.Is(1.0, 1, 1, true), FTData.Is(1.0, 1, 1, true)),
			FTPoint.At(2.0, -1, FTData.Is(1.0, 1, 1, true), FTData.Is(1.0, 1, 1, true))
		};

		[TestCaseSource("DefaultValuesTestCases")]
		public void InstanceHasDefaultValues(
			(string filename, string variableName, Func<IInstance, IEnumerable<IVariable>, IEnumerable<object>> reader,
				object expectedDefaultValue, Action<IInstance, IEnumerable<IVariable>> writer, object newValue) t) {
			using IModel model = Model.Load(TestTools.GetFmuPath(t.filename));
			using IInstance instance = Tools.CreateInstance(model, "example");
			var variables = new[] {model.Variables[t.variableName]};
			object actualValue = t.reader(instance, variables).First();
			Assert.That(actualValue, Is.EqualTo(t.expectedDefaultValue));
		}

		[TestCaseSource("DefaultValuesTestCases")]
		public void ChangeInstanceDefaultValues(
			(string filename, string variableName, Func<IInstance, IEnumerable<IVariable>, IEnumerable<object>> reader,
				object expectedDefaultValue, Action<IInstance, IEnumerable<IVariable>> writer, object newValue) t) {
			using IModel model = Model.Load(TestTools.GetFmuPath(t.filename));
			using IInstance instance = Tools.CreateInstance(model, "example");
			var variables = new[] {model.Variables[t.variableName]};
			t.writer(instance, variables);
			object actualValue = t.reader(instance, variables).First();
			Assert.That(actualValue, Is.EqualTo(t.newValue));
		}

		[Test]
		public void ReadMultipleValuesInOneCall() {
			using IModel model = Model.Load(TestTools.GetFmuPath("VanDerPol.fmu"));
			using IInstance instance = Tools.CreateInstance(model, "example");
			IEnumerable<double> actualValues =
				instance.ReadReal(model.Variables["x0"], model.Variables["x1"], model.Variables["mu"]);
			Assert.That(actualValues, Is.EqualTo(new double[] {2, 0, 1}));
		}

		[Test]
		public void
			FeedthroughReferenceScenario() // see https://github.com/modelica/Reference-FMUs/tree/master/Feedthrough
		{
			using IModel model = Model.Load(TestTools.GetFmuPath("Feedthrough.fmu"));
			using IInstance instance = Tools.CreateInstance(model, "reference");
			instance.WriteReal((model.Variables["real_fixed_param"], 1));
			instance.WriteString((model.Variables["string_param"], "FMI is awesome!"));
			IVariable real_tunable_param = model.Variables["real_tunable_param"];
			IVariable real_continuous_in = model.Variables["real_continuous_in"];
			IVariable real_continuous_out = model.Variables["real_continuous_out"];
			IVariable real_discrete_in = model.Variables["real_discrete_in"];
			IVariable real_discrete_out = model.Variables["real_discrete_out"];
			IVariable int_in = model.Variables["int_in"];
			IVariable int_out = model.Variables["int_out"];
			IVariable bool_in = model.Variables["bool_in"];
			IVariable bool_out = model.Variables["bool_out"];
			instance.StartTime(0.0);
			foreach (FTPoint pt in feedthroughScenario) {
				instance.AdvanceTime(pt.time - instance.CurrentTime);
				instance.WriteReal((real_tunable_param, pt.real_tunable_param),
					(real_continuous_in, pt.input.real_continuous), (real_discrete_in, pt.input.real_discrete));
				instance.WriteInteger((int_in, pt.input.integer));
				instance.WriteBoolean((bool_in, pt.input.boolean));
				IEnumerable<double> actual_reals = instance.ReadReal(real_continuous_out, real_discrete_out);
				int actual_int = instance.ReadInteger(int_out);
				bool actual_bool = instance.ReadBoolean(bool_out);
				Assert.That(actual_reals.First(), Is.EqualTo(pt.expectedOutput.real_continuous));
				Assert.That(actual_reals.Last(), Is.EqualTo(pt.expectedOutput.real_discrete));
				Assert.That(actual_int, Is.EqualTo(pt.expectedOutput.integer));
				Assert.That(actual_bool, Is.EqualTo(pt.expectedOutput.boolean));
			}
		}

		private struct FTData {
			public double real_continuous;
			public double real_discrete;
			public int integer;
			public bool boolean;

			public static FTData Is(double real_continuous, double real_discrete, int integer, bool boolean) {
				FTData dt;
				dt.real_continuous = real_continuous;
				dt.real_discrete = real_discrete;
				dt.integer = integer;
				dt.boolean = boolean;
				return dt;
			}
		}

		private struct FTPoint {
			public double time;
			public double real_tunable_param;
			public FTData input;
			public FTData expectedOutput;

			public static FTPoint At(double time, double real_tunable_param, FTData input, FTData expectedOutput) {
				FTPoint pt;
				pt.time = time;
				pt.real_tunable_param = real_tunable_param;
				pt.input = input;
				pt.expectedOutput = expectedOutput;
				return pt;
			}
		}
	}
}