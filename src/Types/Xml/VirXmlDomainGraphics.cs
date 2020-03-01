using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Libvirt
{
    [Serializable]
    [XmlRoot(ElementName = "graphics", Namespace = "")]
    public class VirXmlDomainGraphics
    {
        [XmlAttribute(AttributeName = "type")]
        public VirXmlDomainGraphicsType Type { get; set; }

        [XmlAttribute(AttributeName = "listen")]
        public string Listen { get; set; }

        [XmlAttribute(AttributeName = "port")]
        public int Port { get; set; }

        [XmlAttribute(AttributeName = "autoport")]
        private string _autoport { get; set; }

        [XmlIgnore]
        public bool IsAutoPort {  get { return string.Equals("yes", _autoport); } }
    }
}
