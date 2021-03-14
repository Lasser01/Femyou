using System;
using System.IO;

namespace Femyou {
	public class ConsoleCallbacks : ICallbacks {
		public void Logger(IInstance instance, Status status, string category, string message) {
			using var console = new StreamWriter(Console.OpenStandardOutput());
			console.WriteLine($"[{instance.Name}] {category}({status}): {message}");
		}
	}
}