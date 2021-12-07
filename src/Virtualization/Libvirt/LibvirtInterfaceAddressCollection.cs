using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using IDNT.AppBasics.Virtualization.Libvirt.Native;

namespace IDNT.AppBasics.Virtualization.Libvirt
{
    /// <summary>
    /// This class describes addresses of all network interfacaes attached to domain.
    /// Its not serializable to XML.
    /// </summary>
    public class LibvirtInterfaceAddressCollection: IEnumerable<LibvirtInterfaceAddress>
    {
        #region Native structures
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
            public string Name;
            /// <summary>
            /// Hardware address, may be NULL
            /// </summary>
            public string Hwaddr;
            /// <summary>
            /// Number of items in <c>addrs</c>
            /// </summary>
            public uint Naddrs;
            /// <summary>
            /// Array of IP addresses
            /// </summary>
            public IntPtr Addrs;
        }
        
        /// <summary>
        /// Information about domain IP address
        /// <a href="https://libvirt.org/html/libvirt-libvirt-domain.html#virDomainIPAddress"/>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class VirDomainIPAddressStruct
        {
            /// <summary>
            /// Type of address - IPv4 or IPv6
            /// </summary>
            public VirIPAddrType type;
            /// <summary>
            /// IP address - human readable format(4 octests divised by 3 dots)
            /// </summary>
            public string Addr;
            /// <summary>
            /// Prefix - netmask in prefix format
            /// </summary>
            public uint Prefix;
        }
        #endregion
        
        #region Native functions
        /// <summary>
        /// Return a pointer to the allocated array of pointers to interfaces present in given domain along with their IP and MAC addresses.
        /// Note that single interface can have multiple or even 0 IP addresses.
        /// This API dynamically allocates the virDomainInterfacePtr struct based on how many interfaces domain dom has,
        /// usually there's 1:1 correlation. The count of the interfaces is returned as the return value.
        /// </summary>
        /// <param name="dom">A <see cref="IntPtr"/>pointer to the domain object.</param>
        /// <param name="ifaces">Dynamically allocated array of <see cref="VirDomainInterfaceStruct"/> with interfaces info.</param>
        /// <param name="source">Method of getting informations about interfaces. <see cref=""/></param>
        /// <param name="flags">Must be 0</param>
        /// <returns>The number of interfaces on success, -1 in case of error.</returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "virDomainInterfaceAddresses")]
        private static extern int virDomainInterfaceAddresses(IntPtr dom, [Out] out IntPtr ifaces, uint source, uint flags);
        /// <summary>
        /// Free the interface object. The data structure is freed and should not be used thereafter. If iface is NULL, then this method has no effect.
        /// </summary>
        /// <param name="iface">Pointer to the InterfaceObject.</param>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "virDomainInterfaceFree")]
        private static extern void virDomainInterfaceFree(IntPtr iface);
        #endregion
        
        private readonly LibvirtDomain _domain;
        private List<LibvirtInterfaceAddress> addresses;
        public LibvirtInterfaceAddressCollection(LibvirtDomain dom)
        {
            this._domain = dom;
            addresses = new List<LibvirtInterfaceAddress>();
            
            //Get address info by native code calls
            IntPtr ifacesPtr;
            int ret = virDomainInterfaceAddresses(_domain.DomainPtr, out ifacesPtr,
                (uint)VirDomainInterfaceAddressesSource.VIR_DOMAIN_INTERFACE_ADDRESSES_SRC_AGENT, 0);
            if (ret < 0)
                ret = virDomainInterfaceAddresses(_domain.DomainPtr, out ifacesPtr,
                    (uint)VirDomainInterfaceAddressesSource.VIR_DOMAIN_INTERFACE_ADDRESSES_SRC_LEASE, 0);
            if (ret < 0)
                throw new LibvirtException();
            
            //Create ArrayOfPointers to ifaces
            IntPtr[] ifaces = new IntPtr[ret];
            for (int i = 0; i < ret; ++i)
            {
                ifaces[i] = Marshal.ReadIntPtr(ifacesPtr, i * Marshal.SizeOf(ifacesPtr));
            }
            
            //Map memory to structs
            foreach (IntPtr ifacePtr in ifaces)
            {
                VirDomainInterfaceStruct iface = new VirDomainInterfaceStruct();
                Marshal.PtrToStructure(ifacePtr, iface);
                
                VirDomainIPAddressStruct[] addrs = new VirDomainIPAddressStruct[iface.Naddrs];
                for (int j = 0; j < iface.Naddrs; ++j)
                {
                    VirDomainIPAddressStruct addr = new VirDomainIPAddressStruct();
                    IntPtr addrPtr = IntPtr.Add(iface.Addrs, j * Marshal.SizeOf(addr));
                    Marshal.PtrToStructure(addrPtr, addr);
                    addrs[j] = addr;
                }
                
                //Create managed class
                addresses.Add(new LibvirtInterfaceAddress(iface, addrs));
                
                //Free data aquired from native call
                //smogork: i Don't know if it is neccessary, but i would do it just to be sure
                virDomainInterfaceFree(ifacePtr);
            }
        }

        #region IEnumerable<LibvirtInterfaceAddress>
        public IEnumerator<LibvirtInterfaceAddress> GetEnumerator()
        {
            foreach (LibvirtInterfaceAddress iface in addresses)
                yield return iface;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return addresses.GetEnumerator();
        }
        #endregion
    }
}