using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using AppKit;
using Foundation;

namespace MonoTouch.NUnit.UI {
	public class MacRunner : BaseTouchRunner {
		// The exitProcess callback must not return. The boolean parameter specifies whether the test run succeeded or not.
		public static async Task<int> MainAsync (IList<string> arguments, bool requiresNSApplicationRun, Action<int> exitProcess, params Assembly[] assemblies)
		{
			var success = false;

			NSApplication.Init ();

			var options = new TouchOptions (arguments);
			TouchOptions.Current = options;

			if (requiresNSApplicationRun) {
				var app = NSApplication.SharedApplication;
				app.InvokeOnMainThread (async () => {
					success = await RunTestsAsync (options, assemblies);
					// The only reliable way to stop NSApplication.Run is to call NSApplication.Terminate, which will
					// terminate the app, but won't allow us to specify the exit code. So we need an callback that will
					// exit the process
					exitProcess (success ? 0 : 1);
				});
				app.Run ();
			} else {
				success = await RunTestsAsync (options, assemblies);
			}

			return success ? 0 : 1;
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
