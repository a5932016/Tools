using System;
using System.Collections.Generic;
using System.Xml;

namespace SoapFuzzer
{
	public class SoapService
	{
		public string Name { get; set; }
		public List<SoapPort> Ports { get; set; }

		public SoapService (XmlNode node)
		{
			this.Name = node.Attributes["name"].Value;
			this.Ports = new List<SoapPort>();

			foreach (XmlNode port in node.ChildNodes)
				this.Ports.Add(new SoapPort(port));
		}
	}
}