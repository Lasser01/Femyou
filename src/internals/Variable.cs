using System;
using System.Xml.Linq;

namespace Femyou.internals {
	internal class Variable : IVariable {
		public uint ValueReference { get; }
		public string Name { get; }
		public string Description { get; }
		public IVariableType VariableType { get; }

		public Variability Variability { get; }

		public Causality Causality { get; }
		
		public Variable(XElement xElement) {
			XAttribute name = xElement.Attribute("name");
			Name = name != null ? name.Value : "No name";

			XAttribute valueReference = xElement.Attribute("valueReference");
			if (valueReference == null)
				throw new ArgumentException("Value refence isn't allowed to be null or not set in the xml");

			ValueReference = uint.Parse(valueReference.Value);

			XAttribute description = xElement.Attribute("description");
			Description = description != null ? description.Value : "No description";

			XAttribute variability = xElement.Attribute("variability");
			Variability = variability != null
				? Enum.Parse<Variability>(variability.Value, true)
				: Variability.Unknown;

			XAttribute causality = xElement.Attribute("causality");
			Causality = causality != null
				? Enum.Parse<Causality>(causality.Value, true)
				: Causality.Unknown;

			VariableType = new VariableTypeImpl(xElement);
		}

		public override string ToString() {
			return
				$"{nameof(Name)}: {Name}, {nameof(VariableType)}: {VariableType}, {nameof(Variability)}: {Variability}, {nameof(Causality)}: {Causality}";
		}
	}
}