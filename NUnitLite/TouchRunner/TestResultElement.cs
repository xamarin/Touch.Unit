// TestResultElement.cs: MonoTouch.Dialog element for TestResult
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2012 Xamarin Inc. All rights reserved

using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MonoTouch.NUnit.UI {
	
	class TestResultElement : StyledMultilineElement {
		
		public TestResultElement (TestResult result) : 
			base (result.Message ?? "Unknown error", result.StackTrace, UITableViewCellStyle.Subtitle)
		{
		}
	}
}