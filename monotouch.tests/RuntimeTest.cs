using System;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.ObjCRuntime {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RuntimeTest {
		
		[Test]
		public void GetNSObject_IntPtrZero ()
		{
			Assert.Null (Runtime.GetNSObject (IntPtr.Zero));
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void RegisterAssembly_null ()
		{
			Runtime.RegisterAssembly (null);
		}
	}
}