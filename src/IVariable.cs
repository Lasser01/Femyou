using Femyou.internals;

namespace Femyou {
	public interface IVariable {
		string Name { get; }
		
		string Description { get; }

		public uint ValueReference { get; }

		IVariableType VariableType { get; }

		public Variability Variability { get; }

		public Causality Causality { get; }
	}
}