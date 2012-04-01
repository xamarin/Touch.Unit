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
		DateTime start;
		
		public TestSuiteElement (TestSuite test, TouchRunner runner)
			: base (test, runner)
		{
			Caption = Suite.Name;
			Suite.StartedEvent += StartedHandler;;
			Suite.CompletedEvent += CompletedHandler;
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
			Suite.RunAsync (Runner);
		}
		
		void StartedHandler (object sender, EventArgs args)
		{
			start = DateTime.UtcNow;
		}
		
		void CompletedHandler (object sender, EventArgs args)
		{
			time = (DateTime.UtcNow - start);
			Result = Suite.Result;
			Update ();
		}
		
		public override void Update ()
		{
			int error = 0;
			int failure = 0;
			int success = 0;
			Count (Result, ref error, ref failure, ref success);
			
			if ((failure == 0) && (error == 0) && (success > 0)) {
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
			
			Reload ();
		}
		
		void Count (TestResult result, ref int error, ref int failure, ref int success)
		{
			foreach (TestResult tr in result.Results) {
				if (tr.Results.Count > 0)
					Count (tr, ref error, ref failure, ref success);
				else if (tr.IsError ())
					error++;
				else if (tr.IsFailure ())
					failure++;
				else
					success++;
			}
		}
	}
}