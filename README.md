# About

This provides some enhancement to the raw bindings provided by 
Libvirt Bindings (https://libvirt.org/git/?p=libvirt-csharp.git).

Arnaud Champion and Jaromír Červenka have done a wonderful job in providing 
libvirt bindings for .NET, but we thought it would be useful to have a little 
more .NETish interface for working with libvirt. 

This is still a work in progress, so please expect some breaking changes for 
the time being. As soon as this is ready for prime time will ask the original 
authors or the libvirt team if they like to add it to the libvirt project.

Please ignore the MonoDevelop solution for now. We did not maintain it. The
code runs fine on mono (tested with mono 6.8 on RHEL 8).

Contributions are highly appreciated.
 
# Documentation
 
The following code should give you a head start: 

```c#
private static void Connection_DomainEventReceived(object sender, VirDomainEventArgs e)
{
    var domain = (LibvirtDomain)sender; // Note: this is null on undefined event
    Console.WriteLine($"EVENT: {e.UniqueId} {domain?.Name} {e.EventType.ToString()}");
}

static void Main(string[] args)
{
    using (var connection = LibvirtConnection.Open())
    {
        connection.DomainEventReceived += Connection_DomainEventReceived;

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
        Console.WriteLine("Waiting for domain lifecycle events...");
        Console.WriteLine("[ENTER] to exit");
        Console.ReadLine();
    }
}
```

Once again, feel free to contribute!

# Nuget package

A NuGet package is now also available:

```PS
Install-Package libvirt-dotnet
```
