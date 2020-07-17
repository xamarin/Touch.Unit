using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using AppKit;
using Foundation;

namespace MonoTouch.NUnit.UI {
	public class MacRunner : BaseTouchRunner {
		public static async Task<int> MainAsync (IList<string> arguments, params Assembly[] assemblies)
		{
			NSApplication.Init ();

			var options = new TouchOptions (arguments);
			TouchOptions.Current = options;
			return await RunTestsAsync (options, assemblies) ? 0 : 1;
		}

		static async Task<bool> RunTestsAsync (TouchOptions options, Assembly[] assemblies)
		{
			var runner = new MacRunner ();
			if (assemblies == null || assemblies.Length == 0)
				assemblies = AppDomain.CurrentDomain.GetAssemblies ();

			foreach (var asm in assemblies)
				runner.Load (asm);

			await runner.RunAsync ();

			return !runner.Result.IsFailure ();
		}

		protected override void WriteDeviceInformation (TextWriter writer)
		{
			var piinfo = NSProcessInfo.ProcessInfo;
			writer.WriteLine ("[macOS: {0}]", piinfo.OperatingSystemVersionString);
		}
	}
}
