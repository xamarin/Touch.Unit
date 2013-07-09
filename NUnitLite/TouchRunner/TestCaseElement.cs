// TestCaseElement.cs: MonoTouch.Dialog element for TestCase
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2013 Xamarin Inc.
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
using System.Reflection;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Api;

namespace MonoTouch.NUnit.UI {
	
	class TestCaseElement : TestElement {
		
		public TestCaseElement (TestMethod testCase, TouchRunner runner)
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
				if ((TestCase.RunState == RunState.Runnable) && !Result.IsSuccess ()) {
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
		
		public TestMethod TestCase {
			get { return Test as TestMethod; }
		}
		
		public void Run ()
		{
			Update (Runner.Run (TestCase));
		}
		
		public override void Update ()
		{
			if (Result.IsIgnored ()) {
				Value = Result.GetMessage ();
				DetailColor = UIColor.Orange;
			} else if (Result.IsSuccess () || Result.IsInconclusive ()) {
				int counter = Result.AssertCount;
				Value = String.Format ("{0} {1} ms for {2} assertion{3}",
					Result.IsInconclusive () ? "Inconclusive." : "Success!",
					Result.Duration.TotalMilliseconds, counter,
					counter == 1 ? String.Empty : "s");
				DetailColor = DarkGreen;
			} else if (Result.IsFailure ()) {
				Value = Result.GetMessage ();
				DetailColor = UIColor.Red;
			} else {
				// Assert.Ignore falls into this
				Value = Result.GetMessage ();
			}
		}
	}
}