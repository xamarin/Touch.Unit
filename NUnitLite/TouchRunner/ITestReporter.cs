using System;
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace MonoTouch.NUnit
{
	public interface ITestReporter : ITestListener
	{
		void TestSuiteStarted (TestSuite ts);
		void TestSuiteFinished (TestSuite ts);
	}
}

