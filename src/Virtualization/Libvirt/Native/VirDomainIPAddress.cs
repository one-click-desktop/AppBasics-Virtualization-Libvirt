using System.Runtime.InteropServices;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    /// <summary>
    /// Information about domain IP address
    /// <a href="https://libvirt.org/html/libvirt-libvirt-domain.html#virDomainIPAddress"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class VirDomainIPAddress
    {
        private int type;
        [MarshalAs(UnmanagedType.LPStr)] public string Addr;
        [MarshalAs(UnmanagedType.U4)] public uint Prefix;
        public VirIPAddrType Type => (VirIPAddrType) type;
    }
}