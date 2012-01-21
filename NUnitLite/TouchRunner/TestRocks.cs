using NUnitLite;
using NUnitLite.Runner;
using NUnit.Framework;

namespace MonoTouch.NUnit {
	
	static class TestRock {
		
		const string NUnitFrameworkExceptionPrefix = "NUnit.Framework.";
		const string IgnoreExceptionPrefix = NUnitFrameworkExceptionPrefix + "IgnoreException : ";
		const string InconclusiveExceptionPrefix = NUnitFrameworkExceptionPrefix + "InconclusiveException : ";
		const string SuccessExceptionPrefix = NUnitFrameworkExceptionPrefix + "SuccessException : ";
		
		static public bool IsIgnored (this TestResult result)
		{
			if (result.IsError)
				return result.Message.StartsWith (IgnoreExceptionPrefix);
			return result.Test.RunState == RunState.Ignored;
		}
		
		static public bool IsSuccess (this TestResult result)
		{
			if (result.IsError) {
				string m = result.Message;
				return (m.StartsWith (InconclusiveExceptionPrefix) || m.StartsWith (SuccessExceptionPrefix));
			}
			return result.IsSuccess;
		}
		
		static public bool IsError (this TestResult result)
		{
			if (result.IsError) {
				string m = result.Message;
				return (!m.StartsWith (InconclusiveExceptionPrefix) && !m.StartsWith (SuccessExceptionPrefix));
			}
			return result.IsFailure;
		}
		
		static public bool IsInconclusive (this TestResult result)
		{
				return (result.IsError && result.Message.StartsWith (InconclusiveExceptionPrefix));
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