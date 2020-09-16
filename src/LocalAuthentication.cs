using Libvirt.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libvirt
{
    public class LocalAuthentication : LibvirtAuthentication
    {
        internal override IntPtr Connect(string uri, LibvirtConfiguration configuration)
        {
            return NativeVirConnect.Open(uri);
        }
    }
}
