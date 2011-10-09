// TestResultElement.cs: MonoTouch.Dialog element for TestResult
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
	
	class TestResultElement : StyledMultilineElement {
		
		public TestResultElement (TestResult result) : 
			base (result.Message ?? "Unknown error", result.StackTrace, UITableViewCellStyle.Subtitle)
		{
		}
	}
}