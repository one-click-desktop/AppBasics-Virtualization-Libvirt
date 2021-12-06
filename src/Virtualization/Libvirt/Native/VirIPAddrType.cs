namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    /// <summary>
    /// Type of domain IP Address
    /// <a href="https://libvirt.org/html/libvirt-libvirt-network.html#virIPAddrType"/>
    /// </summary>
    public enum VirIPAddrType
    {
        /// <summary>
        /// IPv4 address
        /// </summary>
        VIR_IP_ADDR_TYPE_IPV4 = 0,//0x0
        /// <summary>
        /// IPv6 address
        /// </summary>
        VIR_IP_ADDR_TYPE_IPV6 =	1,//0x1
        /// <summary>
        /// Sentinel
        /// </summary>
        VIR_IP_ADDR_TYPE_LAST =	2//0x2
    }
}