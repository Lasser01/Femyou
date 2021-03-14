using System;
using System.Collections.Generic;
using System.Linq;
using Femyou.fmi;

namespace Femyou.internals {
	internal class Instance : IInstance {
		private readonly Callbacks _callbacks;
		private readonly IntPtr _handle;
		private readonly Library _library;
		private readonly IModel _model;
		private bool _started;

		public Instance(string name, ModelImpl model, Library library, FMI2.fmi2Type instanceType, ICallbacks cb) {
			Name = name;
			_library = library;
			_model = model;
			_callbacks = new Callbacks(this, cb);
			CurrentTime = model.DefaultStartTime;
			_handle = library.fmi2Instantiate(
				name,
				instanceType,
				model.GUID,
				model.TmpFolder,
				_callbacks.Structure,
				FMI2.fmi2Boolean.fmi2False,
				FMI2.fmi2Boolean.fmi2False
			);
			if (_handle == IntPtr.Zero)
				throw new Exception("Cannot instanciate model");
		}

		public string Name { get; }
		public double CurrentTime { get; private set; }

		public double ReadReal(IVariable variable) {
			return Read(variable, new double(), (a, b, c, d) => _library.fmi2GetReal(a, b, c, d));
		}

		public int ReadInteger(IVariable variable) {
			return Read(variable, new int(), (a, b, c, d) => _library.fmi2GetInteger(a, b, c, d));
		}

		public bool ReadBoolean(IVariable variable) {
			return Read(variable, new FMI2.fmi2Boolean(), (a, b, c, d) => _library.fmi2GetBoolean(a, b, c, d)) ==
			       FMI2.fmi2Boolean.fmi2True;
		}
		
		public string ReadString(IVariable variable) {
			return Marshalling.GetString(Read(variable, new IntPtr(),
				(a, b, c, d) => _library.fmi2GetString(a, b, c, d)));
		}

		public void WriteReal((IVariable, double) variable) {
			Write(variable, (a, b, c, d) => _library.fmi2SetReal(a, b, c, d));
		}

		public void WriteInteger((IVariable, int) variable) {
			Write(variable, (a, b, c, d) => _library.fmi2SetInteger(a, b, c, d));
		}

		public void WriteBoolean((IVariable, bool) variable) {
			Write((variable.Item1, variable.Item2 ? FMI2.fmi2Boolean.fmi2True : FMI2.fmi2Boolean.fmi2False),
				(a, b, c, d) => _library.fmi2SetBoolean(a, b, c, d));
		}

		public void WriteString((IVariable, string) variable) {
			Write(variable, (a, b, c, d) => _library.fmi2SetString(a, b, c, d));
		}
		
		public void StartTime(double time) {
			CurrentTime = time;
			_library.fmi2SetupExperiment(_handle, FMI2.fmi2Boolean.fmi2True, _model.Tolerance, _model.DefaultStartTime,
				FMI2.fmi2Boolean.fmi2True, _model.DefaultStopTime);
			_library.fmi2EnterInitializationMode(_handle);
			_library.fmi2ExitInitializationMode(_handle);
			_started = true;
		}

		public void AdvanceTime(double step) {
			if (step == 0.0)
				return;
			_library.fmi2DoStep(_handle, CurrentTime, step, FMI2.fmi2Boolean.fmi2True);
			CurrentTime += step;
		}

		public IEnumerable<double> ReadReal(IEnumerable<IVariable> variables) {
			return Read(
				variables,
				new double[variables.Count()],
				(a, b, c, d) => _library.fmi2GetReal(a, b, c, d)
			);
		}

		public IEnumerable<int> ReadInteger(IEnumerable<IVariable> variables) {
			return Read(
				variables,
				new int[variables.Count()],
				(a, b, c, d) => _library.fmi2GetInteger(a, b, c, d)
			);
		}

		public IEnumerable<bool> ReadBoolean(IEnumerable<IVariable> variables) {
			return Read(
				variables,
				new FMI2.fmi2Boolean[variables.Count()],
				(a, b, c, d) => _library.fmi2GetBoolean(a, b, c, d)
			).Select(r => r == FMI2.fmi2Boolean.fmi2True);
		}

		public IEnumerable<string> ReadString(IEnumerable<IVariable> variables) {
			return Read(
				variables,
				Marshalling.CreateArray(variables.Count()),
				(a, b, c, d) => _library.fmi2GetString(a, b, c, d)
			).Select(r => Marshalling.GetString(r));
		}

		public void WriteReal(IEnumerable<(IVariable, double)> variables) {
			Write(
				variables,
				(a, b, c, d) => _library.fmi2SetReal(a, b, c, d)
			);
		}

		public void WriteInteger(IEnumerable<(IVariable, int)> variables) {
			Write(
				variables,
				(a, b, c, d) => _library.fmi2SetInteger(a, b, c, d)
			);
		}

		public void WriteBoolean(IEnumerable<(IVariable, bool)> variables) {
			Write(
				variables.Select(v => (v.Item1, v.Item2 ? FMI2.fmi2Boolean.fmi2True : FMI2.fmi2Boolean.fmi2False)),
				(a, b, c, d) => _library.fmi2SetBoolean(a, b, c, d)
			);
		}

		public void WriteString(IEnumerable<(IVariable, string)> variables) {
			Write(
				variables,
				(a, b, c, d) => _library.fmi2SetString(a, b, c, d)
			);
		}

		public void Dispose() {
			if (_started)
				_library.fmi2Terminate(_handle);
			_library.fmi2FreeInstance(_handle);
			_callbacks.Dispose();
		}

		private T[] Read<T>(IEnumerable<IVariable> variables, T[] values, Func<IntPtr, uint[], ulong, T[], int> call) {
			uint[] valueReferences = variables.Cast<Variable>().Select(variables => variables.ValueReference).ToArray();
			int status = call(_handle, valueReferences, (ulong) valueReferences.Length, values);
			if (status != 0)
				throw new Exception("Failed to read");
			return values;
		}

		private T Read<T>(IVariable variable, T value, Func<IntPtr, uint[], ulong, T[], int> call) {
			T[] values = {value};
			int status = call(_handle, new[] {variable.ValueReference}, 1, values);
			if (status != 0)
				throw new Exception("Failed to read");
			return values[0];
		}

		private void Write<T>(IEnumerable<(IVariable, T)> variables, Func<IntPtr, uint[], ulong, T[], int> call) {
			uint[] valueReferences = variables.Select(variables => variables.Item1).Cast<Variable>()
				.Select(variables => variables.ValueReference).ToArray();
			T[] values = variables.Select(variables => variables.Item2).ToArray();
			int status = call(_handle, valueReferences, (ulong) valueReferences.Length, values);
			if (status != 0)
				throw new Exception("Failed to write");
		}

		private void Write<T>((IVariable, T) variableTuple, Func<IntPtr, uint[], ulong, T[], int> call) {
			(IVariable variable, T value) = variableTuple;
			T[] values = {value};
			int status = call(_handle, new[] {variable.ValueReference}, 1, values);
			if (status != 0)
				throw new Exception("Failed to read");
		}
	}
}