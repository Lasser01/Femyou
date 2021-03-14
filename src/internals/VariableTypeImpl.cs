using System.Xml.Linq;

namespace Femyou.internals {
	internal class VariableTypeImpl : IVariableType {
		public VariableTypeImpl(string declaredType, string start, VariableType variableType) {
			DeclaredType = declaredType;
			Start = start;
			VariableType = variableType;
		}

		public VariableTypeImpl(XElement variableElement) {
			XElement real = variableElement.Element("Real");
			XElement inter = variableElement.Element("Integer");
			XElement st = variableElement.Element("String");
			XElement boolean = variableElement.Element("Boolean");
			XElement enumeration = variableElement.Element("Enumeration");

			XElement elementToUse;
			if (real != null) {
				VariableType = VariableType.Real;
				elementToUse = real;
			}
			else if (inter != null) {
				VariableType = VariableType.Integer;
				elementToUse = inter;
			}
			else if (st != null) {
				VariableType = VariableType.String;
				elementToUse = st;
			}
			else if (boolean != null) {
				VariableType = VariableType.Boolean;
				elementToUse = boolean;
			}
			else if (enumeration != null) {
				VariableType = VariableType.Enumeration;
				elementToUse = enumeration;
			}
			else {
				VariableType = VariableType.Unknown;
				return;
			}


			XAttribute start = elementToUse.Attribute("start");
			Start = start != null ? start.Value : "";

			XAttribute declaredType = elementToUse.Attribute("declaredType");
			DeclaredType = declaredType != null ? declaredType.Value : "";
		}

		public string DeclaredType { get; }
		public string Start { get; }
		public VariableType VariableType { get; }

		public override string ToString() {
			return
				$"{nameof(DeclaredType)}: {DeclaredType}, {nameof(Start)}: {Start}, {nameof(VariableType)}: {VariableType}";
		}
	}
}