using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace IDNT.AppBasics.Virtualization.Libvirt
{
    /// <summary>
    /// Class describes addresses of network interface attached to domain.
    /// </summary>
    public class LibvirtInterfaceAddress
    {
        public class PrefixAddress
        {
            public IPAddress Address;
            public uint Prefix;
        }
        
        public string Name { get; }
        public string HwAddress { get; }
        public List<PrefixAddress> Addresses { get; }

        public LibvirtInterfaceAddress(LibvirtInterfaceAddressCollection.VirDomainInterfaceStruct iface,
            LibvirtInterfaceAddressCollection.VirDomainIPAddressStruct[] addresses)
        {
            Name = iface.Name;
            HwAddress = iface.Hwaddr;
            Addresses = addresses.Select(
                a => new PrefixAddress() {Address = IPAddress.Parse(a.Addr), Prefix = a.Prefix}).ToList();
            
        }
    }
}