using System;
using System.Collections.Generic;

namespace Femyou {
	public interface IInstance : IDisposable {
		string Name { get; }
		double CurrentTime { get; }
		double ReadReal(IVariable variable);
		int ReadInteger(IVariable variable);
		bool ReadBoolean(IVariable variable);
		string ReadString(IVariable variable);
		void WriteReal((IVariable, double) variable);
		void WriteInteger((IVariable, int) variable);
		void WriteBoolean((IVariable, bool) variable);
		void WriteString((IVariable, string) variable);
		IEnumerable<double> ReadReal(IEnumerable<IVariable> variables);
		IEnumerable<int> ReadInteger(IEnumerable<IVariable> variables);
		IEnumerable<bool> ReadBoolean(IEnumerable<IVariable> variables);
		IEnumerable<string> ReadString(IEnumerable<IVariable> variables);
		void WriteReal(IEnumerable<(IVariable, double)> variables);
		void WriteInteger(IEnumerable<(IVariable, int)> variables);
		void WriteBoolean(IEnumerable<(IVariable, bool)> variables);
		void WriteString(IEnumerable<(IVariable, string)> variables);
		void StartTime(double time);
		void AdvanceTime(double time);
	}

	public static class ExtensionsIInstance {
		public static IEnumerable<double> ReadReal(this IInstance instance, params IVariable[] variables) {
			return instance.ReadReal(variables);
		}

		public static IEnumerable<int> ReadInteger(this IInstance instance, params IVariable[] variables) {
			return instance.ReadInteger(variables);
		}

		public static IEnumerable<bool> ReadBoolean(this IInstance instance, params IVariable[] variables) {
			return instance.ReadBoolean(variables);
		}

		public static IEnumerable<string> ReadString(this IInstance instance, params IVariable[] variables) {
			return instance.ReadString(variables);
		}

		public static void WriteReal(this IInstance instance, params (IVariable, double)[] variables) {
			instance.WriteReal(variables);
		}

		public static void WriteInteger(this IInstance instance, params (IVariable, int)[] variables) {
			instance.WriteInteger(variables);
		}

		public static void WriteBoolean(this IInstance instance, params (IVariable, bool)[] variables) {
			instance.WriteBoolean(variables);
		}

		public static void WriteString(this IInstance instance, params (IVariable, string)[] variables) {
			instance.WriteString(variables);
		}
	}
}