using NUnitLite;
using NUnitLite.Runner;
using NUnit.Framework;

namespace MonoTouch.NUnit {
	
	static class TestRock {
		
		const string NUnitFrameworkExceptionPrefix = "NUnit.Framework.";
		const string IgnoreExceptionPrefix = NUnitFrameworkExceptionPrefix + "IgnoreException : ";
		const string IgnoreExceptionSuffix = " <" + NUnitFrameworkExceptionPrefix + "IgnoreException>";
		const string InconclusiveExceptionPrefix = NUnitFrameworkExceptionPrefix + "InconclusiveException : ";
		const string InconclusiveExceptionSuffix = " <" + NUnitFrameworkExceptionPrefix + "InconclusiveException>";
		const string SuccessExceptionPrefix = NUnitFrameworkExceptionPrefix + "SuccessException : ";
		const string SuccessExceptionSuffix =  "<" + NUnitFrameworkExceptionPrefix + "SuccessException>";
		
		static public bool IsIgnored (this TestResult result)
		{
			string m = result.Message;
			if (result.IsError)
				return m.StartsWith (IgnoreExceptionPrefix);
			if (result.IsFailure)
				return m.EndsWith (IgnoreExceptionSuffix);
			return result.Test.RunState == RunState.Ignored;
		}
		
		static public bool IsSuccess (this TestResult result)
		{
			if (result.IsInconclusive ())
			    return true;
			string m = result.Message;
			if (result.IsError)
				return m.StartsWith (SuccessExceptionPrefix);
			if (result.IsFailure)
				return m.EndsWith (SuccessExceptionSuffix);
			return result.IsSuccess;
		}
		
		static public bool IsError (this TestResult result)
		{
			if (result.IsError) {
				string m = result.Message;
				return (!m.StartsWith (InconclusiveExceptionPrefix) && !m.StartsWith (SuccessExceptionPrefix));
			}
			return result.IsError;
		}

		static public bool IsFailure (this TestResult result)
		{
			if (result.IsFailure) {
				string m = result.Message;
				if (m.EndsWith (IgnoreExceptionSuffix) || m.EndsWith (SuccessExceptionSuffix) || m.EndsWith (InconclusiveExceptionSuffix))
					return false;
			}
			return result.IsFailure;
		}
		
		static public bool IsInconclusive (this TestResult result)
		{
			string m = result.Message;
			if (result.IsError)
				return m.StartsWith (InconclusiveExceptionPrefix);
			if (result.IsFailure)
				return m.EndsWith (InconclusiveExceptionSuffix);
			return false;
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