using System;
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
		ManualResetEvent connected = new ManualResetEvent (false);

		public IncomingTcpTextWriter (int port)
		{
			if ((port < 0) || (port > UInt16.MaxValue))
				throw new ArgumentException ("port");
			
			Port = port;

#if __IOS__
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
#endif

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
			WaitForConnection ();
			writer.Close ();
			client.Close ();
		}

		protected override void Dispose (bool disposing)
		{
			WaitForConnection ();
			writer.Dispose ();
			client.Dispose ();
		}

		public override void Flush ()
		{
			WaitForConnection ();
			writer.Flush ();
		}

		// minimum to override - see http://msdn.microsoft.com/en-us/library/system.io.textwriter.aspx
		public override void Write (char value)
		{
			WaitForConnection ();
			writer.Write (value);
		}

		public override void Write (char [] buffer)
		{
			WaitForConnection ();
			writer.Write (buffer);
		}

		public override void Write (char [] buffer, int index, int count)
		{
			WaitForConnection ();
			writer.Write (buffer, index, count);
		}

		public override void Write (string value)
		{
			WaitForConnection ();
			writer.Write (value);
		}

		// special extra override to ensure we flush data regularly

		public override void WriteLine ()
		{
			WaitForConnection ();
			writer.WriteLine ();
			writer.Flush ();
		}
	}
}
