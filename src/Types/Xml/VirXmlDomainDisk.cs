using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Libvirt
{
    [Serializable]
    [XmlRoot(ElementName = "disk", Namespace = "")]
    public class VirXmlDomainDisk
    {
        [XmlAttribute(AttributeName = "type")]
        public VirXmlDomainDiskType Type { get; set; }

        [XmlAttribute(AttributeName = "device")]
        public VirXmlDomainDiskDevice Device { get; set; }

        [XmlElement(ElementName = "source", IsNullable = false, Namespace = "")]
        public VirXmlDomainDiskSource Source { get; set; }

        [XmlElement("readonly", IsNullable = true)]
        private string _readonly { get; set; }

        [XmlIgnore]
        public bool IsReadOnly {  get { return _readonly != null; } }
    }
}
