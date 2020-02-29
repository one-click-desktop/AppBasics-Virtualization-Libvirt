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
    public class LibvirtDomain : IDisposable
    {
        private readonly LibvirtConnection _conn;
        private readonly IntPtr _domainPtr;
        private XmlDocument _xmlDescription = null;
        private readonly object _xmlDescrLock = new object();

        internal LibvirtDomain(LibvirtConnection connection, Guid uniqueId, IntPtr domainPtr)
        {
            _conn = connection ?? throw new ArgumentNullException("connection");
            if (Guid.Empty.Equals(uniqueId))
                throw new ArgumentNullException("uniqueId");
            UniqueId = uniqueId;
            if (domainPtr == IntPtr.Zero)
                throw new ArgumentNullException("domainPtr");
            _domainPtr = domainPtr;
        }

        #region Properties
        /// <summary>
        /// Unique domain identifier
        /// </summary>
        public Guid UniqueId { get; private set; }

        /// <summary>
        /// True if the domain is currently active (running).
        /// </summary>
        public bool IsActive { get { return NativeVirDomain.IsActive(_domainPtr) == 1;  } }

        /// <summary>
        /// Retruns the hypervisors domain id or -1 if the domain is not running.
        /// </summary>
        public int Id { get { return NativeVirDomain.GetID(_domainPtr); } }

        /// <summary>
        /// The domains human readable name
        /// </summary>
        public string Name {  get { return NativeVirDomain.GetName(_domainPtr); } }

        /// <summary>
        /// The type of operating system
        /// </summary>
        public string OSType {  get { return NativeVirDomain.GetOSType(_domainPtr); } }

        /// <summary>
        /// Get domains runnig state
        /// </summary>
        public VirDomainState State
        {
            get
            {
                VirDomainState state = VirDomainState.VIR_DOMAIN_NOSTATE;
                int reason = 0;
                if (NativeVirDomain.GetState(_domainPtr, out state, out reason, 0) < 0)
                    return VirDomainState.VIR_DOMAIN_NOSTATE;
                return state;
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
                            string xmlText = NativeVirDomain.GetXMLDesc(_domainPtr, VirDomainXMLFlags.VIR_DOMAIN_XML_SECURE | VirDomainXMLFlags.VIR_DOMAIN_XML_INACTIVE);
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
        internal void DispatchDomainEvent(VirDomainEventArgs args)
        {
            if (Thread.VolatileRead(ref _isDisposing) != 0)
                return;

            if (args.EventType == VirDomainEventType.VIR_DOMAIN_EVENT_DEFINED)
                lock(_xmlDescrLock)
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
            return obj is LibvirtDomain && ((LibvirtDomain)obj).UniqueId.Equals(UniqueId);
        }

        public override string ToString()
        {
            return $"{typeof(LibvirtDomain).Name} name={Name}, uuid={UniqueId}, osType={OSType}";
        }
        #endregion

        #region Control

        /// <summary>
        /// Reset a domain immediately without any guest OS shutdown. Reset emulates the power reset button on a machine, where all hardware sees the RST line set and reinitializes internal state.
        /// </summary>
        /// <returns>True on success</returns>
        public bool Reset()
        {
            return NativeVirDomain.Reset(_domainPtr, 0) == 0;
        }

        /// <summary>
        /// Suspends an active domain, the process is frozen without further access to CPU resources and I/O but the memory used by the domain at the hypervisor level will stay allocated. 
        /// </summary>
        /// <returns>True on success</returns>
        public bool Suspend()
        {
            return NativeVirDomain.Suspend(_domainPtr) == 0;
        }

        /// <summary>
        /// Resume a suspended domain, the process is restarted from the state where it was frozen by calling virDomainSuspend(). This function may require privileged access Moreover, resume may not be supported if domain is in some special state like VIR_DOMAIN_PMSUSPENDED.
        /// </summary>
        /// <returns>True on success</returns>
        public bool Resume()
        {
            return NativeVirDomain.Resume(_domainPtr) == 0;
        }

        /// <summary>
        /// Shutdown a domain, the domain object is still usable thereafter, but the domain OS is being stopped. Note that the guest OS may ignore the request.
        /// </summary>
        /// <returns>True on success</returns>
        public bool Shutdown()
        {
            return NativeVirDomain.Shutdown(_domainPtr) == 0;
        }

        /// <summary>
        /// Shutdown a domain, the domain object is still usable thereafter, but the domain OS is being stopped. Note that the guest OS may ignore the request.
        /// </summary>
        /// <returns>True on success</returns>
        public bool ManagedSave()
        {
            return NativeVirDomain.ManagedSave(_domainPtr, VirDomainSaveRestoreFlags.Empty) == 0;
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

            Trace.WriteLine($"Disposing domain {this.ToString()}.");

            if (_domainPtr != IntPtr.Zero)
                NativeVirDomain.Free(_domainPtr);
        }
        #endregion
    }
}
