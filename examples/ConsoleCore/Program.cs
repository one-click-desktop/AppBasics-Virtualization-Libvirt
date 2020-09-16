using Libvirt;
using System;

namespace ConsoleCore
{
    class Program
    {
        private static void Connection_DomainEventReceived(object sender, VirDomainEventArgs e)
        {
            var domain = (LibvirtDomain)sender; // Note: this is null on undefined event
            Console.WriteLine($"EVENT: {e.UniqueId} {domain?.Name} {e.EventType.ToString()}");
        }

        static void Main(string[] args)
        {
            using (var connection = LibvirtConnection.Create.WithLocalAuth().WithMetricsDisabled().Connect())
            {
                connection.DomainEventReceived += Connection_DomainEventReceived;

                Console.WriteLine($"Connected to node {connection.Node.Hostname}");
                Console.WriteLine(Environment.NewLine+"[Node]");
                Console.WriteLine($"   Total Memory ..........: {connection.Node.MemoryKBytes/1024/1024} GB");
                Console.WriteLine($"   Free Memory ...........: {connection.Node.MemFreeBytes / 1024 / 1024 / 1024} GB");
                Console.WriteLine($"   CPU Model .............: {connection.Node.CpuModelName}");
                Console.WriteLine($"   CPU Frequency .........: {connection.Node.CpuFrequencyMhz} MHz");
                Console.WriteLine($"   CPU NUMA Nodes ........: {connection.Node.CpuNumaNodes}");
                Console.WriteLine($"   CPU Sockets per Node ..: {connection.Node.CpuSocketsPerNode}");
                Console.WriteLine($"   CPU Cores per Socket ..: {connection.Node.CpuCoresPerSocket}");
                Console.WriteLine($"   CPU Threads per Core ..: {connection.Node.CpuThreadsPerCore}");

                Console.WriteLine(Environment.NewLine + "[Pools]");
                foreach (var pool in connection.StoragePools)
                {
                    Console.WriteLine($"   {pool.Name} (state={pool.State} driver={pool.DriverType}) {pool.CapacityInByte / 1024 / 1024 / 1024} GB ({pool.ByteAvailable/1024/1024/1024} GB free)");
                    foreach (var volume in pool.Volumes)
                        Console.WriteLine($"      Volume {volume.Name} (type={volume.VolumeType}, path={volume.Path}) {volume.CapacityInByte / 1024 / 1024 / 1024} GB ({volume.ByteAllocated/1024/1024/1024} GB allocated)");
                }

                Console.WriteLine(Environment.NewLine + "[Domains]");
                foreach (var domain in connection.Domains)
                {
                    Console.WriteLine($"   {domain.Name} ({domain.UniqueId}) {domain.State} osInfo={domain.OsInfoId} Up={TimeSpan.FromSeconds(domain.UptimeSeconds).ToString()}");
                    Console.WriteLine($"      (machineType={domain.MachineType}, arch={domain.MachineArch}, driverType={domain.DriverType}, memory={domain.MemoryMaxKbyte/1024} MB)");
                    Console.WriteLine("      [NIC]");
                    foreach (var nic in domain.NetworkInterfaces)
                        Console.WriteLine($"         {nic.Address.ToString()} bridge={nic.Source.Network}, mac={nic.MAC.Address}");
                    Console.WriteLine("      [Disks]");
                    foreach (var dev in domain.DiskDevices)
                        Console.WriteLine($"         {dev.Address.ToString()} {dev.Device} (driver={dev.Driver}) target={dev.Target.ToString()} source={dev.Source?.GetPath()}");
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine("[ENTER] to exit");
                Console.ReadLine();
            }
        }
    }
}
