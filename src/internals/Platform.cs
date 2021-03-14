using System;
using System.IO;

namespace Femyou.internals {
	public class Platform {
		public static string GetLibraryPath(PlatformID platformID, bool is64bits, string baseFolder, string name) {
			return platformID switch {
				PlatformID.Unix => Path.Combine(baseFolder, "binaries", "linux" + Bits(is64bits), name + ".so"),
				PlatformID.Win32NT => Path.Combine(baseFolder, "binaries", "win" + Bits(is64bits), name + ".dll"),
				_ => throw new ArgumentException($"Unsupported platform {platformID}")
			};
		}

		private static string Bits(bool is64bits) {
			return is64bits ? "64" : "32";
		}
	}
}