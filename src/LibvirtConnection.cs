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
using System.Threading.Tasks;
using System.Xml;

namespace Libvirt
{
    /// <summary>
    /// Represents a hypervirsor node connection
    /// </summary>
    public class LibvirtConnection : IDisposable
    {
        private readonly CancellationTokenSource _closeTokenSource = new CancellationTokenSource();
        private readonly LibvirtEventLoop _lvEvents;
        private readonly Timer _metricsTicker;

        /// <summary>
        /// Creates a new connections
        /// </summary>
        /// <param name="conn"></param>
        public LibvirtConnection(IntPtr conn, LibvirtConfiguration configuration = null)
        {
            if (conn == IntPtr.Zero)
                throw new LibvirtConnectionException();

            ConnectionPtr = conn;

            Configuration = configuration ?? new LibvirtConfiguration();

            _lvEvents = new LibvirtEventLoop(this);

            SetKeepAlive(LibvirtConfiguration.DEFAULT_LIBVIRT_KEEPALIVE_INTERVAL, 
                         LibvirtConfiguration.DEFAULT_LIBVIRT_KEEPALIVE_COUNT);

            Node = new LibvirtNode(this);

            _metricsTicker = new Timer(MetricsTickerCallback, null,
                Configuration.MetricsIntervalSeconds * 1000,
                Configuration.MetricsIntervalSeconds * 1000);

            Configuration.OnMetricsIntervalChanged = (i) => 
                _metricsTicker.Change(i == 0 ? Timeout.Infinite : i, i == 0 ? Timeout.Infinite : i);
        }

        #region Metrics Tick
        private readonly object _metricsTickEventLock = new object();
        private event EventHandler _metricsTickEventHandler;

        internal event EventHandler MetricsTick
        {
            add { lock (_metricsTickEventLock) _metricsTickEventHandler += value; }
            remove { lock (_metricsTickEventLock) _metricsTickEventHandler -= value; }
        }

        private void MetricsTickerCallback(object state)
        {
            if (Thread.VolatileRead(ref _isDisposing) != 0)
                return;

            EventHandler handler;
            lock (_metricsTickEventLock)
                handler = _metricsTickEventHandler;

            handler?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Connection
        public bool IsAlive {  get { return NativeVirConnect.IsAlive(ConnectionPtr) == 1; } }
        
        public void SetKeepAlive(int interval, uint count)
        {
            if (NativeVirConnect.SetKeepAlive(ConnectionPtr, interval, count) < 0)
                throw new LibvirtConnectionException();
        }

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        public void Close()
        {
            this.Dispose();
        }

        public LibvirtConfiguration Configuration { get; private set; }

        #endregion

        #region Node
        public LibvirtNode Node { get; private set; }

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
            int nbDomains = NativeVirConnect.NumOfDomains(ConnectionPtr);
            int[] domainIDs = new int[nbDomains];
            if (NativeVirConnect.ListDomains(ConnectionPtr, domainIDs, nbDomains) < 0)
                throw new LibvirtQueryException();

            foreach (IntPtr domainPtr in domainIDs.Select(domainID => NativeVirDomain.LookupByID(ConnectionPtr, domainID)))
            {
                try
                {
                    var domUUID = new char[16];
                    if (NativeVirDomain.GetUUID(domainPtr, domUUID) < 0)
                        throw new LibvirtQueryException();

                    yield return _domainCache.GetOrAdd(domUUID.ToGuid(), (id) =>
                    {
                        NativeVirDomain.Ref(domainPtr);
                        return new LibvirtDomain(this, domUUID.ToGuid(), domainPtr);
                    });
                }
                finally
                {
                    NativeVirDomain.Free(domainPtr);
                }
            }

            if (includeDefined)
            {
                nbDomains = NativeVirConnect.NumOfDefinedDomains(ConnectionPtr);
                string[] domainNames = new string[nbDomains];
                if (NativeVirConnect.ListDefinedDomains(ConnectionPtr, ref domainNames, nbDomains) < 0)
                    throw new LibvirtQueryException();

                foreach (IntPtr domainPtr in domainNames.Select(domainName => NativeVirDomain.LookupByName(ConnectionPtr, domainName)))
                {
                    try
                    {
                        var domUUID = new char[16];
                        if (NativeVirDomain.GetUUID(domainPtr, domUUID) < 0)
                            throw new LibvirtQueryException();

                        yield return _domainCache.GetOrAdd(domUUID.ToGuid(), (id) =>
                        {
                            NativeVirDomain.Ref(domainPtr);
                            return new LibvirtDomain(this, domUUID.ToGuid(), domainPtr);
                        });
                    }
                    finally
                    {
                        NativeVirDomain.Free(domainPtr);
                    }
                }
            }
        }

        /// <summary>
        /// Get a domain by UUID
        /// </summary>
        /// <param name="uniqueId">Domains unique id</param>
        /// <returns>Domain or NULL</returns>
        public LibvirtDomain GetDomainByUniqueId(Guid uniqueId, bool cachedOnly = false)
        {
            LibvirtDomain domain;
            if (_domainCache.TryGetValue(uniqueId, out domain))
                return domain;

            if (cachedOnly)
                return null;
            
            var domainPtr = NativeVirDomain.LookupByUUID(ConnectionPtr, uniqueId.ToUUID());
            if (domainPtr == IntPtr.Zero)
            {
                if (_domainCache.TryRemove(uniqueId, out domain))
                    domain.Dispose();

                Trace.WriteLine($"Could not find domain with UUID '{uniqueId}'.");
                return null;
            }

            try
            {
                return _domainCache.GetOrAdd(uniqueId, (id) =>
                {
                    NativeVirDomain.Ref(domainPtr);
                    return new LibvirtDomain(this, uniqueId, domainPtr);
                });
            }
            finally
            {
                NativeVirDomain.Free(domainPtr);
            }
        }

        /// <summary>
        /// Get a domain by hypervisor id
        /// </summary>
        /// <param name="domainId">Hypervisors domain id</param>
        /// <returns>Domain or NULL</returns>
        public LibvirtDomain GetDomainByID(int domainId)
        {
            var domainPtr = NativeVirDomain.LookupByID(ConnectionPtr, domainId);
            if (domainPtr == IntPtr.Zero)
            {
                Trace.WriteLine($"Could not find domain with ID '{domainId}'.");
                return null;
            }

            try
            {
                var domUUID = new char[16];
                if (NativeVirDomain.GetUUID(domainPtr, domUUID) < 0)
                    throw new LibvirtQueryException();

                return _domainCache.GetOrAdd(domUUID.ToGuid(), (id) =>
                {
                    NativeVirDomain.Ref(domainPtr);
                    return new LibvirtDomain(this, domUUID.ToGuid(), domainPtr);
                });
            }
            finally
            {
                NativeVirDomain.Free(domainPtr);
            }
        }

        /// <summary>
        /// Get a domain by name
        /// </summary>
        /// <param name="domainName">Domain name</param>
        /// <returns>Domain or NULL</returns>
        public LibvirtDomain GetDomainByName(string domainName)
        {
            var domainPtr = NativeVirDomain.LookupByName(ConnectionPtr, domainName);
            if (domainPtr == IntPtr.Zero)
            {
                Trace.WriteLine($"Could not find domain with name '{domainName}'.");
                return null;
            }

            try
            {
                var domUUID = new char[16];
                if (NativeVirDomain.GetUUID(domainPtr, domUUID) < 0)
                    throw new LibvirtQueryException();

                return _domainCache.GetOrAdd(domUUID.ToGuid(), (id) =>
                {
                    NativeVirDomain.Ref(domainPtr);
                    return new LibvirtDomain(this, domUUID.ToGuid(), domainPtr);
                });
            }
            finally
            {
                NativeVirDomain.Free(domainPtr);
            }
        }

        /// <summary>
        /// Enumerates all running as well as defined domains
        /// </summary>
        public IEnumerable<LibvirtDomain> Domains
        {
            get { return ListDomains(includeDefined: true); }
        }

        #region Events
        private readonly object _domainEventReceivedLock = new object();
        private event EventHandler<VirDomainEventArgs> _domainEventHandler;

        public event EventHandler<VirDomainEventArgs> DomainEventReceived
        {
            add { lock (_domainEventReceivedLock) _domainEventHandler += value; }
            remove { lock (_domainEventReceivedLock) _domainEventHandler -= value; }
        }

        internal void DispatchDomainEvent(Guid uniqueId, VirDomainEventArgs args)
        {
            if (Thread.VolatileRead(ref _isDisposing) != 0)
                return;

            LibvirtDomain domain = GetDomainByUniqueId(uniqueId,
                cachedOnly: args.EventType == VirDomainEventType.VIR_DOMAIN_EVENT_UNDEFINED);
            
            domain?.DispatchDomainEvent(args);

            EventHandler<VirDomainEventArgs> handler;
            lock (_domainEventReceivedLock)
                handler = _domainEventHandler;

            try
            {
                handler?.Invoke(domain, args);
            }
            finally
            {
                if (domain != null && args.EventType == VirDomainEventType.VIR_DOMAIN_EVENT_UNDEFINED)
                    if (_domainCache.TryRemove(domain.UniqueId, out domain))
                        domain.Dispose();
            }
        }
        #endregion

        #endregion

        #region Storage Pools
        private ConcurrentDictionary<Guid, LibvirtStoragePool> _storagePoolCache =
                new ConcurrentDictionary<Guid, LibvirtStoragePool>();

        /// <summary>
        /// Queries storag epools
        /// </summary>
        /// <param name="includeDefined">Returns inactive storag epools if true</param>
        /// <returns>List of storag epools</returns>
        public IEnumerable<LibvirtStoragePool> ListStoragePools(bool includeDefined = false)
        {
            int nbPools = NativeVirConnect.NumOfStoragePools(ConnectionPtr);
            string[] poolNames = new string[nbPools];
            if (NativeVirConnect.ListStoragePools(ConnectionPtr, ref poolNames, nbPools) < 0)
                throw new LibvirtQueryException();

            foreach (IntPtr poolPtr in poolNames.Select(poolName => NativeVirStoragePool.LookupByName(ConnectionPtr, poolName)))
            {
                try
                {
                    var poolUUID = new char[16];
                    if (NativeVirStoragePool.GetUUID(poolPtr, poolUUID) < 0)
                        throw new LibvirtQueryException();

                    yield return _storagePoolCache.GetOrAdd(poolUUID.ToGuid(), (id) =>
                    {
                        NativeVirStoragePool.Ref(poolPtr);
                        return new LibvirtStoragePool(this, poolUUID.ToGuid(), poolPtr);
                    });
                }
                finally
                {
                    NativeVirStoragePool.Free(poolPtr);
                }
            }

            if (includeDefined)
            {
                nbPools = NativeVirConnect.NumOfDefinedStoragePools(ConnectionPtr);
                poolNames = new string[nbPools];
                if (NativeVirConnect.ListDefinedStoragePools(ConnectionPtr, ref poolNames, nbPools) < 0)
                    throw new LibvirtQueryException();

                foreach (IntPtr poolPtr in poolNames.Select(poolName => NativeVirStoragePool.LookupByName(ConnectionPtr, poolName)))
                {
                    try
                    {
                        var poolUUID = new char[16];
                        if (NativeVirStoragePool.GetUUID(poolPtr, poolUUID) < 0)
                            throw new LibvirtQueryException();

                        yield return _storagePoolCache.GetOrAdd(poolUUID.ToGuid(), (id) =>
                        {
                            NativeVirStoragePool.Ref(poolPtr);
                            return new LibvirtStoragePool(this, poolUUID.ToGuid(), poolPtr);
                        });
                    }
                    finally
                    {
                        NativeVirStoragePool.Free(poolPtr);
                    }
                }
            }
        }

        /// <summary>
        /// Get a storag epool by UUID
        /// </summary>
        /// <param name="uniqueId">Storage pools unique id</param>
        /// <returns>Storage pool or NULL</returns>
        public LibvirtStoragePool GetStoragePoolByUniqueId(Guid uniqueId, bool cachedOnly = false)
        {
            LibvirtStoragePool pool;
            if (_storagePoolCache.TryGetValue(uniqueId, out pool))
                return pool;

            if (cachedOnly)
                return null;

            var poolPtr = NativeVirStoragePool.LookupByUUID(ConnectionPtr, uniqueId.ToUUID());
            if (poolPtr == IntPtr.Zero)
            {
                if (_storagePoolCache.TryRemove(uniqueId, out pool))
                    pool.Dispose();

                Trace.WriteLine($"Could not find storage pool with UUID '{uniqueId}'.");
                return null;
            }

            try
            {
                return _storagePoolCache.GetOrAdd(uniqueId, (id) =>
                {
                    NativeVirStoragePool.Ref(poolPtr);
                    return new LibvirtStoragePool(this, uniqueId, poolPtr);
                });
            }
            finally
            {
                NativeVirStoragePool.Free(poolPtr);
            }
        }

        /// <summary>
        /// Get a storage pool by name
        /// </summary>
        /// <param name="poolName">Storage pool name</param>
        /// <returns>Storage pool or NULL</returns>
        public LibvirtStoragePool GetStoragePoolByName(string poolName)
        {
            var poolPtr = NativeVirStoragePool.LookupByName(ConnectionPtr, poolName);
            if (poolPtr == IntPtr.Zero)
            {
                Trace.WriteLine($"Could not find storage pool with name '{poolName}'.");
                return null;
            }
            try
            {
                var poolUUID = new char[16];
                if (NativeVirStoragePool.GetUUID(poolPtr, poolUUID) < 0)
                    throw new LibvirtQueryException();

                return _storagePoolCache.GetOrAdd(poolUUID.ToGuid(), (id) =>
                {
                    NativeVirStoragePool.Ref(poolPtr);
                    return new LibvirtStoragePool(this, poolUUID.ToGuid(), poolPtr);
                });
            }
            finally
            {
                NativeVirStoragePool.Free(poolPtr);
            }
        }

        /// <summary>
        /// Enumerates all running as well as defined storag epools
        /// </summary>
        public IEnumerable<LibvirtStoragePool> StoragePools
        {
            get { return ListStoragePools(includeDefined: true); }
        }
        #endregion

        #region Events
        private readonly object _storagePoolLifecycleEventReceivedLock = new object();
        private event EventHandler<VirStoragePoolLifecycleEventArgs> _storagePoolLifecycleEventHandler;

        public event EventHandler<VirStoragePoolLifecycleEventArgs> StoragePoolLifecycleEventReceived
        {
            add { lock (_storagePoolLifecycleEventReceivedLock) _storagePoolLifecycleEventHandler += value; }
            remove { lock (_storagePoolLifecycleEventReceivedLock) _storagePoolLifecycleEventHandler -= value; }
        }

        private readonly object _storagePoolRefreshEventReceivedLock = new object();
        private event EventHandler<VirStoragePoolRefreshEventArgs> _storagePoolRefreshEventHandler;

        public event EventHandler<VirStoragePoolRefreshEventArgs> StoragePoolRefreshEventReceived
        {
            add { lock (_storagePoolRefreshEventReceivedLock) _storagePoolRefreshEventHandler += value; }
            remove { lock (_storagePoolRefreshEventReceivedLock) _storagePoolRefreshEventHandler -= value; }
        }

        internal void DispatchStoragePoolEvent(Guid uniqueId, VirStoragePoolLifecycleEventArgs args)
        {
            if (Thread.VolatileRead(ref _isDisposing) != 0)
                return;

            LibvirtStoragePool pool = GetStoragePoolByUniqueId(uniqueId,
                cachedOnly: args.EventType == VirStoragePoolEventLifecycleType.VIR_STORAGE_POOL_EVENT_UNDEFINED);

            pool?.DispatchStoragePoolEvent(args);

            EventHandler<VirStoragePoolLifecycleEventArgs> handler;
            lock (_storagePoolLifecycleEventReceivedLock)
                handler = _storagePoolLifecycleEventHandler;

            try
            {
                handler?.Invoke(pool, args);
            }
            finally
            {
                if (pool != null && args.EventType == VirStoragePoolEventLifecycleType.VIR_STORAGE_POOL_EVENT_UNDEFINED)
                    if (_storagePoolCache.TryRemove(pool.UniqueId, out pool))
                        pool.Dispose();
            }
        }

        internal void DispatchStoragePoolEvent(Guid uniqueId, VirStoragePoolRefreshEventArgs args)
        {
            if (Thread.VolatileRead(ref _isDisposing) != 0)
                return;

            LibvirtStoragePool pool = GetStoragePoolByUniqueId(uniqueId);

            pool?.DispatchStoragePoolEvent(args);

            EventHandler<VirStoragePoolRefreshEventArgs> handler;
            lock (_storagePoolRefreshEventReceivedLock)
                handler = _storagePoolRefreshEventHandler;

            if (handler != null)
                handler.Invoke(pool, args);
        }

        internal ConcurrentDictionary<string, LibvirtStorageVolume> VolumeCache { get; } = new ConcurrentDictionary<string, LibvirtStorageVolume>();

        public IEnumerable<LibvirtStorageVolume> ListStorageVolumes()
        {
            foreach (var volume in this.StoragePools.SelectMany(x => x.Volumes))
                yield return volume;
        }

        public IEnumerable<LibvirtStorageVolume> StorageVolumes
        {
            get { return ListStorageVolumes(); }
        }

        /// <summary>
        /// Get a volume by name
        /// </summary>
        /// <param name="volumeKey">Volume key</param>
        /// <returns>Volume or NULL</returns>
        public LibvirtStorageVolume GetVolumeByKey(string volumeKey, bool cachedOnly = false)
        {
            if (string.IsNullOrEmpty(volumeKey))
                throw new ArgumentNullException("volumeKey");

            Trace.WriteLine($"GetVolumeByKey('{volumeKey}', {cachedOnly})");

            LibvirtStorageVolume volume;
            if (cachedOnly)
            {
                if (VolumeCache.TryGetValue(volumeKey, out volume))
                    return volume;
                return null;
            }

            var volumePtr = NativeVirStorageVol.LookupByKey(ConnectionPtr, volumeKey);
            if (volumePtr == IntPtr.Zero)
            {
                if (VolumeCache.TryRemove(volumeKey, out volume))
                    volume.Dispose();

                Trace.WriteLine($"Could not find volume with key '{volumeKey}'.");
                return null;
            }

            try
            {
                var poolPtr = NativeVirStoragePool.LookupByVolume(volumePtr);
                if (poolPtr == IntPtr.Zero)
                    throw new LibvirtQueryException();

                try
                {
                    var poolUUID = new char[16];
                    if (NativeVirStoragePool.GetUUID(poolPtr, poolUUID) < 0)
                        throw new LibvirtQueryException();

                    return VolumeCache.GetOrAdd(volumeKey, (id) =>
                    {
                        NativeVirStorageVol.Ref(volumePtr);
                        return new LibvirtStorageVolume(this, poolUUID.ToGuid(), volumeKey, volumePtr);
                    });
                }
                finally
                {
                    NativeVirStoragePool.Free(poolPtr);
                }
            }
            finally
            {
                NativeVirStorageVol.Free(volumePtr);
            }
        }

        public LibvirtStorageVolume GetVolumeByDiskSource(VirXmlDomainDiskSource diskSource)
        {
            if (diskSource == null)
                return null;

            return GetVolumeByKey(diskSource.GetPath());
        }
        #endregion

        #region Static
        static LibvirtConnection()
        {
            NativeVirInitialize.Initialize();

            if (NativeVirEvent.RegisterDefaultImpl() != 0)
                throw new LibvirtException();
            Trace.WriteLine("Registered default event loop implementation.");
        }

        /// <summary>
        /// Opens a new connection
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        static public LibvirtConnection Open(string conn = @"qemu:///system", LibvirtConfiguration configuration = null)
        {
            return new LibvirtConnection(NativeVirConnect.Open(conn), configuration);
        }

        #endregion

        #region Internal 

        internal IntPtr ConnectionPtr { get; private set; }

        internal CancellationToken ShutdownToken
        {
            get { return _closeTokenSource.Token; }
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

            if (_metricsTicker != null)
            {
                _metricsTicker.Change(Timeout.Infinite, Timeout.Infinite);
                _metricsTicker.Dispose();
            }

            _closeTokenSource.Cancel(false);

            foreach (var domain in _domainCache.Values)
                domain.Dispose();

            _domainCache.Clear();

            foreach (var storageVolume in VolumeCache.Values)
                storageVolume.Dispose();

            VolumeCache.Clear();

            foreach (var storagePool in _storagePoolCache.Values)
                storagePool.Dispose();

            _storagePoolCache.Clear();

            Node.Dispose();

            if (ConnectionPtr != IntPtr.Zero)
                NativeVirConnect.Close(ConnectionPtr);
        }
        #endregion
    }
}
