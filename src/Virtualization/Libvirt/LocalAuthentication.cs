using IDNT.AppBasics.Virtualization.Libvirt.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDNT.AppBasics.Virtualization.Libvirt
{
    public class LocalAuthentication : LibvirtAuthentication
    {
        internal override IntPtr Connect(string uri, LibvirtConfiguration configuration)
        {
            return NativeVirConnect.Open(uri);
        }
    }
}
