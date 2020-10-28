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
using System.Collections.Generic;
using System.Text;

namespace IDNT.AppBasics.Virtualization.Libvirt
{
    public class LibvirtConfiguration : IConfigurationBuilder
    {
        // RH8 path layout for kvm
        public const string DEFAULT_QEMU_DOMAIN_LOGPATH = "/var/log/libvirt/qemu";
        public const string DEFAULT_QEMU_DOMAIN_RUNPATH = "/var/run/libvirt/qemu";
        public const string DEFAULT_QEMU_DOMAIN_ETCPATH = "/etc/libvirt/qemu";

        public const int DEFAULT_LIBVIRT_KEEPALIVE_INTERVAL = 6;

        public const int DEFAULT_LIBVIRT_KEEPALIVE_COUNT = 5;

        public LibvirtConfiguration()
        {
            this.MetricsEnabled = true;
            this.EventsEnabled = true;
            this.KeepaliveInterval = DEFAULT_LIBVIRT_KEEPALIVE_INTERVAL;
            this.KeepaliveCount = DEFAULT_LIBVIRT_KEEPALIVE_COUNT;
            this.QemuDomainRunPath = DEFAULT_QEMU_DOMAIN_RUNPATH;
            this.QemuDomainLogPath = DEFAULT_QEMU_DOMAIN_LOGPATH;
            this.QemuDomainEtcPath = DEFAULT_QEMU_DOMAIN_ETCPATH;
        }
       

        #region Configuration Properties
        /// <summary>
        /// The path where qemu stores a domains runtime configuration data (pid file, ...)
        /// </summary>
        public string QemuDomainRunPath
        {
            get; private set;
        }

        /// <summary>
        /// The path where qemu stores a domains logfile 
        /// </summary>
        public string QemuDomainLogPath
        {
            get; private set;
        }

        /// <summary>
        /// The path where qemu stores domain configuration filed
        /// </summary>
        public string QemuDomainEtcPath
        {
            get; private set;
        }

        /// <summary>
        /// True if event are enabled (default).
        /// </summary>
        public bool EventsEnabled
        {
            get; private set;
        }

        /// <summary>
        /// True if metrics collection is enabled (default).
        /// </summary>
        public bool MetricsEnabled
        {
            get; private set;
        }

        /// <summary>
        /// Set to 0 to disable keepalive (default: LibvirtConfiguration.DEFAULT_LIBVIRT_KEEPALIVE_INTERVAL)
        /// </summary>
        public int KeepaliveInterval
        {
            get; private set;
        }

        public uint KeepaliveCount
        {
            get; private set;
        }

        public LibvirtAuthentication Credentials
        {
            get; private set;
        }
        #endregion

        #region Internal Callbacks
        internal Action<int> OnMetricsIntervalChanged;
        #endregion

        private int _metricsInterval = 1000;

        public int MetricsIntervalSeconds
        {
            get { return _metricsInterval / 1000; }
            set
            {
                if (value <= 0)
                {
                    _metricsInterval = 0;
                    OnMetricsIntervalChanged(_metricsInterval);
                    return;
                }
                _metricsInterval = value < 1 ? 1000 : value * 1000;
                OnMetricsIntervalChanged(_metricsInterval);
            }
        }

        #region IConfigurationBuilder implementation
        IConfigurationBuilder IConfigurationBuilder.WithQemuDomainLogPath(string path)
        {
            QemuDomainLogPath = path ?? throw new ArgumentNullException();
            return this;
        }

        IConfigurationBuilder IConfigurationBuilder.WithQemuDomainRunPath(string path)
        {
            QemuDomainRunPath = path ?? throw new ArgumentNullException();
            return this;
        }

        IConfigurationBuilder IConfigurationBuilder.WithQemuDomainEtcPath(string path)
        {
            QemuDomainEtcPath = path ?? throw new ArgumentNullException();
            return this;
        }

        IConfigurationBuilder IConfigurationBuilder.WithoutKeepalive()
        {
            KeepaliveInterval = 0;
            return this;
        }

        IConfigurationBuilder IConfigurationBuilder.WithKeepalive(int interval, uint count)
        {
            KeepaliveInterval = interval;
            KeepaliveCount = count;
            return this;
        }

        IConfigurationBuilder IConfigurationBuilder.WithMetricsEnabled()
        {
            MetricsEnabled = true;
            return this;
        }

        IConfigurationBuilder IConfigurationBuilder.WithMetricsDisabled()
        {
            MetricsEnabled = false;
            return this;
        }

        IConfigurationBuilder IConfigurationBuilder.WithEventsEnabled()
        {
            EventsEnabled = true;
            return this;
        }

        IConfigurationBuilder IConfigurationBuilder.WithEventsDisabled()
        {
            EventsEnabled = false;
            return this;
        }

        IConfigurationBuilder IConfigurationBuilder.WithCredentials(LibvirtAuthentication credentials)
        {
            this.Credentials = credentials ?? LibvirtAuthentication.Local();
            return this;
        }

        IConfigurationBuilder IConfigurationBuilder.WithLocalAuth()
        {
            this.Credentials = LibvirtAuthentication.Local();
            return this;
        }

        IConfigurationBuilder IConfigurationBuilder.WithOpenAuth(string username, string password, VirConnectFlags flags)
        {
            this.Credentials = LibvirtAuthentication.WithUsernameAndPassword(username, password, flags);
            return this;
        }

        static public readonly LibvirtConfiguration Defaults = new LibvirtConfiguration();

        LibvirtConfiguration IConfigurationBuilder.Configuration => this;

        LibvirtConnection IConfigurationBuilder.Connect(string uri, LibvirtAuthentication auth) 
        { 
            return new LibvirtConnection(((Credentials = (auth ?? Credentials)) ?? LibvirtAuthentication.Local()).Connect(uri, this), this); 
        }

        #endregion
    }
}
