using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Libvirt
{
    [Serializable]
    [XmlRoot(ElementName = "host", Namespace = "")]
    public class VirXmlDomainDiskSourceHost
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "port")]
        public int Port { get; set; }

    }
}
