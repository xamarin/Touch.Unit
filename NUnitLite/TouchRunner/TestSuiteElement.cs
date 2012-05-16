// TestSuiteElement.cs: MonoTouch.Dialog element for TestSuite
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2012 Xamarin Inc. All rights reserved

using System;
using System.Text;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

using NUnit.Framework.Internal;

namespace MonoTouch.NUnit.UI {

	class TestSuiteElement : TestElement {

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
			Result = Runner.Run (Suite);
			Update ();
		}
		
		public override void Update ()
		{
			int positive = Result.PassCount + Result.InconclusiveCount;
			int failure = Result.FailCount;
			int skipped = Result.SkipCount;

			StringBuilder sb = new StringBuilder ();
			if (failure == 0) {
				DetailColor = DarkGreen;
				sb.Append ("Success! ").Append (Result.Time * 1000).Append (" ms for ").Append (positive).Append (" test");
				if (positive > 1)
					sb.Append ('s');
			} else {
				DetailColor = UIColor.Red;
				if (positive > 0)
					sb.Append (positive).Append (" success");
				if (sb.Length > 0)
					sb.Append (", ");
				sb.Append (failure).Append (" failure");
				if (failure > 1)
					sb.Append ('s');
				if (skipped > 0)
					sb.Append (", ").Append (skipped).Append (" ignored");
			}
			Value = sb.ToString ();
		}
	}
}