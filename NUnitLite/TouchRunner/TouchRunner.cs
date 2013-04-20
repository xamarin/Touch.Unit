// TouchRunner.cs: MonoTouch.Dialog-based driver to run unit tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2012 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.WorkItems;

namespace MonoTouch.NUnit.UI {
	
	public class TouchRunner : ITestListener
	{
		
		UIWindow window;
		TestSuite suite = new TestSuite (String.Empty);
		ITestFilter filter;
		ITestListener listener;

		[CLSCompliant (false)]
		public TouchRunner (UIWindow window)
		{
			if (window == null)
				throw new ArgumentNullException ("window");
			
			this.window = window;
			filter = TestFilter.Empty;
		}
		
		public bool AutoStart {
			get { return TouchOptions.Current.AutoStart; }
			set { TouchOptions.Current.AutoStart = value; }
		}
		
		public ITestFilter Filter {
			get { return filter; }
			set { filter = value; }
		}

		public ITestListener Listener {
			get { return listener ?? (listener = new DefaultTestListener(Writer)); }
			set { listener = value; }
		}
		
		public bool TerminateAfterExecution {
			get { return TouchOptions.Current.TerminateAfterExecution; }
			set { TouchOptions.Current.TerminateAfterExecution = value; }
		}
		
		[CLSCompliant (false)]
		public UINavigationController NavigationController {
			get { return (UINavigationController) window.RootViewController; }
		}
		
		List<Assembly> assemblies = new List<Assembly> ();
		ManualResetEvent mre = new ManualResetEvent (false);
		
		public void Add (Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException ("assembly");
			
			assemblies.Add (assembly);
		}
		
		static void TerminateWithSuccess ()
		{
			Selector selector = new Selector ("terminateWithSuccess");
			UIApplication.SharedApplication.PerformSelector (selector, UIApplication.SharedApplication, 0);						
		}
		
		[CLSCompliant (false)]
		public UIViewController GetViewController ()
		{
			var menu = new RootElement ("Test Runner");
			
			Section main = new Section ("Loading test suites...");
			menu.Add (main);
			
			Section options = new Section () {
				new StyledStringElement ("Options", Options) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
				new StyledStringElement ("Credits", Credits) { Accessory = UITableViewCellAccessory.DisclosureIndicator }
			};
			menu.Add (options);

			// large unit tests applications can take more time to initialize
			// than what the iOS watchdog will allow them on devices
			ThreadPool.QueueUserWorkItem (delegate {
				foreach (Assembly assembly in assemblies)
					Load (assembly, null);

				window.InvokeOnMainThread (delegate {
					foreach (TestSuite ts in suite.Tests) {
						main.Add (Setup (ts));
					}
					mre.Set ();
					
					main.Caption = null;
					menu.Reload (main, UITableViewRowAnimation.Fade);
					
					options.Insert (0, new StringElement ("Run Everything", Run));
					menu.Reload (options, UITableViewRowAnimation.Fade);
				});
				assemblies.Clear ();
			});
			
			var dv = new DialogViewController (menu) { Autorotate = true };

			// AutoStart running the tests (with either the supplied 'writer' or the options)
			if (AutoStart) {
				ThreadPool.QueueUserWorkItem (delegate {
					mre.WaitOne ();
					window.BeginInvokeOnMainThread (delegate {
						Run ();	
						// optionally end the process, e.g. click "Touch.Unit" -> log tests results, return to springboard...
						// http://stackoverflow.com/questions/1978695/uiapplication-sharedapplication-terminatewithsuccess-is-not-there
						if (TerminateAfterExecution)
							TerminateWithSuccess ();
					});
				});
			}
			return dv;
		}
		
		void Run ()
		{
			if (!OpenWriter ("Run Everything"))
				return;
			try {
				Run (suite);
			}
			finally {
				CloseWriter ();
			}
		}
				
		void Options ()
		{
			NavigationController.PushViewController (TouchOptions.Current.GetViewController (), true);				
		}
		
		void Credits ()
		{
			var title = new MultilineElement ("Touch.Unit Runner\nCopyright 2011-2012 Xamarin Inc.\nAll rights reserved.");
			title.Alignment = UITextAlignment.Center;
			
			var root = new RootElement ("Credits") {
				new Section () { title },
				new Section () {
					new HtmlElement ("About Xamarin", "http://www.xamarin.com"),
					new HtmlElement ("About MonoTouch", "http://ios.xamarin.com"),
					new HtmlElement ("About MonoTouch.Dialog", "https://github.com/migueldeicaza/MonoTouch.Dialog"),
					new HtmlElement ("About NUnitLite", "http://www.nunitlite.org"),
					new HtmlElement ("About Font Awesome", "http://fortawesome.github.com/Font-Awesome")
				}
			};
				
			var dv = new DialogViewController (root, true) { Autorotate = true };
			NavigationController.PushViewController (dv, true);				
		}
		
		#region writer
		
		public TextWriter Writer { get; set; }
		
		static string SelectHostName (string[] names, int port)
		{
			if (names.Length == 0)
				return null;
			
			if (names.Length == 1)
				return names [0];
			
			object lock_obj = new object ();
			string result = null;
			int failures = 0;
			
			using (var evt = new ManualResetEvent (false)) {
				for (int i = names.Length - 1; i >= 0; i--) {
					var name = names [i];
					ThreadPool.QueueUserWorkItem ((v) =>
					{
						try {
							var client = new TcpClient (name, port);
							using (var writer = new StreamWriter (client.GetStream ())) {
								writer.WriteLine ("ping");
							}
							lock (lock_obj) {
								if (result == null)
									result = name;
							}
							evt.Set ();
						} catch (Exception) {
							lock (lock_obj) {
								failures++;
								if (failures == names.Length)
									evt.Set ();
							}
						}
					});
				}
				
				// Wait for 1 success or all failures
				evt.WaitOne ();
			}
			
			return result;
		}
		
		public bool OpenWriter (string message)
		{
			TouchOptions options = TouchOptions.Current;
			DateTime now = DateTime.Now;
			// let the application provide it's own TextWriter to ease automation with AutoStart property
			if (Writer == null) {
				if (options.ShowUseNetworkLogger) {
					var hostname = SelectHostName (options.HostName.Split (','), options.HostPort);
					
					if (hostname != null) {
						Console.WriteLine ("[{0}] Sending '{1}' results to {2}:{3}", now, message, hostname, options.HostPort);
						try {
							Writer = new TcpTextWriter (hostname, options.HostPort);
						}
						catch (SocketException) {
							UIAlertView alert = new UIAlertView ("Network Error", 
								String.Format ("Cannot connect to {0}:{1}. Continue on console ?", hostname, options.HostPort), 
								null, "Cancel", "Continue");
							int button = -1;
							alert.Clicked += delegate(object sender, UIButtonEventArgs e) {
								button = e.ButtonIndex;
							};
							alert.Show ();
							while (button == -1)
								NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.5));
							Console.WriteLine (button);
							Console.WriteLine ("[Host unreachable: {0}]", button == 0 ? "Execution cancelled" : "Switching to console output");
							if (button == 0)
								return false;
							else
								Writer = Console.Out;
						}
					}
				} else {
					Writer = Console.Out;
				}
			}
			
			Writer.WriteLine ("[Runner executing:\t{0}]", message);
			Writer.WriteLine ("[MonoTouch Version:\t{0}]", MonoTouch.Constants.Version);
			UIDevice device = UIDevice.CurrentDevice;
			Writer.WriteLine ("[{0}:\t{1} v{2}]", device.Model, device.SystemName, device.SystemVersion);
			Writer.WriteLine ("[Device Name:\t{0}]", device.Name);
			Writer.WriteLine ("[Device UDID:\t{0}]", device.UniqueIdentifier);
			Writer.WriteLine ("[Device Date/Time:\t{0}]", now); // to match earlier C.WL output

			Writer.WriteLine ("[Bundle:\t{0}]", NSBundle.MainBundle.BundleIdentifier);
			// FIXME: add data about how the app was compiled (e.g. ARMvX, LLVM, GC and Linker options)
			return true;
		}
		
		public void CloseWriter ()
		{
			Writer.Close ();
			Writer = null;
		}
		
		#endregion
		
		Dictionary<TestSuite, TouchViewController> suites_dvc = new Dictionary<TestSuite, TouchViewController> ();
		Dictionary<TestSuite, TestSuiteElement> suite_elements = new Dictionary<TestSuite, TestSuiteElement> ();
		Dictionary<TestMethod, TestCaseElement> case_elements = new Dictionary<TestMethod, TestCaseElement> ();
		
		public void Show (TestSuite suite)
		{
			NavigationController.PushViewController (suites_dvc [suite], true);
		}
	
		TestSuiteElement Setup (TestSuite suite)
		{
			TestSuiteElement tse = new TestSuiteElement (suite, this);
			suite_elements.Add (suite, tse);
			
			var root = new RootElement ("Tests");
		
			Section section = new Section (suite.Name);
			foreach (ITest test in suite.Tests) {
				TestSuite ts = (test as TestSuite);
				if (ts != null) {
					section.Add (Setup (ts));
				} else {
					TestMethod tc = (test as TestMethod);
					if (tc != null) {
						section.Add (Setup (tc));
					} else {
						throw new NotImplementedException (test.GetType ().ToString ());
					}
				}
			}
		
			root.Add (section);
			
			if (section.Count > 1) {
				Section options = new Section () {
					new StringElement ("Run all", delegate () {
						if (OpenWriter (suite.Name)) {
							Run (suite);
							CloseWriter ();
							suites_dvc [suite].Filter ();
						}
					})
				};
				root.Add (options);
			}

			suites_dvc.Add (suite, new TouchViewController (root));
			return tse;
		}
		
		TestCaseElement Setup (TestMethod test)
		{
			TestCaseElement tce = new TestCaseElement (test, this);
			case_elements.Add (test, tce);
			return tce;
		}

		NUnitLiteTestAssemblyBuilder builder = new NUnitLiteTestAssemblyBuilder ();
		Dictionary<string, object> empty = new Dictionary<string, object> ();

		public bool Load (string assemblyName, IDictionary settings)
		{
			return AddSuite (builder.Build (assemblyName, settings ?? empty));
		}

		public bool Load (Assembly assembly, IDictionary settings)
		{
			return AddSuite (builder.Build (assembly, settings ?? empty));
		}

		bool AddSuite (TestSuite ts)
		{
			if (ts == null)
				return false;
			suite.Add (ts);
			return true;
		}

		public TestResult Run (Test test)
		{
			TestExecutionContext current = TestExecutionContext.CurrentContext;
			current.WorkDirectory = Environment.CurrentDirectory;
			current.Listener = Listener;
			current.TestObject = test is TestSuite ? null : Reflect.Construct ((test as TestMethod).Method.ReflectedType, null);
			WorkItem wi = WorkItem.CreateWorkItem (test, current, filter);
			wi.Execute ();
			return wi.Result;
		}

		public ITest LoadedTest {
			get {
				return suite;
			}
		}

		#region ITestListener implementation

		public void TestStarted (ITest test)
		{
			Listener.TestStarted(test);
		}

		public void TestFinished (ITestResult r)
		{
			Listener.TestFinished(r);
			
			TestResult result = r as TestResult;
			TestSuite ts = result.Test as TestSuite;
			if (ts != null) {
				TestSuiteElement tse;
				if (suite_elements.TryGetValue (ts, out tse))
					tse.Update (result);
			} else {
				TestMethod tc = result.Test as TestMethod;
				if (tc != null)
					case_elements [tc].Update (result);
			}
		}

		public void TestOutput (TestOutput testOutput)
		{
			Listener.TestOutput(testOutput);
		}


		#endregion
	}
}