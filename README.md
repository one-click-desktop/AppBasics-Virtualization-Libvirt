# About

Libvirt-dotnet provides a .NETish interface to the libvirt virtualization API. 
The original code is base on the raw bindings provided by Libvirt Bindings (https://libvirt.org/git/?p=libvirt-csharp.git).

Arnaud Champion and Jaromír Červenka have done a wonderful job in providing 
libvirt bindings for .NET, but we thought it would be useful to have a little 
more .NETish interface for working with libvirt. 

Note that this is still a work in progress. Expect some breaking changes for 
the time being. The code runs fine on mono and dotnet core (tested with mono 
6.8 and dotnet-sdk 3.1 on RHEL 8). Operations are thread-safe.

# Nuget package

A NuGet package is now also available at https://www.nuget.org/packages/libvirt-dotnet

```PS
Install-Package libvirt-dotnet
```

 
# Documentation
 
The following code should give you a head start: 

## Example 1

The following code will output a list of domains and storage pools. Thereafter it 
waits for domain events like stopped, started, ... until you hit ENTER.

```c#
private static void Connection_DomainEventReceived(object sender, VirDomainEventArgs e)
{
    var domain = (LibvirtDomain)sender; // Note: this is null on undefine event
    Console.WriteLine($"EVENT: {e.UniqueId} {domain?.Name} {e.EventType}");
}

static void Main(string[] args)
{
    using (var connection = LibvirtConnection.Open())
    {
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
			Console.WriteLine($"{domain.UniqueId} {domain.Name} {domain.State}");
		}

        Console.WriteLine();
        Console.WriteLine("Waiting for domain lifecycle events...");
        connection.DomainEventReceived += Connection_DomainEventReceived;

        Console.WriteLine();
        Console.WriteLine("[ENTER] to exit");
        Console.ReadLine();
    }
}
```

## Example 2

Getting the domains CPU utilization in percent is as easy as:

```c#
    using (var connection = LibvirtConnection.Open())
    {
		var d = domain in connection.Domains.Where(t => t.Name == 'MyVM').First();
		
		while(! Console.KeyAvailable)
		{
		    Console.WriteLine($"{d.Name}'s CPU Utilization = {d.CpuUtilization.LastSecond}%");
		    Thread.Sleep(1000);
		}
	}
```

## Example 3

You need a screenshot of a domains console? Here you go:

```c#
    using (var connection = LibvirtConnection.Open())
    {
		var d = domain in connection.Domains.Where(t => t.Name == 'MyVM').First();
		
		using(var fs = new FileStream("image.jpg", FileMode.Create))
			d.GetScreenshot(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
	}
```

# Version History

1.0.1.4 Added donet core 3.1 support 
1.0.1.5 Latest nuget package

# License

This code is licensed under the GNU Lesser General Public Library, Version 2.1 (the "License"). 
You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-2.1.en.html

# Copyright

The original libvirt-csharp code was written by Arnaud Champion and Jaromír Červenka and can be found at
https://libvirt.org/git/?p=libvirt-csharp.git

# Contributions 

The libvirt-dotnet repository is maintained by IDNT [https://www.idnt.net/]

Contributions of all kinds are always welcome.
