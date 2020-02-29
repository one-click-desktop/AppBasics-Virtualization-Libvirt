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
    /// Represents a libvirt domain
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
                    throw new LibvirtQueryFailedException();
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
                    throw new LibvirtQueryFailedException();
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
                    throw new LibvirtQueryFailedException();
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
                    throw new LibvirtQueryFailedException();
                return poolInfo.Allocation;
            }
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
                            string xmlText = NativeVirStoragePool.GetXMLDesc(_poolPtr, VirDomainXMLFlags.VIR_DOMAIN_XML_SECURE | VirDomainXMLFlags.VIR_DOMAIN_XML_INACTIVE);
                            if (string.IsNullOrWhiteSpace(xmlText))
                                throw new LibvirtQueryFailedException();
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
