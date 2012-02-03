// TestCaseElement.cs: MonoTouch.Dialog element for TestCase
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011 Xamarin Inc. All rights reserved

using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

using NUnitLite;
using NUnitLite.Runner;
using NUnit.Framework;

namespace MonoTouch.NUnit.UI {
	
	class TestCaseElement : TestElement {
		
		TimeSpan time;
		bool tapped;
		DateTime start;
		
		public TestCaseElement (TestCase testCase, TouchRunner runner)
			: base (testCase, runner)
		{
			Caption = testCase.Name;
			Value = "NotExecuted";
			testCase.StartedEvent += StartedHandler;
			testCase.CompletedEvent += CompletedHandler;
			this.Tapped += delegate {
				if (!Runner.OpenWriter (Test.FullName))
					return;
				tapped = true;
				Run ();
			};
		}
		
		void StartedHandler (object sender, EventArgs args)
		{
			start = DateTime.UtcNow;
		}
		
		void CompletedHandler (object sender, EventArgs args)
		{
			if (tapped) {
				Runner.CloseWriterAsync ();
				// display more details on (any) failure (but not when ignored)
				if ((TestCase.RunState == RunState.Runnable) && !Result.IsSuccess) {
					var root = new RootElement ("Results") {
						new Section () {
							new TestResultElement (Result)
						}
					};
					var dvc = new DialogViewController (root, true) { Autorotate = true };
					Runner.NavigationController.PushViewController (dvc, true);
				}
				tapped = false;
			}
			time = (DateTime.UtcNow - start);
			Result = TestCase.Result;
			Update ();
		}
		
		public TestCase TestCase {
			get { return Test as TestCase; }
		}
		
		public void Run ()
		{
			TestCase.RunAsync (Runner);
		}
		
		public override void Update ()
		{
			string m = Result.Message;
			if (Result.IsIgnored ()) {
				Value = Result.GetMessage ();
				DetailColor = UIColor.Orange;
			} else if (Result.IsError ()) {
				Value = Result.GetMessage ();
				DetailColor = UIColor.Red;
			} else if (Result.IsSuccess ()) {
				int counter = Assert.Counter;
				Value = String.Format ("{0} {1} ms for {2} assertion{3}",
					Result.IsInconclusive () ? "Inconclusive." : "Success!",
					time.TotalMilliseconds, counter,
					counter == 1 ? String.Empty : "s");
				DetailColor = DarkGreen;
			}
			
			Reload ();
		}
	}
}