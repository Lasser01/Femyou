namespace Femyou {
	public interface IVariableType {
		string DeclaredType { get; }
		string Start { get; }

		VariableType VariableType { get; }
	}
}