using System;
using System.Collections.Generic;
using System.Xml;

namespace SoapFuzzer
{
	public class SoapPortType
	{
		public string Name { get; set; }
		public List<SoapOperation> Operations { get; set; }

		public SoapPortType (XmlNode node)
		{
			this.Name = node.Attributes ["name"].Value;
			this.Operations = new List<SoapOperation>();
			foreach (XmlNode op in node.ChildNodes) 
				this.Operations.Add(new SoapOperation(op));
		}
	}
}

