using Libvirt.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libvirt
{
    public interface IConfigurationBuilder
    {
        IConfigurationBuilder WithQemuDomainLogPath(string path);
        IConfigurationBuilder WithQemuDomainRunPath(string path);
        IConfigurationBuilder WithQemuDomainEtcPath(string path);

        IConfigurationBuilder WithoutKeepalive();
        IConfigurationBuilder WithKeepalive(int interval, uint count = LibvirtConfiguration.DEFAULT_LIBVIRT_KEEPALIVE_COUNT);

        IConfigurationBuilder WithEventsDisabled();
        IConfigurationBuilder WithEventsEnabled();


        IConfigurationBuilder WithMetricsDisabled();
        IConfigurationBuilder WithMetricsEnabled();

        IConfigurationBuilder WithCredentials(LibvirtAuthentication credentials);
        IConfigurationBuilder WithLocalAuth();
        IConfigurationBuilder WithOpenAuth(string username, string password, VirConnectFlags flags = VirConnectFlags.Empty);

        LibvirtConfiguration Configuration { get; }

        LibvirtConnection Connect(string uri = @"qemu:///system", LibvirtAuthentication auth = null);
    }
}
