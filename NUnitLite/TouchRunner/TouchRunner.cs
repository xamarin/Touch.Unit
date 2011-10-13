// TouchRunner.cs: MonoTouch.Dialog-based driver to run unit tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011 Xamarin Inc. All rights reserved

using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

using NUnitLite;
using NUnitLite.Runner;

namespace MonoTouch.NUnit.UI {
	
	public class TouchRunner : TestListener {
		
		UIWindow window;
		TouchOptions options;
		
		public TouchRunner (UIWindow window)
		{
			if (window == null)
				throw new ArgumentNullException ("window");
			
			this.window = window;
			options = new TouchOptions ();
		}
		
		public bool AutoStart { get; set; }
		public bool TerminateAfterExecution { get; set; }

		public UINavigationController NavigationController {
			get { return (UINavigationController) window.RootViewController; }
		}
		
		List<TestSuite> suites = new List<TestSuite> ();
		
		public void Add (Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException ("assembly");
			
			// TestLoader.Load always return a TestSuite so we can avoid casting many times
			suites.Add (TestLoader.Load (assembly) as TestSuite);
		}
		
		static void TerminateWithSuccess ()
		{
			Selector selector = new Selector ("terminateWithSuccess");
			UIApplication.SharedApplication.PerformSelector (selector, UIApplication.SharedApplication, 0);						
		}
		
		public UIViewController GetViewController ()
		{
			var menu = new RootElement ("Test Runner");
			
			Section main = new Section ();
			foreach (TestSuite suite in suites) {
				main.Add (Setup (suite));
			}
			menu.Add (main);
			
			Section options = new Section () {
				new StringElement ("Run Everything", Run),
				new StyledStringElement ("Options", Options) { Accessory = UITableViewCellAccessory.DisclosureIndicator },
				new StyledStringElement ("Credits", Credits) { Accessory = UITableViewCellAccessory.DisclosureIndicator }
			};
			menu.Add (options);
			
			var dv = new DialogViewController (menu) { Autorotate = true };
			
			// AutoStart running the tests (with either the supplied 'writer' or the options)
			if (AutoStart) {
				ThreadPool.QueueUserWorkItem (delegate {
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
				foreach (TestSuite ts in suites)
					suite_elements [ts].Run ();
			}
			finally {
				CloseWriter ();
			}
		}
				
		void Options ()
		{
			NavigationController.PushViewController (options.GetViewController (), true);				
		}
		
		void Credits ()
		{
			var title = new MultilineElement ("Touch.Unit Runner\nCopyright 2011 Xamarin Inc.\nAll rights reserved.\n\nAuthor: Sebastien Pouliot");
			title.Alignment = UITextAlignment.Center;
			
			var root = new RootElement ("Credits") {
				new Section () { title },
				new Section () {
					new HtmlElement ("About Xamarin", "http://www.xamarin.com"),
					new HtmlElement ("About MonoTouch", "http://ios.xamarin.com"),
					new HtmlElement ("About MonoTouch.Dialog", "https://github.com/migueldeicaza/MonoTouch.Dialog"),
					new HtmlElement ("About NUnitLite", "http://www.nunitlite.org")
				}
			};
				
			var dv = new DialogViewController (root, true) { Autorotate = true };
			NavigationController.PushViewController (dv, true);				
		}
		
		#region writer
		
		public TextWriter Writer { get; set; }
		
		public bool OpenWriter (string message)
		{
			DateTime now = DateTime.Now;
			// let the application provide it's own TextWriter to ease automation with AutoStart property
			if (Writer == null) {
				if (options.ShowUseNetworkLogger) {
					Console.WriteLine ("[{0}] Sending '{1}' results to {2}:{3}", now, message, options.HostName, options.HostPort);
					try {
						Writer = new TcpTextWriter (options.HostName, options.HostPort);
					}
					catch (SocketException) {
						UIAlertView alert = new UIAlertView ("Network Error", 
							String.Format ("Cannot connect to {0}:{1}. Continue on console ?", options.HostName, options.HostPort), 
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
				} else {
					Writer = Console.Out;
				}
			}
			
			Writer.WriteLine ("[Runner executing:\t{0}]", message);
			Writer.WriteLine ("[MonoTouch Version:\t{0}]", MonoTouch.Constants.Version);
			UIDevice device = UIDevice.CurrentDevice;
			Writer.WriteLine ("[{0}:\t{1} v{2}]", device.Model, device.SystemName, device.SystemVersion);
			Writer.WriteLine ("[Device Date/Time:\t{0}]", now); // to match earlier C.WL output
			// FIXME: add more data about the device
			
			Writer.WriteLine ("[Bundle:\t{0}]", NSBundle.MainBundle.BundleIdentifier);
			// FIXME: add data about how the app was compiled (e.g. ARMvX, LLVM, Linker options)
			return true;
		}
		
		public void CloseWriter ()
		{
			Writer.Close ();
			Writer = null;
		}
		
		#endregion
		
		Dictionary<TestSuite, DialogViewController> suites_dvc = new Dictionary<TestSuite, DialogViewController> ();
		Dictionary<TestSuite, TestSuiteElement> suite_elements = new Dictionary<TestSuite, TestSuiteElement> ();
		Dictionary<TestCase, TestCaseElement> case_elements = new Dictionary<TestCase, TestCaseElement> ();
		
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
					TestCase tc = (test as TestCase);
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
						}
					})
				};
				root.Add (options);
			}
				
			var dv = new DialogViewController (root, true) { Autorotate = true };
			suites_dvc.Add (suite, dv);
			return tse;
		}
		
		TestCaseElement Setup (TestCase test)
		{
			TestCaseElement tce = new TestCaseElement (test, this);
			case_elements.Add (test, tce);
			return tce;
		}
				
		void Run (TestSuite suite)
		{
			suite_elements [suite].Run ();
		}
		
		public void TestStarted (ITest test)
		{
			if (test is TestSuite) {
				Writer.WriteLine ();
				time.Push (DateTime.UtcNow);
				Writer.WriteLine (test.Name);
			}
		}
		
		Stack<DateTime> time = new Stack<DateTime> ();
			
		public void TestFinished (TestResult result)
		{
			TestSuite ts = result.Test as TestSuite;
			if (ts != null) {
				suite_elements [ts].Update (result);
			} else {
				TestCase tc = result.Test as TestCase;
				if (tc != null)
					case_elements [tc].Update (result);
			}
			
			if (result.Test is TestSuite) {
				if (!result.IsError && !result.IsFailure && !result.IsSuccess && !result.Executed)
					Writer.WriteLine ("\t[INFO] {0}", result.Message);
				
				var diff = DateTime.UtcNow - time.Pop ();
				Writer.WriteLine ("{0} : {1} ms", result.Test.Name, diff.TotalMilliseconds);
			} else {
				if (result.IsSuccess) {
					Writer.Write ("\t{0} ", result.Executed ? "[PASS]" : "[IGNORED]");
				} else if (result.IsFailure || result.IsError) {
					Writer.Write ("\t[FAIL] ");
				} else {
					Writer.Write ("\t[INFO] ");
				}
				Writer.Write (result.Test.Name);
				
				string message = result.Message;
				if (!String.IsNullOrEmpty (message)) {
					Writer.Write (" : {0}", message.Replace ("\r\n", "\\r\\n"));
				}
				Writer.WriteLine ();
						
				string stacktrace = result.StackTrace;
				if (!String.IsNullOrEmpty (result.StackTrace)) {
					string[] lines = stacktrace.Split (new char [] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string line in lines)
						Writer.WriteLine ("\t\t{0}", line);
				}
			}
		}
	}
}