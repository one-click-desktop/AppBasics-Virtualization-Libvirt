using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Libvirt
{
    [Serializable]
    public enum VirXmlDomainDiskDevice
    {
        [XmlEnum("disk")]
        Disk,


        [XmlEnum("cdrom")]
        CDROM
    }
}
