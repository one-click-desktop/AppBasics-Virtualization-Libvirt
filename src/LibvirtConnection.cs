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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Libvirt
{
    /// <summary>
    /// Represents a hypervirsor node connection
    /// </summary>
    public class LibvirtConnection : IDisposable
    {
        private readonly IntPtr _connPtr;

        private readonly CancellationTokenSource _closeTokenSource;
        private readonly LibvirtEventLoop _lvEvents;

        /// <summary>
        /// Creates a new connections
        /// </summary>
        /// <param name="conn"></param>
        public LibvirtConnection(IntPtr conn)
        {
            if (conn == IntPtr.Zero)
                throw new LibvirtConnectionErrorException();
            _connPtr = conn;

            _closeTokenSource = new CancellationTokenSource();

            _lvEvents = new LibvirtEventLoop(this);
        }

        #region Connection
        public bool IsAlive {  get { return NativeVirConnect.IsAlive(_connPtr) == 1; } }
        
        public void SetKeepAlive(int interval, uint count)
        {
            if (NativeVirConnect.SetKeepAlive(_connPtr, interval, count) < 0)
                throw new LibvirtConnectionErrorException();
        }

        #endregion

        #region Domains
        private ConcurrentDictionary<Guid, LibvirtDomain> _domainCache = 
                new ConcurrentDictionary<Guid, LibvirtDomain>();

        /// <summary>
        /// Queries domains
        /// </summary>
        /// <param name="includeDefined">Returns inactive domains if true</param>
        /// <returns>List of domains</returns>
        public IEnumerable<LibvirtDomain> ListDomains(bool includeDefined = false)
        {
            int nbDomains = NativeVirConnect.NumOfDomains(_connPtr);
            int[] domainIDs = new int[nbDomains];
            if (NativeVirConnect.ListDomains(_connPtr, domainIDs, nbDomains) < 0)
                throw new LibvirtQueryFailedException();

            foreach (IntPtr domainPtr in domainIDs.Select(domainID => NativeVirDomain.LookupByID(_connPtr, domainID)))
            {
                var domUUID = new char[16];
                if (NativeVirDomain.GetUUID(domainPtr, domUUID) < 0)
                    throw new LibvirtQueryFailedException();

                var uuid = new Guid(domUUID.Select(t => Convert.ToByte(t)).ToArray());

                yield return _domainCache.GetOrAdd(uuid, (id) =>
                {
                    return new LibvirtDomain(this, uuid, domainPtr);
                });
            }

            if (includeDefined)
            {
                nbDomains = NativeVirConnect.NumOfDefinedDomains(_connPtr);
                string[] domainNames = new string[nbDomains];
                if (NativeVirConnect.ListDefinedDomains(_connPtr, ref domainNames, nbDomains) < 0)
                    throw new LibvirtQueryFailedException();

                foreach (IntPtr domainPtr in domainNames.Select(domainName => NativeVirDomain.LookupByName(_connPtr, domainName)))
                {
                    var domUUID = new char[16];
                    if (NativeVirDomain.GetUUID(domainPtr, domUUID) < 0)
                        throw new LibvirtQueryFailedException();

                    var uuid = new Guid(domUUID.Select(t => Convert.ToByte(t)).ToArray());

                    yield return _domainCache.GetOrAdd(uuid, (id) =>
                    {
                        return new LibvirtDomain(this, uuid, domainPtr);
                    });
                }
            }
        }

        /// <summary>
        /// Get a domain by UUID
        /// </summary>
        /// <param name="uuid">Domains unique id</param>
        /// <returns>Domain or NULL</returns>
        public LibvirtDomain GetDomainByUUID(Guid uuid)
        {
            var domainPtr = NativeVirDomain.LookupByUUID(_connPtr, uuid.ToByteArray().Select(t => Convert.ToChar(t)).ToArray());
            if (domainPtr == IntPtr.Zero)
            {
                LibvirtDomain domain;
                if (_domainCache.TryRemove(uuid, out domain))
                    domain.Dispose();

                Trace.WriteLine($"Could not find domain with UUID '{uuid}'.");
                return null;
            }

            return _domainCache.GetOrAdd(uuid, (id) =>
            {
                return new LibvirtDomain(this, uuid, domainPtr);
            });
        }

        /// <summary>
        /// Get a domain by hypervisor id
        /// </summary>
        /// <param name="domainId">Hypervisors domain id</param>
        /// <returns>Domain or NULL</returns>
        public LibvirtDomain GetDomainByID(int domainId)
        {
            var domainPtr = NativeVirDomain.LookupByID(_connPtr, domainId);
            if (domainPtr == IntPtr.Zero)
            {
                Trace.WriteLine($"Could not find domain with ID '{domainId}'.");
                return null;
            }

            var domUUID = new char[16];
            if (NativeVirDomain.GetUUID(domainPtr, domUUID) < 0)
                throw new LibvirtQueryFailedException();

            var uuid = new Guid(domUUID.Select(t => Convert.ToByte(t)).ToArray());

            return _domainCache.GetOrAdd(uuid, (id) =>
            {
                return new LibvirtDomain(this, uuid, domainPtr);
            });
        }

        /// <summary>
        /// Get a domain by name
        /// </summary>
        /// <param name="domainName">Domain name</param>
        /// <returns>Domain or NULL</returns>
        public LibvirtDomain GetDomainByName(string domainName)
        {
            var domainPtr = NativeVirDomain.LookupByName(_connPtr, domainName);
            if (domainPtr == IntPtr.Zero)
            {
                Trace.WriteLine($"Could not find domain with name '{domainName}'.");
                return null;
            }

            var domUUID = new char[16];
            if (NativeVirDomain.GetUUID(domainPtr, domUUID) < 0)
                throw new LibvirtQueryFailedException();

            var uuid = new Guid(domUUID.Select(t => Convert.ToByte(t)).ToArray());

            return _domainCache.GetOrAdd(uuid, (id) =>
            {
                return new LibvirtDomain(this, uuid, domainPtr);
            });
        }

        /// <summary>
        /// Enumerates all running as well as defined domains
        /// </summary>
        public IEnumerable<LibvirtDomain> Domains
        {
            get { return ListDomains(includeDefined: true); }
        }
        #endregion

        #region Storage Pools
        private ConcurrentDictionary<Guid, LibvirtStoragePool> _poolCache =
                new ConcurrentDictionary<Guid, LibvirtStoragePool>();

        /// <summary>
        /// Queries storag epools
        /// </summary>
        /// <param name="includeDefined">Returns inactive storag epools if true</param>
        /// <returns>List of storag epools</returns>
        public IEnumerable<LibvirtStoragePool> ListStoragePools(bool includeDefined = false)
        {
            int nbPools = NativeVirConnect.NumOfStoragePools(_connPtr);
            string[] poolNames = new string[nbPools];
            if (NativeVirConnect.ListStoragePools(_connPtr, ref poolNames, nbPools) < 0)
                throw new LibvirtQueryFailedException();

            foreach (IntPtr poolPtr in poolNames.Select(poolName => NativeVirStoragePool.LookupByName(_connPtr, poolName)))
            {
                var poolUUID = new char[16];
                if (NativeVirStoragePool.GetUUID(poolPtr, poolUUID) < 0)
                    throw new LibvirtQueryFailedException();

                var uuid = new Guid(poolUUID.Select(t => Convert.ToByte(t)).ToArray());

                yield return _poolCache.GetOrAdd(uuid, (id) =>
                {
                    return new LibvirtStoragePool(this, uuid, poolPtr);
                });
            }

            if (includeDefined)
            {
                nbPools = NativeVirConnect.NumOfDefinedStoragePools(_connPtr);
                poolNames = new string[nbPools];
                if (NativeVirConnect.ListDefinedStoragePools(_connPtr, ref poolNames, nbPools) < 0)
                    throw new LibvirtQueryFailedException();

                foreach (IntPtr poolPtr in poolNames.Select(poolName => NativeVirStoragePool.LookupByName(_connPtr, poolName)))
                {
                    var poolUUID = new char[16];
                    if (NativeVirStoragePool.GetUUID(poolPtr, poolUUID) < 0)
                        throw new LibvirtQueryFailedException();

                    var uuid = new Guid(poolUUID.Select(t => Convert.ToByte(t)).ToArray());

                    yield return _poolCache.GetOrAdd(uuid, (id) =>
                    {
                        return new LibvirtStoragePool(this, uuid, poolPtr);
                    });
                }
            }
        }

        /// <summary>
        /// Get a storag epool by UUID
        /// </summary>
        /// <param name="uuid">Storage pools unique id</param>
        /// <returns>Storage pool or NULL</returns>
        public LibvirtStoragePool GetStoragePoolByUUID(Guid uuid)
        {
            var poolPtr = NativeVirStoragePool.LookupByUUID(_connPtr, uuid.ToByteArray().Select(t => Convert.ToChar(t)).ToArray());
            if (poolPtr == IntPtr.Zero)
            {
                LibvirtStoragePool pool;
                if (_poolCache.TryRemove(uuid, out pool))
                    pool.Dispose();

                Trace.WriteLine($"Could not find storage pool with UUID '{uuid}'.");
                return null;
            }

            return _poolCache.GetOrAdd(uuid, (id) =>
            {
                return new LibvirtStoragePool(this, uuid, poolPtr);
            });
        }

        /// <summary>
        /// Get a storage pool by name
        /// </summary>
        /// <param name="poolName">Storage pool name</param>
        /// <returns>Storage pool or NULL</returns>
        public LibvirtStoragePool GetStoragePoolByName(string poolName)
        {
            var poolPtr = NativeVirStoragePool.LookupByName(_connPtr, poolName);
            if (poolPtr == IntPtr.Zero)
            {
                Trace.WriteLine($"Could not find storage pool with name '{poolName}'.");
                return null;
            }

            var poolUUID = new char[16];
            if (NativeVirStoragePool.GetUUID(poolPtr, poolUUID) < 0)
                throw new LibvirtQueryFailedException();

            var uuid = new Guid(poolUUID.Select(t => Convert.ToByte(t)).ToArray());

            return _poolCache.GetOrAdd(uuid, (id) =>
            {
                return new LibvirtStoragePool(this, uuid, poolPtr);
            });
        }

        /// <summary>
        /// Enumerates all running as well as defined storag epools
        /// </summary>
        public IEnumerable<LibvirtStoragePool> StoragePools
        {
            get { return ListStoragePools(includeDefined: true); }
        }
        #endregion

        #region Static
        static LibvirtConnection()
        {
            if (NativeVirEvent.RegisterDefaultImpl() != 0)
                throw new LibvirtException();
            Trace.WriteLine("Registered default event loop implementation.");
        }

        /// <summary>
        /// Opens a new connection
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        static public LibvirtConnection Open(string conn = @"qemu:///system")
        {
            return new LibvirtConnection(NativeVirConnect.Open(conn));
        }

        #endregion

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        public void Close()
        {
            this.Dispose();
        }

        #region Internal 

        internal IntPtr ConnectionPtr
        {
            get { return _connPtr; }
        }

        internal CancellationToken ShutdownToken
        {
            get { return _closeTokenSource.Token; }
        }
        #endregion

        #region Events
        private object _domainEventReceivedLock = new object();
        private event EventHandler<VirDomainEventArgs> _domainEventHandler;

        public event EventHandler<VirDomainEventArgs> DomainEventReceived
        {
            add { lock (_domainEventReceivedLock) _domainEventHandler += value; }
            remove { lock (_domainEventReceivedLock) _domainEventHandler -= value; }
        }

        internal void DispatchDomainEvent(Guid domainUUID, VirDomainEventArgs args)
        {
            if (Thread.VolatileRead(ref _isDisposing) != 0)
                return;

            LibvirtDomain domain = null;
            (domain = GetDomainByUUID(domainUUID))?.DispatchDomainEvent(args);

            if (domain != null)
            {
                EventHandler<VirDomainEventArgs> handler;
                lock (_domainEventReceivedLock)
                    handler = _domainEventHandler;

                try
                {
                    if (handler != null)

                        handler.Invoke(domain, args);
                }
                finally
                {
                    if (args.EventType == VirDomainEventType.VIR_DOMAIN_EVENT_UNDEFINED)
                        if (_domainCache.TryRemove(domain.UUID, out domain))
                            domain.Dispose();
                }
            }
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

            Trace.WriteLine("Disposing connection.");

            _closeTokenSource.Cancel(false);

            foreach (var domain in _domainCache.Values)
                domain.Dispose();

            _domainCache.Clear();

            foreach (var storagePool in _poolCache.Values)
                storagePool.Dispose();

            _poolCache.Clear();

            if (_connPtr != IntPtr.Zero)
                NativeVirConnect.Close(_connPtr);
        }
        #endregion
    }
}
