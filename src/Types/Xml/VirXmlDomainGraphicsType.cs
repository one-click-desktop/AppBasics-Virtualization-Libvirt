using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Libvirt
{
    [Serializable]
    public enum VirXmlDomainGraphicsType
    {
        [XmlEnum("vnc")]
        VNC,

        [XmlEnum("spice")]
        Spice,

        [XmlEnum("rdp")]
        RDP,
    }
}
