using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using Femyou.fmi;

namespace Femyou.internals {
	internal class ModelImpl : IModel {
		private readonly Library CoSimulation;
		public readonly string FmuPath;
		public readonly XDocument ModelDescription;
		public readonly string TmpFolder;

		public ModelImpl(string fmuPath) {
			FmuPath = fmuPath;
			TmpFolder = Path.Combine(Path.GetTempPath(), nameof(Femyou), Path.GetFileName(FmuPath));
			ZipFile.ExtractToDirectory(fmuPath, TmpFolder, true);
			ModelDescription = XDocument.Load(Path.Combine(TmpFolder, "modelDescription.xml"));
			Variables = ModelDescription.Root.Element("ModelVariables").Elements()
				.Select(sv => new Variable(sv) as IVariable).ToDictionary(sv => sv.Name, sv => sv);

			XElement coSim = ModelDescription.Root.Element("CoSimulation");

			if (coSim == null)
				throw new NotSupportedException("Model Exchange is not supported.");
			string coSimulationID = coSim.Attribute("modelIdentifier").Value;
			CoSimulation = new Library(TmpFolder, coSimulationID);

			FMIVersion = ModelDescription.Root.Attribute("fmiVersion").Value;
			if (FMIVersion != "2.0")
				throw new NotSupportedException("We are unsure if your model will work since it's not FMI version 2.0");

			XAttribute genTool = ModelDescription.Root.Attribute("generationTool");
			GenerationTool = genTool != null ? genTool.Value : "Unknown";

			XElement defaultExperiment = ModelDescription.Root.Element("DefaultExperiment");
			if (defaultExperiment == null)
				return;
			XAttribute startTime = defaultExperiment.Attribute("startTime");
			DefaultStartTime = startTime != null ? Convert.ToDouble(startTime.Value) : 0;

			XAttribute stopTime = defaultExperiment.Attribute("stopTime");
			DefaultStopTime = stopTime != null ? Convert.ToDouble(stopTime.Value) : 0;

			XAttribute tolerance = defaultExperiment.Attribute("tolerance");
			Tolerance = tolerance != null ? Convert.ToDouble(tolerance.Value) : 0.001;
		}

		public string GUID => ModelDescription.Root.Attribute("guid").Value;
		public string FMIVersion { get; }
		public double DefaultStartTime { get; }
		public double DefaultStopTime { get; }
		public double Tolerance { get; }
		public string GenerationTool { get; }

		public string Name => ModelDescription.Root.Attribute("modelName").Value;
		public string Description => ModelDescription.Root.Attribute("description").Value;
		public IReadOnlyDictionary<string, IVariable> Variables { get; }

		public IInstance CreateCoSimulationInstance(string name, ICallbacks callbacks) {
			return new Instance(name, this, CoSimulation, FMI2.fmi2Type.fmi2CoSimulation, callbacks);
		}

		public void Dispose() {
			CoSimulation.Dispose();
			Directory.Delete(TmpFolder, true);
		}
	}
}