// NUnitOutputTextWriter.cs
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
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
using System.IO;
using System.Text;

using NUnitLite.Runner;
using MonoTouch.NUnit.UI;

namespace MonoTouch.NUnit {

	public class NUnitOutputTextWriter : TextWriter {

		bool real_time_reporting;
		StringBuilder extra_data = new StringBuilder ();

		public NUnitOutputTextWriter (BaseTouchRunner runner, TextWriter baseWriter, OutputWriter xmlWriter)
		{
			Runner = runner;
			BaseWriter = baseWriter ?? Console.Out;
			XmlOutputWriter = xmlWriter;
			// do not send real-time test results on the writer sif XML reports are enabled
			real_time_reporting = (xmlWriter == null);
		}

		public override Encoding Encoding {
			get { return Encoding.UTF8; }
		}

		public TextWriter BaseWriter { get; private set; }

		public BaseTouchRunner Runner { get; private set; }

		public OutputWriter XmlOutputWriter { get; private set; }

		public override void Write (char value)
		{
			if (real_time_reporting)
				BaseWriter.Write (value);
			else
				extra_data.Append (value);
		}

		public override void Write (string value)
		{
			if (real_time_reporting)
				BaseWriter.Write (value);
			else
				extra_data.Append (value);
		}

		public override void Close ()
		{
			if (XmlOutputWriter != null) {
				// now we want the XML report to write
				real_time_reporting = true;
				// write to a temporary string, because NUnit2XmlOutputWriter.WriteResultFile closes the stream,
				// and we need to write more things to it.
				using (var textWriter = new StringWriter ()) {
					XmlOutputWriter.WriteResultFile (Runner.Result, textWriter);
					BaseWriter.WriteLine (textWriter.ToString ());
				}
				if (extra_data.Length > 0) {
					BaseWriter.WriteLine ("<!-- extra output\n ");
					BaseWriter.WriteLine (extra_data);
					BaseWriter.WriteLine ("-->");
				}
				BaseWriter.WriteLine ("<!-- the end -->");
				BaseWriter.Close ();
				real_time_reporting = false;
			}
			base.Close ();
		}
	}
}