// TouchRunner.cs: MonoTouch.Dialog-based driver to run unit tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2012 Xamarin Inc. All rights reserved

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
	
	/// <summary>
	/// <c>TouchRunner</c> is the main class for Touch.Unit. With a few settings it will create
	/// it's own UI and can start (by user input or automatically) executing all unit tests that
	/// were added to the runner's instance. By default all logs will be displayed on the console
	/// (i.e. they will be shown in MonoDevelop's Application Output pad) or they can be sent to
	/// a network server (e.g. for automated testing).
	/// </summary>
	/// <code>
	/// // a common FinishedLaunching implementation to use Touch.Unit inside a project
	/// public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	/// {
	/// 	window = new UIWindow (UIScreen.MainScreen.Bounds);
	/// 	runner = new TouchRunner (window);
	/// 	// register every tests included in the main application/assembly
	/// 	runner.Add (System.Reflection.Assembly.GetExecutingAssembly ());
	/// 	window.RootViewController = new UINavigationController (runner.GetViewController ());
	/// 	window.MakeKeyAndVisible ();
	///		return true;
	/// }
	/// </code>
	/// <code>
	/// // a FinishedLaunching implementation to use the automated testing features
	/// public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	/// {
	/// 	window = new UIWindow (UIScreen.MainScreen.Bounds);
	/// 	runner = new TouchRunner (window);
	/// 	// register every tests included in the main application/assembly
	/// 	runner.Add (System.Reflection.Assembly.GetExecutingAssembly ());
	/// 	// you can use a TcpTextWriter or set your own custom writer
	/// 	runner.Writer = new TcpTextWriter ("10.0.1.2", 16384);
	/// 	// start running the test suites as soon as the application is loaded
	/// 	runner.AutoStart = true;
	/// 	// crash the application (to ensure it's ended) and return to springboard
	/// 	runner.TerminateAfterExecution = true;
	/// 	window.RootViewController = new UINavigationController (runner.GetViewController ());
	/// 	window.MakeKeyAndVisible ();
	///		return true;
	/// }
	/// </code>
	public class TouchRunner : TestListener {
		
		UIWindow window;
		TouchOptions options;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoTouch.NUnit.UI.TouchRunner"/> class with
		/// the default options, previously saved user options or (for automated testing) by parsing
		/// the command-lines options given to the application.
		/// </summary>
		/// <param name='window'>
		/// The <c>UIWindow</c> instance where the runner's user interface will be down.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when a <see langword="null" /> <c>UIWindow</c> instance is passed to this method.
		/// </exception>
		public TouchRunner (UIWindow window)
		{
			if (window == null)
				throw new ArgumentNullException ("window");
			
			this.window = window;
			options = new TouchOptions ();
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="MonoTouch.NUnit.UI.TouchRunner"/> 
		/// should automatically start the unit tests execution when the application is launched. This
		/// can be used with the network logger to completely automate test execution.
		/// </summary>
		/// <value>
		/// <c>true</c> for automatic test execution; otherwise, <c>false</c> (default).
		/// </value>
		public bool AutoStart {
			get { return options.AutoStart; }
			set { options.AutoStart = value; }
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="MonoTouch.NUnit.UI.TouchRunner"/> 
		/// should automatically terminate (i.e. kill) the application after execution of all unit tests.
		/// Useful if you want to automate the sequential launch of several Touch.Unit applications
		/// (e.g. different test suites, different CPU architecture on the same tests).
		/// </summary>
		/// <value>
		/// <c>true</c> if the application shall terminate after the unit tests execution; otherwise, 
		/// <c>false</c> (default).
		/// </value>
		public bool TerminateAfterExecution {
			get { return options.TerminateAfterExecution; }
			set { options.TerminateAfterExecution = value; }
		}
		
		/// <summary>
		/// Helper property that gets the <c>RootViewController</c> property of the `UIWindow` 
		/// that was provided to the constructor.
		/// </summary>
		/// <value>
		/// The navigation controller.
		/// </value>
		public UINavigationController NavigationController {
			get { return (UINavigationController) window.RootViewController; }
		}
		
		List<TestSuite> suites = new List<TestSuite> ();
		
		/// <summary>
		/// Add the specified assembly to the list of assemblies that the runner will scan
		/// to find test suites, i.e. types decorated with <c>[TestSuite]</c>.
		/// </summary>
		/// <param name='assembly'>
		/// Assembly that contains one, or more, test suites to be executed by the runner.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an <see langword="null" /> assembly is passed to this method.
		/// </exception>
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
		
		/// <summary>
		/// Gets the view controller that will coordinate the UI and unit tests execution, 
		/// including the (optional) automation support to start and terminate the application
		/// automatically.
		/// </summary>
		/// <returns>
		/// The view controller.
		/// </returns>
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
			var title = new MultilineElement ("Touch.Unit Runner\nCopyright 2011-2012 Xamarin Inc.\nAll rights reserved.\n\nAuthor: Sebastien Pouliot");
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
		
		/// <summary>
		/// Gets or sets the <c>TextWriter</c> instance where the results of the unit tests
		/// execution will be written. By default all text is written to <c>Console.Out</c>
		/// but the user can select (using the options) or programmatically set a <c>TcpTextWriter</c>
		/// instance to have the test result be sent to a remote server.
		/// </summary>
		/// <value>
		/// The writer.
		/// </value>
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
						} catch (Exception ex) {
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
		
		/// <summary>
		/// Opens the writer when starting the unit tests execution.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the writer could be initialized properly, <c>false</c> otherwise (e.g. if
		/// the network logger could not contact it's server).
		/// </returns>
		/// <param name='message'>
		/// First message to write on the output.
		/// </param>
		public bool OpenWriter (string message)
		{
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
			Writer.WriteLine ("[Device Date/Time:\t{0}]", now); // to match earlier C.WL output
			// FIXME: add more data about the device
			
			Writer.WriteLine ("[Bundle:\t{0}]", NSBundle.MainBundle.BundleIdentifier);
			// FIXME: add data about how the app was compiled (e.g. ARMvX, LLVM, Linker options)
			return true;
		}
		
		/// <summary>
		/// Closes the current writer. This will end any network connection.
		/// </summary>
		public void CloseWriter ()
		{
			Writer.Close ();
			Writer = null;
		}
		
		#endregion
		
		Dictionary<TestSuite, DialogViewController> suites_dvc = new Dictionary<TestSuite, DialogViewController> ();
		Dictionary<TestSuite, TestSuiteElement> suite_elements = new Dictionary<TestSuite, TestSuiteElement> ();
		Dictionary<TestCase, TestCaseElement> case_elements = new Dictionary<TestCase, TestCaseElement> ();
		
		/// <summary>
		/// Push the <c>UIViewController</c> that is responsable to handle the specified 
		/// <c>TestSuite</c> instance.
		/// </summary>
		/// <param name='suite'>
		/// The <c>TestSuite</c> to be shown.
		/// </param>
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
		
		/// <summary>
		/// Called when an <c>ITest</c> is about to start it's execution.
		/// </summary>
		/// <param name='test'>
		/// An <c>ITest</c>, i.e. either a test suite or a test case.
		/// </param>
		public void TestStarted (ITest test)
		{
			if (test is TestSuite) {
				Writer.WriteLine ();
				time.Push (DateTime.UtcNow);
				Writer.WriteLine (test.Name);
			}
		}
		
		Stack<DateTime> time = new Stack<DateTime> ();
		
		/// <summary>
		/// Called when test suite or test case is completed and results are available.
		/// </summary>
		/// <param name='result'>
		/// The <c>TestResult</c> instance that contains the results and a reference to the test.
		/// </param>
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
				if (result.IsSuccess ()) {
					Writer.Write ("\t[PASS] ");
				} else if (result.IsIgnored ()) {
					Writer.Write ("\t[IGNORED] ");
				} else if (result.IsError ()) {
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