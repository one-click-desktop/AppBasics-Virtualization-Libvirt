using System.Runtime.InteropServices;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    ///<summary>
    /// Domain interface informations
    /// <a href="https://libvirt.org/html/libvirt-libvirt-domain.html#virDomainInterface"/>
    ///</summary>
    [StructLayout(LayoutKind.Sequential)]
    public class VirDomainInterfaceStruct
    {
        /// <summary>
        /// Interface name
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)] public string Name;
        /// <summary>
        /// Hardware address, may be NULL
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)] public string Hwaddr;
        /// <summary>
        /// Number of items in <c>addrs</c>
        /// </summary>
        [MarshalAs(UnmanagedType.U4)] public uint Naddrs;
        /// <summary>
        /// Array of IP addresses
        /// </summary>
        [MarshalAs(UnmanagedType.LPArray)] public VirDomainIPAddress[] Addrs;
    }
}