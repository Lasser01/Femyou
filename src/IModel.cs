using System;
using System.Collections.Generic;

namespace Femyou {
	public interface IModel : IDisposable {
		string Name { get; }
		string Description { get; }

		string FMIVersion { get; }

		double DefaultStartTime { get; }

		double DefaultStopTime { get; }

		double Tolerance { get; }

		string GenerationTool { get; }

		IReadOnlyDictionary<string, IVariable> Variables { get; }

		IInstance CreateCoSimulationInstance(string name, ICallbacks callbacks = null);
	}
}