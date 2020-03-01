using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Libvirt
{
    [Serializable]
    public enum VirXmlDomainDiskType
    {
        [XmlEnum("file")]
        File,

        [XmlEnum("network")]
        Network
    }
}
