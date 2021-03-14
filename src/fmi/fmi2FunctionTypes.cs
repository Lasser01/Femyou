using System.Linq;
using System.Runtime.InteropServices;
using size_t = System.UInt64;
/* Type definitions of variables passed as arguments
   Version "default" means:
   fmi2Component           : an opaque object pointer
   fmi2ComponentEnvironment: an opaque object pointer
   fmi2FMUstate            : an opaque object pointer
   fmi2ValueReference      : handle to the value of a variable
   fmi2Real                : double precision floating-point data type
   fmi2Integer             : basic signed integer data type
   fmi2Boolean             : basic signed integer data type
   fmi2Char                : character data type
   fmi2String              : a pointer to a vector of fmi2Char characters
                             ('\0' terminated, UTF8 encoded)
   fmi2Byte                : smallest addressable unit of the machine, typically one byte.
*/
using fmi2Component = System.IntPtr; /* Pointer to FMU instance       */
using fmi2ComponentEnvironment = System.IntPtr; /* Pointer to FMU environment    */
/* Pointer to internal FMU state */
using fmi2ValueReference = System.UInt32;
using fmi2Real = System.Double;
using fmi2Integer = System.Int32;
using fmi2String = System.String;
/* Type definitions */
using fmi2Status = System.Int32;

namespace Femyou.fmi {
	internal static class FMI2 {
		public delegate fmi2Component fmi2CallbackAllocateMemory(ulong nobj, ulong size);

		public delegate void fmi2CallbackFreeMemory(fmi2Component obj);

		public delegate void fmi2CallbackLogger(fmi2ComponentEnvironment componentEnvironment,
			string instanceName,
			int status,
			string category,
			string message);


		/***************************************************
		  Types for Functions for FMI2 for Co-Simulation
		****************************************************/

		/* Simulating the slave */
		public delegate int fmi2DoStepTYPE(fmi2Component c,
			double currentCommunicationPoint,
			double communicationStepSize,
			fmi2Boolean noSetFMUStatePriorToCurrentPoint);

		public delegate int fmi2EnterInitializationModeTYPE(fmi2Component c);

		public delegate int fmi2ExitInitializationModeTYPE(fmi2Component c);

		public delegate void fmi2FreeInstanceTYPE(fmi2Component c);

		public delegate int fmi2GetBooleanTYPE(fmi2Component c, uint[] vr, ulong nvr, fmi2Boolean[] value);

		public delegate int fmi2GetIntegerTYPE(fmi2Component c, uint[] vr, ulong nvr, int[] value);

		/* Getting and setting variable values */
		public delegate int fmi2GetRealTYPE(fmi2Component c, uint[] vr, ulong nvr, double[] value);

		public delegate int fmi2GetStringTYPE(fmi2Component c, uint[] vr, ulong nvr, fmi2Component[] value);

		/* Creation and destruction of FMU instances and setting debug status */
		public delegate fmi2Component fmi2InstantiateTYPE(
			string instanceName,
			fmi2Type fmuType,
			string fmuGUID,
			string fmuResourceLocation,
			fmi2Component functions,
			fmi2Boolean visible,
			fmi2Boolean loggingOn
		);

		public delegate int fmi2ResetTYPE(fmi2Component c);

		public delegate int fmi2SetBooleanTYPE(fmi2Component c, uint[] vr, ulong nvr, fmi2Boolean[] value);

		public delegate int fmi2SetIntegerTYPE(fmi2Component c, uint[] vr, ulong nvr, int[] value);


		public delegate int fmi2SetRealTYPE(fmi2Component c, uint[] vr, ulong nvr, double[] value);

		public delegate int fmi2SetStringTYPE(fmi2Component c, uint[] vr, ulong nvr,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]
			string[] value);

		/* Enter and exit initialization mode, terminate and reset */
		public delegate int fmi2SetupExperimentTYPE(fmi2Component c,
			fmi2Boolean toleranceDefined,
			double tolerance,
			double startTime,
			fmi2Boolean stopTimeDefined,
			double stopTime);

		public delegate void fmi2StepFinished(fmi2ComponentEnvironment componentEnvironment, int status);

		public delegate int fmi2TerminateTYPE(fmi2Component c);

		/* Values for fmi2Boolean  */
		public enum fmi2Boolean {
			fmi2False = 0,
			fmi2True = 1
		}

		/* Type definitions */
		public enum fmi2Type {
			fmi2ModelExchange = 0,
			fmi2CoSimulation = 1
		}

		public struct fmi2CallbackFunctions {
			public fmi2CallbackLogger logger;
			public fmi2CallbackAllocateMemory allocateMemory;
			public fmi2CallbackFreeMemory freeMemory;
			public fmi2StepFinished stepFinished;
			public fmi2ComponentEnvironment componentEnvironment;
		}
	}

	internal static class Marshalling {
		public static fmi2Component[] CreateArray(int size) {
			return Enumerable.Repeat(fmi2Component.Zero, size).ToArray();
		}

		public static string GetString(fmi2Component stringPtr) {
			return Marshal.PtrToStringAnsi(stringPtr);
		}

		public static fmi2Component AllocateMemory(ulong nobj, ulong size) {
			return Marshal.AllocHGlobal((int) (nobj * size));
		}

		public static void FreeMemory(fmi2Component obj) {
			Marshal.FreeHGlobal(obj);
		}
	}
}