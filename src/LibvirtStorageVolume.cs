/*
 * Copyright (C)
 *
 *   Arnaud Champion <arnaud.champion@devatom.fr>
 *   Jaromír Červenka <cervajz@cervajz.com>
 *   Marcus Zoller <marcus.zoller@idnt.net>
 *
 * and the Libvirt-CSharp contributors.
 * 
 * Licensed under the GNU Lesser General Public Library, Version 2.1 (the "License");
 * you may not use this file except in compliance with the License. You may obtain a 
 * copy of the License at
 *
 * https://www.gnu.org/licenses/lgpl-2.1.en.html
 * 
 * or see COPYING.LIB for a copy of the license terms. Unless required by applicable 
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
        }

        #region Properties
        /// <summary>
        /// Unique domain identifier
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// True if the domain is currently active (running).
        /// </summary>
        public string Path { get { return NativeVirStorageVol.GetPath(_volumePtr);  } }

        /// <summary>
        /// The domains human readable name
        /// </summary>
        public string Name {  get { return NativeVirStorageVol.GetName(_volumePtr); } }

        /// <summary>
        /// Get storage pool state
        /// </summary>
        public VirStorageVolType VolumeType
        {
            get
            {
                VirStorageVolInfo volInfo = new VirStorageVolInfo();
                if (NativeVirStorageVol.GetInfo(_volumePtr, ref volInfo) < 0)
                    throw new LibvirtQueryException();
                return volInfo.Type;
            }
        }

        /// <summary>
        /// Get pool capacity
        /// </summary>
        public ulong CapacityInByte
        {
            get
            {
                VirStorageVolInfo volInfo = new VirStorageVolInfo();
                if (NativeVirStorageVol.GetInfo(_volumePtr, ref volInfo) < 0)
                    throw new LibvirtQueryException();
                return volInfo.Capacity;
            }
        }

        /// <summary>
        /// Get allocated byte
        /// </summary>
        public ulong ByteAllocated
        {
            get
            {
                VirStorageVolInfo volInfo = new VirStorageVolInfo();
                if (NativeVirStorageVol.GetInfo(_volumePtr, ref volInfo) < 0)
                    throw new LibvirtQueryException();
                return volInfo.Allocation;
            }
        }

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
                if (_xmlDescription == null)
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
                    }
                }
                return _xmlDescription;
            }
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
            return Key.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is LibvirtStorageVolume && ((LibvirtStorageVolume)obj).Key.Equals(Key);
        }

        public override string ToString()
        {
            return $"{typeof(LibvirtStorageVolume).Name} name={Name}, key={Key}, capacity={CapacityInByte}";
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
