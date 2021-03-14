using System;
using System.IO;
using System.Reflection;

namespace Femyou.Tests {
	public static class TestTools {
		public static string FmuFolder => Path.Combine(BaseFolder, "FMU", "bin", "dist");

		public static string BaseFolder =>
			Tools.GetBaseFolder(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath, nameof(Femyou));

		public static string GetFmuPath(string filename) {
			return Path.Combine(FmuFolder, filename);
		}
	}
}