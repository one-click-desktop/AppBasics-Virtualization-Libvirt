using System;
using System.Collections;
using System.Collections.Generic;
using IDNT.AppBasics.Virtualization.Libvirt.Native;

namespace IDNT.AppBasics.Virtualization.Libvirt
{
    /// <summary>
    /// This class describes addresses of all network interfacaes attached to domain.
    /// Its not serializable to XML.
    /// </summary>
    public class LibvirtInterfaceAddressCollection: IEnumerable<VirDomainInterfaceStruct>
    {
        private readonly LibvirtDomain _domain;

        private VirDomainInterfaceStruct[] addresses;
        public LibvirtInterfaceAddressCollection(LibvirtDomain dom)
        {
            this._domain = dom;

            int ret = NativeVirDomain.InterfaceAddresses(_domain.DomainPtr, out addresses,
                VirDomainInterfaceAddressesSource.VIR_DOMAIN_INTERFACE_ADDRESSES_SRC_AGENT);

            if (ret == -1)
                throw new LibvirtException();
        }

        #region IEnumerable<VirDomainInterfaceStruct>
        public IEnumerator<VirDomainInterfaceStruct> GetEnumerator()
        {
            foreach (VirDomainInterfaceStruct iface in addresses)
                yield return iface;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return addresses.GetEnumerator();
        }
        #endregion
    }
}