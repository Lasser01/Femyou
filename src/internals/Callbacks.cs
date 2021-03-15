using System;
using System.Runtime.InteropServices;
using Femyou.fmi;

namespace Femyou.internals {
	internal class Callbacks : IDisposable {
		public readonly ICallbacks CallbacksImpl;

		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private readonly FMI2.fmi2CallbackFunctions _functions;
		private readonly Instance _instance;
		public readonly IntPtr Structure;
		private GCHandle _handle;

		public Callbacks(Instance instance, ICallbacks callbacks) {
			_instance = instance;
			CallbacksImpl = callbacks;
			_handle = GCHandle.Alloc(this);
			_functions = new FMI2.fmi2CallbackFunctions {
				logger = LoggerCallback,
				allocateMemory = Marshalling.AllocateMemory,
				freeMemory = Marshalling.FreeMemory,
				stepFinished = StepFinishedCallback,
				componentEnvironment = GCHandle.ToIntPtr(_handle)
			};
			Structure = Marshalling.AllocateMemory(1, (ulong) Marshal.SizeOf(_functions));
			Marshal.StructureToPtr(_functions, Structure, false);
		}


		public void Dispose() {
			Marshalling.FreeMemory(Structure);
			_handle.Free();
		}

		public static void LoggerCallback(IntPtr componentEnvironment, string instanceName, int status, string category,
			string message) {
			var self = (Callbacks) GCHandle.FromIntPtr(componentEnvironment).Target;
			self.CallbacksImpl?.Logger(self._instance, (Status) status, category, message);
		}

		public static void StepFinishedCallback(IntPtr componentEnvironment, int status) { }
	}
}