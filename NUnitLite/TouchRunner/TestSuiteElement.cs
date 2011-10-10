// TestSuiteElement.cs: MonoTouch.Dialog element for TestSuite
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

namespace MonoTouch.NUnit.UI {

	class TestSuiteElement : TestElement {

		TimeSpan time;
		
		public TestSuiteElement (TestSuite test, TouchRunner runner)
			: base (test, runner)
		{
			Caption = Suite.Name;
			int count = Suite.TestCaseCount;
			if (count > 0) {
				Accessory = UITableViewCellAccessory.DisclosureIndicator;
				DetailColor = DarkGreen;
				Value = String.Format ("{0} test case{1}, {2}", count, count == 1 ? String.Empty : "s", Suite.RunState);
				Tapped += delegate {
					runner.Show (Suite);
				};
			} else {
				DetailColor = UIColor.Orange;
				Value = "No test was found inside this suite";
			}
		}
		
		public TestSuite Suite {
			get { return Test as TestSuite; }
		}
		
		public void Run ()
		{
			DateTime start = DateTime.UtcNow;
			Result = Suite.Run (Runner);
			time = (DateTime.UtcNow - start);
			Update ();
		}
		
		public override void Update ()
		{
			int error = 0;
			int failure = 0;
			int success = 0;
			foreach (TestResult tr in Result.Results) {
				if (tr.IsError)
					error++;
				else if (tr.IsFailure)
					failure++;
				else if (tr.IsSuccess)
					success++;
			}
			
			if (Result.IsSuccess) {
				Value = String.Format ("Success! {0} ms for {1} test{2}",
					time.TotalMilliseconds, success,
					success == 1 ? String.Empty : "s");
				DetailColor = DarkGreen;
			} else if (Result.Executed) {
				DetailColor = UIColor.Red;
				Value = String.Format ("{0} success, {1} failure{2}, {3} error{4}", 
					success, failure, failure > 1 ? "s" : String.Empty,
					error, error > 1 ? "s" : String.Empty);
			}
		}
	}
}