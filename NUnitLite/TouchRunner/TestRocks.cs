// TestRocks.cs: Helpers
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2012 Xamarin Inc. All rights reserved

using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Api;

namespace MonoTouch.NUnit {
	
	static class TestRock {
		
		const string NUnitFrameworkExceptionPrefix = "NUnit.Framework.";

		static public bool IsIgnored (this TestResult result)
		{
			return (result.ResultState.Status == TestStatus.Skipped);
		}
		
		static public bool IsSuccess (this TestResult result)
		{
			return (result.ResultState.Status == TestStatus.Passed);
		}
		
		static public bool IsFailure (this TestResult result)
		{
			return (result.ResultState.Status == TestStatus.Failed);
		}
		
		static public bool IsInconclusive (this TestResult result)
		{
			return (result.ResultState.Status == TestStatus.Inconclusive);
		}

		// remove the nunit exception message from the "real" message
		static public string GetMessage (this TestResult result)
		{
			string m = result.Message;
			if (m == null)
				return "Unknown error";
			if (!m.StartsWith (NUnitFrameworkExceptionPrefix))
				return m;
			return m.Substring (m.IndexOf (" : ") + 3);
		}
	}
}