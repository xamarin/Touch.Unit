// Main.cs: Touch.Unit Simple Server
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011 Xamarin Inc. All rights reserved

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Mono.Options;

// a simple, blocking (i.e. one device/app at the time), listener
class SimpleListener {

	static byte[] buffer = new byte [16 * 1024];

	IPAddress Address { get; set; }
	int Port { get; set; }
	string LogPath { get; set; }

	public void Start ()
	{
		Console.WriteLine ("Touch.Unit Simple Server listening on: {0}:{1}", Address, Port);
		TcpListener server = new TcpListener (Address, Port);
		try {
			server.Start ();

			while (true) {
				using (TcpClient client = server.AcceptTcpClient ()) {
					Processing (client);
				}
			}
		}
		catch (Exception e) {
			Console.WriteLine ("[{0}] : {1}", DateTime.Now, e);
		}
		finally {
			server.Stop ();
		}
	}

	public void Processing (TcpClient client)
	{
		string logfile = Path.Combine (LogPath, DateTime.UtcNow.Ticks.ToString () + ".log");
		string remote = client.Client.RemoteEndPoint.ToString ();
		Console.WriteLine ("Connection from {0} saving logs to {1}", remote, logfile);

		using (FileStream fs = File.OpenWrite (logfile)) {
			// a few extra bits of data only available from this side
			string header = String.Format ("[Local Date/Time:\t{1}]{0}[Remote Address:\t{2}]{0}", 
				Environment.NewLine, DateTime.Now, remote);
			byte[] array = Encoding.UTF8.GetBytes (header);
			fs.Write (array, 0, array.Length);
			fs.Flush ();
			// now simply copy what we receive
			int i;
			NetworkStream stream = client.GetStream ();
			while ((i = stream.Read (buffer, 0, buffer.Length)) != 0) {
				fs.Write (buffer, 0, i);
				fs.Flush ();
			}
		}
	}

	static void ShowHelp (OptionSet os)
	{
		Console.WriteLine ("Usage: mono Touch.Server.exe [options]");
		os.WriteOptionDescriptions (Console.Out);
	}

	public static void Main (string[] args)
	{ 
		Console.WriteLine ("Touch.Unit Simple Server");
		Console.WriteLine ("Copyright 2011, Xamarin Inc. All rights reserved.");
		
		bool help = false;
		string address = null;
		string port = null;
		string log_path = ".";
		
		var os = new OptionSet () {
			{ "h|?|help", "Display help", v => help = true },
			{ "ip", "IP address to listen (default: Any)", v => address = v },
			{ "port", "TCP port to listen (default: 16384)", v => port = v },
			{ "logpath", "Path to save the log files (default: .)", v => log_path = v }
		};
		
		try {
			os.Parse (args);
			if (help)
				ShowHelp (os);
			
			var listener = new SimpleListener ();
			
			IPAddress ip;
			if (String.IsNullOrEmpty (address) || !IPAddress.TryParse (address, out ip))
				listener.Address = IPAddress.Any;
			
			ushort p;
			if (UInt16.TryParse (port, out p))
				listener.Port = p;
			else
				listener.Port = 16384;
			
			listener.LogPath = log_path ?? ".";

			listener.Start ();
		} catch (OptionException oe) {
			Console.WriteLine ("{0} for options '{1}'", oe.Message, oe.OptionName);
		}
	}   
}