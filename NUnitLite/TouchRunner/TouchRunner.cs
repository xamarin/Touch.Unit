// TouchRunner.cs: MonoTouch.Dialog-based driver to run unit tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011 Xamarin Inc. All rights reserved

using System;
using System.Collections.Generic;
using System.Reflection;

using MonoTouch.Dialog;
using MonoTouch.UIKit;

using NUnitLite;
using NUnitLite.Runner;

namespace MonoTouch.NUnit.UI {
	
	public class TouchRunner : TestListener {
		
		UIWindow window;
		
		public TouchRunner (UIWindow window)
		{
			if (window == null)
				throw new ArgumentNullException ("window");
			
			this.window = window;
		}

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
		
		public UIViewController GetViewController ()
		{
			var menu = new RootElement ("Test Runner");
			
			Section main = new Section ();
			foreach (TestSuite suite in suites) {
				main.Add (Setup (suite));
			}
			menu.Add (main);
			
			Section options = new Section () {
				new StringElement ("Run", Run),
				new StringElement ("Options...", Options),
				new StringElement ("Credits...", Credits)
			};
			menu.Add (options);
			
			var dv = new DialogViewController (menu) {
				Autorotate = true
			};
			return dv;
		}
		
		void Run ()
		{
			foreach (TestSuite ts in suites)
				suite_elements [ts].Run ();
		}
		
		void Options ()
		{
			// send results to...
		}
		
		void Credits ()
		{
			// monotouch.dialog, nunitlite, xamarin
		}

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
						Run (suite);
					})
				};
				root.Add (options);
			}
				
			suites_dvc.Add (suite, new DialogViewController (root, true));
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
		}
			
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
		}
	}
}