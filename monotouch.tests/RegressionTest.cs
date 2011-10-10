using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using Mono.Data.Sqlite;
using MonoTouch.AddressBookUI;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using NUnit.Framework;

namespace MonoTouchFixtures {
	
	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class RegressionTest {
		
		[Test]
		// https://github.com/xamarin/monotouch/commit/cbefbeaea2eda820dfc7214e976edc83a55df38e
		public void MonoAssembly_LinkedOut ()
		{
			Assembly a = Assembly.GetExecutingAssembly ();
			Assert.That (a.GetType ().Name, Is.EqualTo ("MonoAssembly"), "MonoAssembly");
		}
		
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=205
		// https://bugzilla.novell.com/show_bug.cgi?id=688414
		public void Bug205_ExposingIEnumerable ()
		{
			var ds = new DataContractSerializer (typeof (IEnumerable<int>));
			using (var xw = XmlWriter.Create (System.IO.Stream.Null))
				ds.WriteObject (xw, new int [] { 1, 2, 3 });
			// the above should not throw System.Runtime.Serialization.SerializationException
		}
		
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=233
		public void Bug233_MonoPInvokeCallback ()
		{
			var c = new SqliteConnection ("Data Source=:memory:");
			c.Open ();
			c.Update += (sender, e) => {};
			// the above should not crash
			c.Close ();
		}
		
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=769
		public void Bug769_UnregistredDelegate ()
		{
			Assert.NotNull (new MKMapViewDelegate ());
			// the above should not throw an Exception
		}

		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=865
		public void Bug865_CanOpenUrl ()
		{
			Assert.False (UIApplication.SharedApplication.CanOpenUrl (null), "null");
			// the above should not throw an ArgumentNullException
			// and that's important because NSUrl.FromString and NSUrl.ctor(string) differs
			const string bad_tel = "tel://1800 023 009";
			Assert.Null (NSUrl.FromString (bad_tel), "bad url"); 
			NSUrl url = new NSUrl (bad_tel);
			Assert.NotNull (url, "ctor, bad url");
			Assert.That (url.Handle, Is.EqualTo (IntPtr.Zero), "null handle");
		}
		
		[Test]
		// issue indirectly found when trying:  http://bugzilla.xamarin.com/show_bug.cgi?id=928
		// similar to MonoAssembly_LinkedOut
		// https://github.com/xamarin/monotouch/commit/409316f87f23723a384cb072163abd03ae7e6045
		public void Bug928_MonoModule_LinkedOut ()
		{
			Module m = Assembly.GetExecutingAssembly ().ManifestModule;
			Assert.That (m.GetType ().Name, Is.EqualTo ("MonoModule"), "MonoModule");
		}

		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=980
		public void Bug980_AddressBook_NRE ()
		{
			ABPeoplePickerNavigationController picker = new ABPeoplePickerNavigationController ();
			Assert.NotNull (picker.AddressBook); // no NRE should occur
		}
		
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1144
		[ExpectedException (typeof (NotSupportedException))]
		public void Bug1144_LinkedAway ()
		{
			Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP); 
			IAsyncResult ias = socket.BeginConnect (IPAddress.Loopback, 4201, null, null);
			ias.AsyncWaitHandle.WaitOne (15, true);
			// emitContext == true should behave identically whether the app is linked (throws) or not (bug)
		}

		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1387
		public void Bug1387_UIEdgeInsets_ToString ()
		{
			var insets = new MonoTouch.UIKit.UIEdgeInsets (1, 2, 3, 4);
			Assert.That (insets.ToString (), Is.Not.EqualTo ("MonoTouch.UIKit.UIEdgeInsets"));
		}
	}
}