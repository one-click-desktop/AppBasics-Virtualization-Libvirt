using Libvirt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleTest
{
    class Program
    {
        //static private string[] GetDomainInterfaces(string xmlDomainDescription)
        //{
        //    XmlDocument xmlDescription = new XmlDocument();
        //    xmlDescription.LoadXml(xmlDomainDescription);
        //    XmlNodeList iFaceNodeList = xmlDescription.SelectNodes("//domain/devices/interface");

        //    return (from XmlNode xn in iFaceNodeList select xn.SelectSingleNode("target").Attributes["dev"].Value).ToArray();
        //}

        //static Tuple<string, string>[] GetDomainBlockDevices(string xmlDomainDescription)
        //{
        //    XmlDocument xmlDescription = new XmlDocument();
        //    xmlDescription.LoadXml(xmlDomainDescription);
        //    XmlNodeList devNodeList = xmlDescription.SelectNodes("//domain/devices/disk[@device='disk']");

        //    return (from XmlNode xn in devNodeList select new Tuple<string,string>(xn.SelectSingleNode("source").Attributes["name"].Value, xn.SelectSingleNode("target").Attributes["dev"].Value)).ToArray();
        //}

        //static private void DomainEventCallback(IntPtr conn, IntPtr dom, Libvirt.DomainEventType evt, int detail, IntPtr opaque)
        //{
        //    var domUUID = new char[16];

        //    Guid domGuid = Guid.Empty;
        //    if (Libvirt.Domain.GetUUID(dom, domUUID) != -1)
        //        domGuid = new Guid(domUUID.Select(t => Convert.ToByte(t)).ToArray());

        //    if (Guid.Empty.Equals(domGuid))
        //    {
        //        Console.WriteLine("Event::ERROR -> Received event from unknown domain.");
        //        return;
        //    }

        //    switch (evt)
        //    {
        //        case Libvirt.DomainEventType.VIR_DOMAIN_EVENT_DEFINED:
        //            // New domain created
        //            Console.WriteLine("Event::VIR_DOMAIN_EVENT_DEFINED");

        //            switch ((Libvirt.DomainEventDefinedDetailType)detail)
        //            {
        //                case Libvirt.DomainEventDefinedDetailType.VIR_DOMAIN_EVENT_DEFINED_ADDED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_DEFINED (ADDED)");
        //                    break;
        //                case Libvirt.DomainEventDefinedDetailType.VIR_DOMAIN_EVENT_DEFINED_UPDATED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_DEFINED (UPDATED)");
        //                    break;
        //            }
        //            break;
        //        case Libvirt.DomainEventType.VIR_DOMAIN_EVENT_UNDEFINED:
        //            // Domain deleted
        //            Console.WriteLine("Event::VIR_DOMAIN_EVENT_UNDEFINED");

        //            switch ((Libvirt.DomainEventUndefinedDetailType)detail)
        //            {
        //                case Libvirt.DomainEventUndefinedDetailType.VIR_DOMAIN_EVENT_UNDEFINED_REMOVED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_UNDEFINED (REMOVED)");
        //                    break;
        //            }
        //            break;
        //        case Libvirt.DomainEventType.VIR_DOMAIN_EVENT_STARTED:
        //            // Domain started
        //            Console.WriteLine("Event::VIR_DOMAIN_EVENT_STARTED");

        //            switch ((Libvirt.DomainEventStartedDetailType)detail)
        //            {
        //                case Libvirt.DomainEventStartedDetailType.VIR_DOMAIN_EVENT_STARTED_BOOTED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_STARTED (BOOTED)");
        //                    break;
        //                case Libvirt.DomainEventStartedDetailType.VIR_DOMAIN_EVENT_STARTED_MIGRATED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_STARTED (MIGRATED)");
        //                    break;
        //                case Libvirt.DomainEventStartedDetailType.VIR_DOMAIN_EVENT_STARTED_RESTORED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_STARTED (RESTORED)");
        //                    break;
        //            }
        //            break;
        //        case Libvirt.DomainEventType.VIR_DOMAIN_EVENT_SUSPENDED:
        //            // Domain suspended
        //            Console.WriteLine("Event::VIR_DOMAIN_EVENT_SUSPENDED");

        //            switch ((Libvirt.DomainEventSuspendedDetailType)detail)
        //            {
        //                case Libvirt.DomainEventSuspendedDetailType.VIR_DOMAIN_EVENT_SUSPENDED_MIGRATED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_SUSPENDED (MIGRATED)");
        //                    break;
        //                case Libvirt.DomainEventSuspendedDetailType.VIR_DOMAIN_EVENT_SUSPENDED_PAUSED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_SUSPENDED (PAUSED)");
        //                    break;
        //            }
        //            break;
        //        case Libvirt.DomainEventType.VIR_DOMAIN_EVENT_RESUMED:
        //            // Domain resumed
        //            Console.WriteLine("Event::VIR_DOMAIN_EVENT_RESUMED");

        //            switch ((Libvirt.DomainEventResumedDetailType)detail)
        //            {
        //                case Libvirt.DomainEventResumedDetailType.VIR_DOMAIN_EVENT_RESUMED_MIGRATED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_RESUMED (MIGRATED)");
        //                    break;
        //                case Libvirt.DomainEventResumedDetailType.VIR_DOMAIN_EVENT_RESUMED_UNPAUSED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_RESUMED (UNPASUED)");
        //                    break;
        //            }
        //            break;
        //        case Libvirt.DomainEventType.VIR_DOMAIN_EVENT_STOPPED:
        //            // Domain stopped
        //            Console.WriteLine("Event::VIR_DOMAIN_EVENT_STOPPED");

        //            switch ((Libvirt.DomainEventStoppedDetailType)detail)
        //            {
        //                case Libvirt.DomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_CRASHED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_STOPPED (CRASHED)");
        //                    break;
        //                case Libvirt.DomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_DESTROYED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_STOPPED (DESTROYED)");
        //                    break;
        //                case Libvirt.DomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_FAILED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_STOPPED (FAILED)");
        //                    break;
        //                case Libvirt.DomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_MIGRATED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_STOPPED (MIGRATED)");
        //                    break;
        //                case Libvirt.DomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_SAVED:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_STOPPED (SAVED)");
        //                    break;
        //                case Libvirt.DomainEventStoppedDetailType.VIR_DOMAIN_EVENT_STOPPED_SHUTDOWN:
        //                    Console.WriteLine("Event::VIR_DOMAIN_EVENT_STOPPED (SHUTDOWN)");
        //                    break;
        //            }
        //            break;
        //    }
        //}

        //static private void DomainEventFreeFunc(IntPtr opaque)
        //{
        //    Console.WriteLine("DomainEventFreeFunc()");
        //}

        //static private int _fd;
        //static private EventHandleType _event;
        //static private EventHandleCallback _cb;
        //static private FreeCallback _ff;
        //static private IntPtr _opaque;
        //static private int _active;
        //static private int _timeout;
        //static private EventTimeoutCallback _tcb;

        //static private System.Threading.Timer _callbackTimer;

        //static private void TimerCallback(object state)
        //{
        //    if (_tcb != null && _active == 1)
        //        _tcb(_timeout, _opaque);

        //    if (_cb != null)
        //    {
        //        _cb(0,
        //             _fd,
        //             (int)_event,
        //             _opaque);
        //    }
        //}

        //static private int RemoveHandleFunc(int watch)
        //{
        //    Console.WriteLine("RemoveHandleFunc()");
        //    _fd = 0;
        //    if (_ff != null)
        //        _ff(_opaque);
        //    return 0;
        //}
        //static private void UpdateHandleFunc(int watch, int events)
        //{
        //    //Console.WriteLine("UpdateHandleFunc()");
        //    _event = (EventHandleType)events;
        //}

        //static private int AddHandleFunc(int fd, int events, EventHandleCallback cb, IntPtr opaque, FreeCallback ff)
        //{
        //    Console.WriteLine("AddHandleFunc()");
        //    _fd = fd;
        //    _event = (EventHandleType)events;
        //    _cb = cb;
        //    _ff = ff;
        //    _opaque = opaque;
        //    return 0;
        //}

        //static private int RemoveTimeoutFunc(int timer)
        //{
        //    Console.WriteLine("RemoveTimeoutFunc()");
        //    _active = 0;
        //    if (_ff != null)
        //        _ff(_opaque);
        //    return 0;
        //}

        //static private void UpdateTimeoutFunc(int timer, int timeout)
        //{
        //    //Console.WriteLine("UpdateTimeoutFunc()");
        //    _timeout = timeout;
        //}

        //static private int AddTimeoutFunc(int timeout, EventTimeoutCallback cb, IntPtr opaque, FreeCallback ff)
        //{
        //    Console.WriteLine("AddTimeoutFunc()");
        //    _active = 1;
        //    _timeout = timeout;
        //    _tcb = cb;
        //    _ff = ff;
        //    _opaque = opaque;
        //    return 0;
        //}

        //static private IntPtr conn = IntPtr.Zero;

        private static void Connection_DomainEventReceived(object sender, VirDomainEventArgs e)
        {
            var domain = (LibvirtDomain)sender; // Note: this is null on undefined event
            Console.WriteLine($"EVENT: {e.UniqueId} {domain?.Name} {e.EventType.ToString()}");
        }

        private static void Connection_StoragePoolLifecycleEventReceived(object sender, VirStoragePoolLifecycleEventArgs e)
        {
            var storagePool = (LibvirtStoragePool)sender;
            Console.WriteLine($"STORAGE POOL EVENT: {e.UniqueId} {storagePool?.Name} {e.EventType.ToString()}");
        }

        private static void Connection_StoragePoolRefreshEventReceived(object sender, VirStoragePoolRefreshEventArgs e)
        {
            var storagePool = (LibvirtStoragePool)sender;
            Console.WriteLine($"STORAGE POOL EVENT: {e.UniqueId} {storagePool.Name} REFRESHED");
        }

        static void Main(string[] args)
        {
            using (var connection = LibvirtConnection.Open())
            {
                connection.DomainEventReceived += Connection_DomainEventReceived;
                connection.StoragePoolLifecycleEventReceived += Connection_StoragePoolLifecycleEventReceived;
                connection.StoragePoolRefreshEventReceived += Connection_StoragePoolRefreshEventReceived;

                Console.WriteLine();
                Console.WriteLine("[DOMAINS]");
                foreach (var domain in connection.Domains)
                {
                    Console.WriteLine($"{domain.UniqueId} {domain.Name} {domain.State}");
                }

                Console.WriteLine();
                Console.WriteLine("[STORAGE POOLS]");
                foreach (var domain in connection.StoragePools)
                {
                    Console.WriteLine($"{domain.UniqueId} {domain.Name} {domain.State} Capacity={domain.CapacityInByte/1024/1024/1024} GiB");
                }

                Console.WriteLine();
                Console.WriteLine("[ENTER] to exit");
                Console.ReadLine();
            }

            //_callbackTimer = new System.Threading.Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);

            ////Event.RegisterImpl(AddHandleFunc, UpdateHandleFunc, RemoveHandleFunc, AddTimeoutFunc, UpdateTimeoutFunc,
            ////               RemoveTimeoutFunc);

            //if (Libvirt.Event.RegisterDefaultImpl() != 0)
            //    Console.WriteLine("WARNING: Could not register default event loop implementation.");

            //IntPtr conn = Connect.Open("qemu:///system");
            //if (conn != IntPtr.Zero)
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("[Virtual Machines]");
            //    int nbDomains = Connect.NumOfDomains(conn);
            //    int[] domainIDs = new int[nbDomains];
            //    Connect.ListDomains(conn, domainIDs, nbDomains);
            //    foreach (IntPtr domainPtr in domainIDs.Select(domainID => Domain.LookupByID(conn, domainID)))
            //    {
            //        DomainInfo info = new DomainInfo();
            //        if (Domain.GetInfo(domainPtr, info) != -1)
            //        {
            //            var name = Domain.GetName(domainPtr);

            //            Console.WriteLine($"{Domain.GetID(domainPtr)} {name} {info.State} {info.memory} Kbyte {info.nrVirtCpu} cpus");

            //            //var xmlDescr = Domain.GetXMLDesc(domainPtr, 0);
            //            //Console.WriteLine(xmlDescr.ToString());

            //            //foreach (var dev in GetDomainBlockDevices(xmlDescr))
            //            //{
            //            //    DomainBlockStatsStruct blockStat;
            //            //    Domain.BlockStats(domainPtr, dev.Item2, out blockStat);

            //            //    var sourceParts = dev.Item1.Split('/');
            //            //    var pool = StoragePool.LookupByName(conn, sourceParts[0]+"-pool");

            //            //    var volume = StorageVol.LookupByName(pool, sourceParts[1]);
            //            //    StorageVolInfo volInfo = new StorageVolInfo();

            //            //    if (StorageVol.GetInfo(volume, ref volInfo) != -1)
            //            //    {
            //            //        Console.WriteLine($"\t{dev.Item1} {volInfo.type} {volInfo.capacity / 1024 / 1024 / 1024} GB ");
            //            //    }

            //            //    StorageVol.Free(volume);
            //            //    StoragePool.Free(pool);
            //            //}
            //        }

            //        Domain.Free(domainPtr);
            //    }

            //    nbDomains = Connect.NumOfDefinedDomains(conn);
            //    string[] domainNames = new string[nbDomains];
            //    Connect.ListDefinedDomains(conn, ref domainNames, nbDomains);
            //    foreach (IntPtr domainPtr in domainNames.Select(domainName => Domain.LookupByName(conn, domainName)))
            //    {
            //        DomainInfo info = new DomainInfo();
            //        if (Domain.GetInfo(domainPtr, info) != -1)
            //        {
            //            var name = Domain.GetName(domainPtr);

            //            Console.WriteLine($"{Domain.GetID(domainPtr)} {name} {info.State} {info.memory} Kbyte {info.nrVirtCpu} cpus");

            //            //var xmlDescr = Domain.GetXMLDesc(domainPtr, 0);
            //            //Console.WriteLine(xmlDescr.ToString());

            //            //foreach (var dev in GetDomainBlockDevices(xmlDescr))
            //            //{
            //            //    DomainBlockStatsStruct blockStat;
            //            //    Domain.BlockStats(domainPtr, dev.Item2, out blockStat);

            //            //    var sourceParts = dev.Item1.Split('/');
            //            //    var pool = StoragePool.LookupByName(conn, sourceParts[0]+"-pool");

            //            //    var volume = StorageVol.LookupByName(pool, sourceParts[1]);
            //            //    StorageVolInfo volInfo = new StorageVolInfo();

            //            //    if (StorageVol.GetInfo(volume, ref volInfo) != -1)
            //            //    {
            //            //        Console.WriteLine($"\t{dev.Item1} {volInfo.type} {volInfo.capacity / 1024 / 1024 / 1024} GB ");
            //            //    }

            //            //    StorageVol.Free(volume);
            //            //    StoragePool.Free(pool);
            //            //}
            //        }

            //        Domain.Free(domainPtr);
            //    }

            //    Console.WriteLine();
            //    Console.WriteLine("[Storage Pools]");
            //    int numOfStoragePools = Connect.NumOfStoragePools(conn);
            //    if (numOfStoragePools != -1)
            //    {
            //        string[] storagePoolsNames = new string[numOfStoragePools];
            //        int listStoragePools = Connect.ListStoragePools(conn, ref storagePoolsNames, numOfStoragePools);
            //        if (listStoragePools != -1)
            //        {
            //            foreach (string storagePoolName in storagePoolsNames)
            //            {
            //                var pool = StoragePool.LookupByName(conn, storagePoolName);

            //                StoragePoolInfo poolInfo = new StoragePoolInfo();
            //                if (StoragePool.GetInfo(pool, ref poolInfo) != -1)
            //                {
            //                    Console.WriteLine($"{storagePoolName}: {poolInfo.capacity / 1024 / 1024 / 1024} GB Capacity, {poolInfo.available / 1024 / 1024 / 1024} GB Free");

            //                    //int numOfVolumes = StoragePool.NumOfVolumes(pool);
            //                    //string[] volumeNames = new string[numOfVolumes];

            //                    //if (StoragePool.ListVolumes(pool, ref volumeNames, numOfVolumes) != -1)
            //                    //{
            //                    //    foreach (var volumeName in volumeNames)
            //                    //    {
            //                    //        var volume = StorageVol.LookupByName(pool, volumeName);
            //                    //        StorageVolInfo volInfo = new StorageVolInfo();

            //                    //        if (StorageVol.GetInfo(volume, ref volInfo) != -1)
            //                    //        {
            //                    //            Console.WriteLine($"\t{volumeName} {volInfo.type} {volInfo.capacity / 1024 / 1024 / 1024} GB ");
            //                    //        }

            //                    //        StorageVol.Free(volume);
            //                    //    }
            //                    //}
            //                }
            //                StoragePool.Free(pool);
            //            }
            //        }
            //    }

            //    if (Libvirt.Connect.DomainEventRegister(conn, DomainEventCallback, IntPtr.Zero, DomainEventFreeFunc) != 0)
            //        Console.WriteLine("WARNING: Could not install evet listener.");

            //    Libvirt.Connect.SetKeepAlive(conn, 5, 3);
            //    _callbackTimer.Change(0, 50);

            //    Console.WriteLine();
            //    Console.WriteLine("[ENTER] to exit");
            //    while(Libvirt.Connect.IsAlive(conn) == 1)
            //    {
            //        Libvirt.Event.RunDefaultImpl();
            //        if (Console.KeyAvailable)
            //            break;
            //    }

            //    Connect.Close(conn);
            //}
            //else
            //{
            //    Console.WriteLine(Errors.GetLastErrorMessage());
            //}
        }

    }
}
