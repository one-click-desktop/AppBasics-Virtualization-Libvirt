namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    /// <summary>
    /// Method of acquiring informations about domain network interface.
    /// <a href="https://libvirt.org/html/libvirt-libvirt-domain.html#VIR_DOMAIN_INTERFACE_ADDRESSES_SRC_LEASE"/>
    /// </summary>
    public enum VirDomainInterfaceAddressesSource
    {
        /// <summary>
        /// Parse DHCP lease file
        /// </summary>
        VIR_DOMAIN_INTERFACE_ADDRESSES_SRC_LEASE	=	0, //(0x0)	
        /// <summary>
        /// Query qemu guest agent
        /// </summary>
        VIR_DOMAIN_INTERFACE_ADDRESSES_SRC_AGENT	=	1, //(0x1)	
        /// <summary>
        /// Query ARP tables
        /// </summary>
        VIR_DOMAIN_INTERFACE_ADDRESSES_SRC_ARP	    =	2, //(0x2)	
        /// <summary>
        /// Sentinel
        /// </summary>
        VIR_DOMAIN_INTERFACE_ADDRESSES_SRC_LAST	    =	3 //(0x3)
    }
}