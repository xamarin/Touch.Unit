// HttpTextWriter.cs: Class to report test results using http requests
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2016 Xamarin Inc.
//

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if __UNIFIED__
using Foundation;
#else
using MonoTouch.Foundation;
#endif

namespace MonoTouch.NUnit {
	class HttpTextWriter : TextWriter
	{
		public string HostName;
		public int Port;

		TaskCompletionSource<bool> finished = new TaskCompletionSource<bool> ();
		TaskCompletionSource<bool> closed = new TaskCompletionSource<bool> ();
		StringBuilder log = new StringBuilder ();

		public Task FinishedTask {
			get {
				return finished.Task;
			}
		}

		public override Encoding Encoding {
			get {
				return Encoding.UTF8;
			}
		}

		public override void Close ()
		{
			closed.SetResult (true);
			Task.Run (async () =>
			{
				await finished.Task;
				base.Close ();
			});
		}

		Task SendData (string action, string uploadData)
		{
			var url = NSUrl.FromString ("http://" + HostName + ":" + Port + "/" + action);
			var request = new NSMutableUrlRequest (url);
			request.HttpMethod = "POST";
			return NSUrlSession.SharedSession.CreateUploadTaskAsync (request, NSData.FromString (uploadData));
		}

		async void SendThread ()
		{
			try {
				await SendData ("Start", "");
				await closed.Task;
				await SendData ("Finish", log.ToString ());
			} catch (Exception ex) {
				Console.WriteLine ("HttpTextWriter failed: {0}", ex);
			} finally {
				finished.SetResult (true);				
			}
		}

		public void Open ()
		{
			new Thread (SendThread)
			{
				IsBackground = true,
			}.Start ();
		}

		public override void Write (char value)
		{
			Console.Out.Write (value);
			log.Append (value);
		}

		public override void Write (char [] buffer)
		{
			Console.Out.Write (buffer);
			log.Append (buffer);
		}
	
		public override void WriteLine (string value)
		{
			Console.Out.WriteLine (value);
			log.AppendLine (value);
		}
	}
}
