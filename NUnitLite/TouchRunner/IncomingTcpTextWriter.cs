using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#if XAMCORE_2_0
using UIKit;
#else
using MonoTouch.UIKit;
#endif

namespace MonoTouch.NUnit {

	public class IncomingTcpTextWriter : TextWriter {

		TcpListener listener;
		TcpClient client;
		StreamWriter writer;
		BlockingCollection<Tuple<SendType, object>> queue = new BlockingCollection<Tuple<SendType, object>> ();
		ManualResetEvent connected = new ManualResetEvent (false);

		public IncomingTcpTextWriter (int port)
		{
			if ((port < 0) || (port > UInt16.MaxValue))
				throw new ArgumentException ("port");
			
			Port = port;

#if __IOS__
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
#endif
			new Thread (ProcessThread) {
				IsBackground = true,
			}.Start ();

			try {
				listener = new TcpListener (IPAddress.Loopback, port);
				listener.Start ();
				listener.BeginAcceptTcpClient (ConnectionAccepted, null);
			} catch {
#if __IOS__
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
#endif
				throw;
			}
		}

		void ConnectionAccepted (IAsyncResult ar)
		{
			client = listener.EndAcceptTcpClient (ar);
			writer = new StreamWriter (client.GetStream ());
			connected.Set ();
			Console.WriteLine ("[{0}] Successful connection from {1}", DateTime.Now, client.Client.RemoteEndPoint);
		}

		void WaitForConnection ()
		{
			if (connected.WaitOne (0))
				return;
			connected.WaitOne ();
		}

		public int Port { get; private set; }

		// we override everything that StreamWriter overrides from TextWriter

		public override System.Text.Encoding Encoding {
			// hardcoded to UTF8 so make it easier on the server side
			get { return System.Text.Encoding.UTF8; }
		}

		public override void Close ()
		{
#if __IOS__
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
#endif
			Enqueue (SendType.Close);
		}

		protected override void Dispose (bool disposing)
		{
			Enqueue (SendType.Dispose);
			queue.CompleteAdding ();
		}

		public override void Flush ()
		{
			Enqueue (SendType.Flush);
		}

		void ProcessThread ()
		{
			try {
				WaitForConnection ();
				while (queue.TryTake (out var data, TimeSpan.FromDays (1))) {
					var type = data.Item1;
					var obj = data.Item2;
					switch (type) {
					case SendType.Write:
						switch (obj) {
						case char char_data:
							writer.Write (char_data);
							break;
						case string string_data:
							writer.Write (string_data);
							break;
						default:
							throw new NotImplementedException ();
						}
						break;
					case SendType.WriteLine:
						if (obj != null)
							throw new NotImplementedException ();
						writer.WriteLine ();
						writer.Flush ();
						break;
					case SendType.Flush:
						if (obj != null)
							throw new NotImplementedException ();
						writer.Flush ();
						break;
					case SendType.Dispose:
						writer.Dispose ();
						client.Dispose ();
						break;
					case SendType.Close:
						writer.Close ();
						client.Close ();
						break;
					default:
						throw new NotImplementedException ();
					}
				}
			} catch (Exception e) {
				Console.WriteLine ("Exception in IncomingTcpTextWriter:ProcessThread: {0}", e);
			}
		}

		void Enqueue (SendType type, object obj = null)
		{
			queue.Add (new Tuple<SendType, object> (type, obj));
		}

		// minimum to override - see http://msdn.microsoft.com/en-us/library/system.io.textwriter.aspx
		public override void Write (char value)
		{
			Enqueue (SendType.Write, value);
			Console.Write (value);
		}

		public override void Write (char [] buffer)
		{
			// make a string of the buffer, since the buffer contents can change
			Enqueue (SendType.Write, new string (buffer));
			Console.Write (buffer);
		}

		public override void Write (char [] buffer, int index, int count)
		{
			// make a string of the buffer, since the buffer contents can change
			Enqueue (SendType.Write, new string (buffer, index, count));
			Console.Write (buffer, index, count);
		}

		public override void Write (string value)
		{
			Enqueue (SendType.Write, value);
			Console.Write (value);
		}

		// special extra override to ensure we flush data regularly

		public override void WriteLine ()
		{
			Enqueue (SendType.WriteLine);
			Console.WriteLine ();
		}

		enum SendType {
			Write,
			WriteLine,
			Flush,
			Dispose,
			Close,
		}
	}
}
