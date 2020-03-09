/*
 * Libvirt-dotnet
 * 
 * Copyright 2020 IDNT (https://www.idnt.net) and Libvirt-dotnet contributors.
 * 
 * This project incorporates work by the following original authors and contributors
 * to libvirt-csharp:
 *    
 *    Copyright (C) 
 *      Arnaud Champion <arnaud.champion@devatom.fr>
 *      Jaromír Červenka <cervajz@cervajz.com>
 *
 * Licensed under the GNU Lesser General Public Library, Version 2.1 (the "License");
 * you may not use this file except in compliance with the License. You may obtain a 
 * copy of the License at
 *
 * https://www.gnu.org/licenses/lgpl-2.1.en.html
 * 
 * or see LICENSE for a copy of the license terms. Unless required by applicable 
 * law or agreed to in writing, software distributed under the License is distributed 
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express 
 * or implied. See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Libvirt
{
    /// <summary>
    /// Represents a libvirt storage pool
    /// </summary>
    public class LibvirtStoragePool : IDisposable
    {
        private readonly LibvirtConnection _conn;
        private readonly IntPtr _poolPtr;
        private XmlDocument _xmlDescription = null;
        private readonly object _xmlDescrLock = new object();

        internal LibvirtStoragePool(LibvirtConnection connection, Guid uniqueId, IntPtr poolPtr)
        {
            _conn = connection ?? throw new ArgumentNullException("connection");
            if (Guid.Empty.Equals(uniqueId))
                throw new ArgumentNullException("uuid");
            UniqueId = uniqueId;
            if (poolPtr == IntPtr.Zero)
                throw new ArgumentNullException("poolPtr");
            _poolPtr = poolPtr;
        }

        #region Properties
        /// <summary>
        /// Unique domain identifier
        /// </summary>
        public Guid UniqueId { get; private set; }

        /// <summary>
        /// True if the domain is currently active (running).
        /// </summary>
        public bool IsActive { get { return NativeVirStoragePool.IsActive(_poolPtr) == 1;  } }

        /// <summary>
        /// The domains human readable name
        /// </summary>
        public string Name {  get { return NativeVirStoragePool.GetName(_poolPtr); } }

        /// <summary>
        /// Get storage pool state
        /// </summary>
        public VirStoragePoolState State
        {
            get
            {
                VirStoragePoolInfo poolInfo = new VirStoragePoolInfo();
                if (NativeVirStoragePool.GetInfo(_poolPtr, ref poolInfo) < 0)
                    throw new LibvirtQueryException();
                return poolInfo.State;
            }
        }

        /// <summary>
        /// Get pool capacity
        /// </summary>
        public ulong CapacityInByte
        {
            get
            {
                VirStoragePoolInfo poolInfo = new VirStoragePoolInfo();
                if (NativeVirStoragePool.GetInfo(_poolPtr, ref poolInfo) < 0)
                    throw new LibvirtQueryException();
                return poolInfo.Capacity;
            }
        }

        /// <summary>
        /// Get available free byte
        /// </summary>
        public ulong ByteAvailable
        {
            get
            {
                VirStoragePoolInfo poolInfo = new VirStoragePoolInfo();
                if (NativeVirStoragePool.GetInfo(_poolPtr, ref poolInfo) < 0)
                    throw new LibvirtQueryException();
                return poolInfo.Available;
            }
        }

        /// <summary>
        /// Get allocated byte
        /// </summary>
        public ulong ByteAllocated
        {
            get
            {
                VirStoragePoolInfo poolInfo = new VirStoragePoolInfo();
                if (NativeVirStoragePool.GetInfo(_poolPtr, ref poolInfo) < 0)
                    throw new LibvirtQueryException();
                return poolInfo.Allocation;
            }
        }

        /// <summary>
        /// Returns the libvirt driver for this domain (qemu|kvm|esx|hyperv|...)
        /// </summary>
        public string DriverType
        {
            get { return XmlDescription.DocumentElement.Attributes["type"].Value; }
        }

        /// <summary>
        /// Returns the path to the store pool root including the protocol used to access the path
        /// </summary>
        public string GetPath()
        {
            var descr = XmlDescription;

            switch(DriverType)
            {
                case "dir":
                    var dirNode = XmlDescription.SelectSingleNode("//target");
                    var targetSerializer = new XmlSerializer(typeof(VirXmlStoragePoolTarget), defaultNamespace: "");
                    using (var reader = new XmlNodeReader(dirNode))
                        return ((VirXmlStoragePoolTarget)targetSerializer.Deserialize(reader)).GetPath("file");

                case "gluster":
                    var sourceNode = XmlDescription.SelectSingleNode("//source");
                    var sourceSerializer = new XmlSerializer(typeof(VirXmlStoragePoolSource), defaultNamespace: "");
                    using (var reader = new XmlNodeReader(sourceNode))
                        return ((VirXmlStoragePoolSource)sourceSerializer.Deserialize(reader)).GetPath(DriverType);
                default:
                    throw new LibvirtNotImplementedException($"Can't determine path of storage pool ${Name} with driver {DriverType}.");
            }

        }

        #endregion

        #region Configuration
        public XmlDocument XmlDescription
        {
            get
            {
                XmlDocument document;
                lock (_xmlDescrLock)
                    document = _xmlDescription;

                if (document == null)
                {
                    lock (_xmlDescrLock)
                    {
                        if (_xmlDescription == null)
                        {
                            string xmlText = NativeVirStoragePool.GetXMLDesc(_poolPtr, 
                                VirDomainXMLFlags.VIR_DOMAIN_XML_SECURE);
                            if (string.IsNullOrWhiteSpace(xmlText))
                                throw new LibvirtQueryException();
                            _xmlDescription = new XmlDocument();
                            _xmlDescription.LoadXml(xmlText);
                        }
                        document = _xmlDescription;
                    }
                }
                return document;
            }
        }
        #endregion

        #region Volumes
        /// <summary>
        /// Queries volumes
        /// </summary>
        /// <returns>List of volumes</returns>
        public IEnumerable<LibvirtStorageVolume> ListVolumes()
        {
            int nbVolumes = NativeVirStoragePool.NumOfVolumes(_poolPtr);
            string[] volumeNames = new string[nbVolumes];
            if (NativeVirStoragePool.ListVolumes(_poolPtr, ref volumeNames, nbVolumes) < 0)
                throw new LibvirtQueryException();

            foreach (IntPtr volumePtr in volumeNames.Select(volumeName => NativeVirStorageVol.LookupByName(_poolPtr, volumeName)))
            {
                try
                {
                    string keyString = NativeVirStorageVol.GetKey(volumePtr);
                    if (string.IsNullOrEmpty(keyString))
                        throw new LibvirtQueryException();

                    yield return _conn.VolumeCache.GetOrAdd(keyString, (id) =>
                    {
                        NativeVirStorageVol.Ref(volumePtr);
                        return new LibvirtStorageVolume(_conn, UniqueId, keyString, volumePtr);
                    });
                }
                finally
                {
                    NativeVirStorageVol.Free(volumePtr);
                }
            }
        }
        
        /// <summary>
        /// Get a volume by name
        /// </summary>
        /// <param name="volumeName">Volume name</param>
        /// <returns>Volume or NULL</returns>
        public LibvirtStorageVolume GetVolumeByName(string volumeName)
        {
            var volumePtr = NativeVirStorageVol.LookupByName(_poolPtr, volumeName);
            if (volumePtr == IntPtr.Zero)
            {
                Trace.WriteLine($"Could not find volume with name '{volumeName}'.");
                return null;
            }

            try
            {
                string keyString = NativeVirStorageVol.GetKey(volumePtr);
                if (string.IsNullOrEmpty(keyString))
                    throw new LibvirtQueryException();

                return _conn.VolumeCache.GetOrAdd(keyString, (id) =>
                {
                    NativeVirStorageVol.Ref(volumePtr);
                    return new LibvirtStorageVolume(_conn, UniqueId, keyString, volumePtr);
                });
            }
            finally
            {
                NativeVirStorageVol.Free(volumePtr);
            }
        }

        /// <summary>
        /// Enumerates all running as well as defined domains
        /// </summary>
        public IEnumerable<LibvirtStorageVolume> Volumes
        {
            get { return ListVolumes(); }
        }
        #endregion

        #region Events
        internal void DispatchStoragePoolEvent(VirStoragePoolLifecycleEventArgs args)
        {
            if (Thread.VolatileRead(ref _isDisposing) != 0)
                return;
        }

        internal void DispatchStoragePoolEvent(VirStoragePoolRefreshEventArgs args)
        {
            if (Thread.VolatileRead(ref _isDisposing) != 0)
                return;

            lock (_xmlDescrLock)
                _xmlDescription = null; // Fore re-read of configuration
        }
        #endregion

        #region Object overrides
        public override int GetHashCode()
        {
            return UniqueId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is LibvirtStoragePool && ((LibvirtStoragePool)obj).UniqueId.Equals(UniqueId);
        }

        public override string ToString()
        {
            return $"{typeof(LibvirtStoragePool).Name} name={Name}, uuid={UniqueId}, capacity={CapacityInByte}";
        }
        #endregion

        #region IDisposable implementation
        private Int32 _isDisposing = 0;

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _isDisposing, 1, 0) == 1)
                return;

            Trace.WriteLine($"Disposing storage pool {this.ToString()}.");

            if (_poolPtr != IntPtr.Zero)
                NativeVirStoragePool.Free(_poolPtr);
        }
        #endregion
    }
}
