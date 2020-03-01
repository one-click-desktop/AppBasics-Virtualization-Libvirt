using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Libvirt
{
    [Serializable]
    [XmlRoot(ElementName = "source", Namespace = "")]
    public class VirXmlDomainDiskSource
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "protocol")]
        public string Protocol { get; set; }

        [XmlAttribute(AttributeName = "file")]
        public string File { get; set; }

        [XmlElement("host", IsNullable = true, Namespace = "")]
        public VirXmlDomainDiskSourceHost Host { get; set; }

        internal string GetKey()
        {
            if (!string.IsNullOrEmpty(File))
                return File;

            if (!string.IsNullOrEmpty(Protocol) && Host != null)
                return $"{Protocol}://{Host.Name}/{Name}";

            return null;
        }
    }
}
