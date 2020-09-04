using System;
using System.Collections.Generic;
using System.Xml;

namespace SoapFuzzer
{
	public class SoapType
	{
		public string Name { get; set; }
		public List<SoapTypeParameter> Parameters { get; set; }

		public SoapType (XmlNode type)
		{
			this.Name = type.Attributes["name"].Value;
			this.Parameters = new List<SoapTypeParameter>();

			if (type.HasChildNodes && type.FirstChild.HasChildNodes) {
				foreach (XmlNode node in type.FirstChild.FirstChild.ChildNodes)
					this.Parameters.Add(new SoapTypeParameter(node));
	        }
		}
	}
}