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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace Libvirt
{
    /// <summary>
    /// Represents a libvirt storage volume
    /// </summary>
    public class LibvirtStorageVolume : IDisposable
    {
        private readonly LibvirtConnection _conn;
        private readonly Guid _poolId;
        private readonly IntPtr _volumePtr;
        private XmlDocument _xmlDescription = null;
        private readonly object _xmlDescrLock = new object();
        Lazy<VirStorageVolInfo> _volInfo;

        internal LibvirtStorageVolume(LibvirtConnection conn, Guid poolId, string keyString, IntPtr volumePtr)
        {
            _conn = conn ?? throw new ArgumentNullException("conn");
            _poolId = poolId;
            if (String.IsNullOrEmpty(keyString))
                throw new ArgumentNullException("keyString");
            Key = keyString;
            if (volumePtr == IntPtr.Zero)
                throw new ArgumentNullException("volumePtr");
            _volumePtr = volumePtr;
            Refresh();
        }

        #region Properties
        /// <summary>
        /// Unique domain identifier
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Get the storage volume path
        /// </summary>
        public string Path { get { return NativeVirStorageVol.GetPath(_volumePtr);  } }

        /// <summary>
        /// The domains human readable name
        /// </summary>
        public string Name {  get { return NativeVirStorageVol.GetName(_volumePtr); } }

        /// <summary>
        /// Get storage pool state
        /// </summary>
        public VirStorageVolType VolumeType => _volInfo.Value.Type;

        /// <summary>
        /// Get pool capacity
        /// </summary>
        public ulong CapacityInByte => _volInfo.Value.Capacity;

        /// <summary>
        /// Get allocated byte
        /// </summary>
        public ulong ByteAllocated => _volInfo.Value.Allocation;

        public LibvirtStoragePool StoragePool
        {
            get { return _conn.GetStoragePoolByUniqueId(_poolId); }
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
                            string xmlText = NativeVirStorageVol.GetXMLDesc(_volumePtr, 0);
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

        public string Format
        {
            get { return this.XmlDescription.DocumentElement.SelectSingleNode("//target/format/@type").Value; }
        }

        public DateTime CreatedAt
        {
            get { return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(this.XmlDescription.SelectSingleNode("//target/timestamps/ctime").InnerText.Split('.').First())).DateTime; }
        }

        public DateTime ModifiedAt
        {
            get { return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(this.XmlDescription.SelectSingleNode("//target/timestamps/mtime").InnerText.Split('.').First())).DateTime; }
        }

        #endregion

        public void Refresh()
        {
            if (Thread.VolatileRead(ref _isDisposing) != 0)
                return;

            lock (_xmlDescrLock)
            {
                _xmlDescription = null; // Fore re-read of configuration
                _volInfo = new Lazy<VirStorageVolInfo>(() =>
                {
                    VirStorageVolInfo volInfo = new VirStorageVolInfo();
                    if (NativeVirStorageVol.GetInfo(_volumePtr, ref volInfo) < 0)
                        throw new LibvirtQueryException();
                    return volInfo;
                }, LazyThreadSafetyMode.ExecutionAndPublication);
            }
        }

        #region Events
        internal void DispatchStoragePoolEvent(VirStoragePoolLifecycleEventArgs args)
        {
            if (Thread.VolatileRead(ref _isDisposing) != 0)
                return;
        }

        internal void DispatchStoragePoolEvent(VirStoragePoolRefreshEventArgs args)
        {
            Refresh();
        }
        #endregion

        #region Object overrides
        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is LibvirtStorageVolume && ((LibvirtStorageVolume)obj).Key.Equals(Key);
        }

        public override string ToString()
        {
            return $"{typeof(LibvirtStorageVolume).Name} {Key}";
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

            Trace.WriteLine($"Disposing storage volume {this.ToString()}.");

            if (_volumePtr != IntPtr.Zero)
                NativeVirStorageVol.Free(_volumePtr);
        }
        #endregion
    }
}
