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
		
		public TestCaseElement (TestCase testCase, TouchRunner runner)
			: base (testCase, runner)
		{
			Caption = testCase.Name;
			Value = "NotExecuted";
			this.Tapped += delegate {
				if (!Runner.OpenWriter (Test.FullName))
					return;
				Run ();
				Runner.CloseWriter ();
				// display more details on (any) failure (but not when ignored)
				if ((TestCase.RunState == RunState.Runnable) && !Result.IsSuccess) {
					var root = new RootElement ("Results") {
						new Section () {
							new TestResultElement (Result)
						}
					};
					var dvc = new DialogViewController (root, true) { Autorotate = true };
					runner.NavigationController.PushViewController (dvc, true);
				}
			};
		}
		
		public TestCase TestCase {
			get { return Test as TestCase; }
		}
		
		public void Run ()
		{
			DateTime start = DateTime.UtcNow;
			Result = TestCase.Run (Runner);
			time = (DateTime.UtcNow - start);
			Update ();
		}
		
		public override void Update ()
		{
			if (TestCase.RunState == RunState.Ignored) {
				Value = Result.Message;
				DetailColor = UIColor.Orange;
			} else if (Result.IsError || Result.IsFailure) {
				Value = Result.Message ?? "Unknown error";
				DetailColor = UIColor.Red;
			} else if (Result.IsSuccess) {
				int counter = Assert.Counter;
				Value = String.Format ("Success! {0} ms for {1} assertion{2}",
					time.TotalMilliseconds, counter,
					counter == 1 ? String.Empty : "s");
				DetailColor = DarkGreen;
			}
		}
	}
}