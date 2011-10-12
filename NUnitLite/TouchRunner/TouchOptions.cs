// TouchOptions.cs: MonoTouch.Dialog-based options
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011 Xamarin Inc. All rights reserved

using System;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace NUnitLite {
	
	public class TouchOptions {
		
		public TouchOptions ()
		{
			var defaults = NSUserDefaults.StandardUserDefaults;
			EnableNetwork = defaults.BoolForKey ("network.enabled");
			HostName = defaults.StringForKey ("network.host.name");
			HostPort = defaults.IntForKey ("network.host.port");
		}
		
		private bool EnableNetwork { get; set; }
		
		public string HostName { get; private set; }
		
		public int HostPort { get; private set; }
		
		public bool ShowUseNetworkLogger {
			get { return (EnableNetwork && !String.IsNullOrWhiteSpace (HostName) && (HostPort > 0)); }
		}
		
		public UIViewController GetViewController ()
		{
			var network = new BooleanElement ("Remote Server", EnableNetwork);

			var host = new EntryElement ("Host Name", "name", HostName);
			host.KeyboardType = UIKeyboardType.ASCIICapable;
			
			var port = new EntryElement ("Port", "name", HostPort.ToString ());
			port.KeyboardType = UIKeyboardType.NumberPad;
			
			var root = new RootElement ("Options") {
				new Section () { network, host, port }
			};
				
			var dv = new DialogViewController (root, true) { Autorotate = true };
			dv.ViewDissapearing += delegate {
				EnableNetwork = network.Value;
				HostName = host.Value;
				ushort p;
				if (UInt16.TryParse (port.Value, out p))
					HostPort = p;
				else
					HostPort = -1;
				
				var defaults = NSUserDefaults.StandardUserDefaults;
				defaults.SetBool (EnableNetwork, "network.enabled");
				defaults.SetString (HostName ?? String.Empty, "network.host.name");
				defaults.SetInt (HostPort, "network.host.port");
			};
			
			return dv;
		}
	}
}