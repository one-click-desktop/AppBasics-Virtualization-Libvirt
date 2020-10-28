using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IDNT.AppBasics.Virtualization.Libvirt
{
    public class LibvirtDiskCollection : IEnumerable<LibvirtDiskDevice>
    {
        private readonly LibvirtDomain _domain;

        private ConcurrentDictionary<string, LibvirtDiskDevice> _disks =
            new ConcurrentDictionary<string, LibvirtDiskDevice>(StringComparer.OrdinalIgnoreCase);

        static private XmlSerializer _serializer = new XmlSerializer(typeof(LibvirtDiskDevice), defaultNamespace: "");

        internal LibvirtDiskCollection(LibvirtDomain domain)
        {
            _domain = domain ?? throw new ArgumentNullException("domain");
        }

        public LibvirtDiskDevice this[string name]
        {
            get { return GetByDeviceName(name); }
        }

        public LibvirtDiskDevice GetByDeviceName(string name)
        {
            LibvirtDiskDevice device;
            if (_disks.TryGetValue(name, out device))
                return device;

            var diskNode = _domain.XmlDescription.SelectSingleNode($"//domain/devices/disk//target[@dev='{name}']/parent::node()");
            if (diskNode == null)
                return null;

            return _disks.GetOrAdd(name, (id) =>
            {
                using (var reader = new XmlNodeReader(diskNode))
                    return ((LibvirtDiskDevice)_serializer.Deserialize(reader))?.Init(_domain);
            });
        }

        IEnumerator<LibvirtDiskDevice> IEnumerable<LibvirtDiskDevice>.GetEnumerator()
        {
            foreach (var node in _domain.XmlDescription.SelectNodes($"//domain/devices/disk//target/@dev"))
                yield return GetByDeviceName(((XmlAttribute)node).Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<LibvirtDiskDevice>)this).GetEnumerator();
        }
    }
}
