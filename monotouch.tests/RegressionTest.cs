//
// Copyright 2011-2012 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using Mono.Data.Sqlite;
using MonoTouch.AddressBookUI;
using MonoTouch.CoreAnimation;
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
		// http://bugzilla.xamarin.com/show_bug.cgi?id=234
		public void Bug234_Interlocked ()
		{
			string str = null;
			Assert.Null (Interlocked.Exchange (ref str, "one"), "Exchange");
			// the above should not crash with System.ExecutionEngineException
			Assert.That (str, Is.EqualTo ("one"), "one");
		}
		
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=300
		// http://stackoverflow.com/questions/6517736/monotouch-crash-dictionary-firstordefault-type-initializer-predicateof
		public void Bug300_Linker_PredicateOf ()
		{
			Dictionary<string, DateTime> queued = new Dictionary<string, DateTime> ();
			KeyValuePair<string, DateTime> valuePair = queued.FirstOrDefault ();
			// above should not crash with System.ExecutionEngineException
			Assert.NotNull (valuePair);
		}

		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=328
		public void Bug328_CompletionBlock ()
		{
			CATransaction.Begin ();
			CATransaction.CompletionBlock = delegate {};
			// the above should not crash with a MonoTouchException
			CATransaction.Commit ();
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

		void CheckExceptionDetailProperty (PropertyInfo pi)
		{
			bool data_member = true;
			foreach (var ca in pi.GetCustomAttributes (false)) {
				if (ca is DataMemberAttribute) {
					data_member = true;
					break;
				}
			}
				// to be valid both getter and setter must be present if [DataMember]
			if (data_member) {
				Assert.NotNull (pi.GetGetMethod (true), "get_" + pi.Name);
				Assert.NotNull (pi.GetSetMethod (true), "set_" + pi.Name);
			} else {
				// check well-known [DataMember]
				switch (pi.Name) {
				case "HelpLink":
				case "InnerException":
				case "Message":
				case "StackTrace":
				case "Type":
					Assert.Fail ("{0} is missing it's [DataMember] attribute", pi.Name);
					break;
				}
			}
		}
		
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1415
		public void Bug1415_Linker_DataMember ()
		{
			// the typeof ensure we're can't (totally) link away System.ServiceModel.dll
			Type ed = typeof (System.ServiceModel.AuditLevel).Assembly.GetType ("System.ServiceModel.ExceptionDetail", false);
			// which means it's [DataContract] / [DataMember] should not be linked out
			// even if we're not specifically using them (and without [Preserve] being added)
			// which is important since System.ServiceModel.dll is an SDK assembly
			Assert.NotNull (ed, "ExceptionDetail");
			bool help_link = false;
			bool inner_exception = false;
			bool message = false;
			bool stack_trace = false;
			bool type = false;
			foreach (var pi in ed.GetProperties ()) {
				CheckExceptionDetailProperty (pi);
				switch (pi.Name) {
				case "HelpLink":
					help_link = true;
					break;
				case "InnerException":
					inner_exception = true;
					break;
				case "Message":
					message = true;
					break;
				case "StackTrace":
					stack_trace = true;
					break;
				case "Type":
					type = true;
					break;
				}
			}
			// ensure all properties are still present
			Assert.True (help_link, "HelpLink");
			Assert.True (inner_exception, "InnerException");
			Assert.True (message, "Message");
			Assert.True (stack_trace, "StackTrace");
			Assert.True (type, "Type");
		}
		
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1415
		// not really part of the bug - but part of the same fix
		public void Bug1415_Linker_XmlAttribute ()
		{
			// the typeof ensure we're can't (totally) link away System.ServiceModel.dll
			Type ed = typeof (System.ServiceModel.AuditLevel).Assembly.GetType ("System.ServiceModel.EndpointAddress10", false);
			// type is decorated with both [XmlSchemaProvider] and [XmlRoot]
			Assert.NotNull (ed, "EndpointAddress10");
			
			var q = new OpenTK.Quaternion ();
			Assert.Null (q.GetType ().GetProperty ("XYZ"), "XmlIgnore");
			// should be null if application is linked (won't be if "Don't link" is used)
		}

		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1516
		public void Bug1516_Appearance_Linker ()
		{
			UINavigationBar.Appearance.TintColor = UIColor.FromRGB (238,234,222);
			UINavigationBar.Appearance.SetTitleTextAttributes (new UITextAttributes() {
				TextColor = UIColor.FromRGB(85, 108, 17),
				TextShadowColor = UIColor.Clear
			});
			// show not throw if the application is linked
		}
	}
}