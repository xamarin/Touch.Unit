// AppDelegate.cs: Customize (add tests assemblies) your runner application here!
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011 Xamarin Inc. All rights reserved

using System.Reflection;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.NUnit.UI;

namespace touchunit
{
	/// <summary>
	/// The UIApplicationDelegate for the application. This class is responsible for launching the 
	/// User Interface of the application, as well as listening (and optionally responding) to 
	/// application events from iOS.
	/// </summary>
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate {
		// class-level declarations
		UIWindow window;
		TouchRunner runner;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			runner = new TouchRunner (window);

			// tests can be inside the main assembly
			runner.Add (Assembly.GetExecutingAssembly ());
			// otherwise you need to ensure that the test assemblies will 
			// become part of the app bundle
			runner.Add (typeof (MonoTouchFixtures.RegressionTest).Assembly);

			window.RootViewController = new UINavigationController (runner.GetViewController ());
			window.MakeKeyAndVisible ();
			return true;
		}
	}
}