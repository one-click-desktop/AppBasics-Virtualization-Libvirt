using Libvirt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleTest
{
    class Program
    {
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
                    Console.WriteLine($"{domain.UniqueId} {domain.Name} {domain.State} {domain.MachineType} {domain.GetGraphicsUri()}");

                    //foreach (var disk in domain.DiskDevices)
                    //    Console.WriteLine(connection.GetVolumeByDiskSource(disk.Source));
                    //Console.WriteLine(domain.GetGraphicsUri());
                }

                //Console.WriteLine();
                //Console.WriteLine("[STORAGE VOLUMES]");
                //foreach (var volume in connection.StorageVolumes)
                //{
                //    Console.WriteLine($"{volume.Name} [{volume.Key}] (Type={volume.VolumeType}, Capacity={volume.CapacityInByte / 1024 / 1024 / 1024} GiB, Pool={volume.StoragePool.Name}");
                //}

                //// Showing CPU utilization of first domain for 5 seconds
                var md = connection.Domains.Where(t => t.Name == "XOPS6").First();
                //for (int i = 0; i < 5; i++)
                //{
                //    Console.WriteLine($"{md.Name}'s CPU Utilization = {md.CpuUtilization.Current}%");
                //    Thread.Sleep(1000);
                //}

                using(var fs = new FileStream("image.jpg", FileMode.Create))
                    md.GetScreenshot(fs, System.Drawing.Imaging.ImageFormat.Jpeg);


                Console.WriteLine();
                Console.WriteLine("[ENTER] to exit");
                Console.ReadLine();
            }
        }

    }
}
