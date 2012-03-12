using System;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Test {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class Test {
		
		[Test]
		public void Ok ()
		{
			Assert.Null (null);
			Assert.True (true);
			Assert.False (false);
		}

		[Test]
		[ExpectedException (typeof (NullReferenceException))]
		public void ExpectedExceptionException ()
		{
			string s = null;
			Assert.That (s.Length, Is.EqualTo (0), "Length");
		}
		
		[Test]
		[Ignore ("don't even execute this one")]
		public void IgnoredByAttribute ()
		{
			throw new NotImplementedException ();
		}

		[Test]
		public void IgnoreInCode ()
		{
			Assert.Ignore ("let's forget about this one");
			throw new NotImplementedException ();
		}

		[Test]
		public void InconclusiveInCode ()
		{
			Assert.Inconclusive ("it did not mean anything");
			throw new NotImplementedException ();
		}
		
		[Test]
		public void FailInCode ()
		{
			Assert.Fail ("that won't do it");
			throw new NotImplementedException ();
		}

		[Test]
		public void PassInCode ()
		{
			Assert.Pass ("good enough");
			throw new NotImplementedException ();
		}

		[Test]
		[ExpectedException (typeof (NullReferenceException))]
		public void IgnoredExpectedException ()
		{
			Assert.Ignore ("ignore [ExpectedException]");
		}

		[Test]
		[ExpectedException (typeof (NullReferenceException))]
		public void InconclusiveExpectedException ()
		{
			Assert.Inconclusive ("inconclusive [ExpectedException]");
		}

		[Test]
		[ExpectedException (typeof (NullReferenceException))]
		public void PassExpectedException ()
		{
			Assert.Pass ("pass [ExpectedException]");
		}
	}
}