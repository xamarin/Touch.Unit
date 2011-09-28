// TestElement.cs: MonoTouch.Dialog element for ITest, i.e. TestSuite and TestCase
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

	abstract class TestElement : StyledStringElement {
		
		static internal UIColor DarkGreen = UIColor.FromRGB (0x00, 0x77, 0x00);
	
		private TestResult result;
		
		public TestElement (ITest test, TouchRunner runner)
			: base ("?", "?", UITableViewCellStyle.Subtitle)
		{
			if (test == null)
				throw new ArgumentNullException ("test");
			if (runner == null)
				throw new ArgumentNullException ("runner");
		
			Test = test;
			Runner = runner;
		}

		protected TouchRunner Runner { get; private set; }
		
		protected TestResult Result {
			get { return result ?? new TestResult (Test); }
			set { result = value; }
		}
		
		protected ITest Test { get; private set; }
		
		public void Update (TestResult result)
		{
			Result = result;
			
			Update ();
			
			if (GetContainerTableView () != null) {
				var root = GetImmediateRootElement ();
				root.Reload (this, UITableViewRowAnimation.Fade);
			}
		}
		
		abstract public void Update ();
	}
}