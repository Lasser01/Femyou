using System.IO;

namespace Femyou {
	public static class Tools {
		public static string GetBaseFolder(string folder, string name) {
			string directory = Path.GetDirectoryName(folder);
			if (Path.GetFileName(directory) == name)
				return directory;
			return GetBaseFolder(directory, name);
		}

		public static IInstance CreateInstance(IModel model, string name) {
			return model.CreateCoSimulationInstance(name, new ConsoleCallbacks());
		}
	}
}