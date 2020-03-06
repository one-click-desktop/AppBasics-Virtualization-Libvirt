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

namespace Libvirt
{
    public class LibvirtConfiguration
    {
        // RH8 path layout for kvm
        public const string DEFAULT_QEMU_DOMAIN_LOGPATH = "/var/log/libvirt/qemu";
        public const string DEFAULT_QEMU_DOMAIN_RUNPATH = "/var/run/libvirt/qemu";
        public const string DEFAULT_QEMU_DOMAIN_ETCPATH = "/etc/libvirt/qemu";

        public const int DEFAULT_LIBVIRT_KEEPALIVE_INTERVAL = 6;

        public const int DEFAULT_LIBVIRT_KEEPALIVE_COUNT = 5;

        public LibvirtConfiguration()
        {
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
            get; set;
        }

        /// <summary>
        /// The path where qemu stores a domains logfile 
        /// </summary>
        public string QemuDomainLogPath
        {
            get; set;
        }

        /// <summary>
        /// The path where qemu stores domain configuration filed
        /// </summary>
        public string QemuDomainEtcPath
        {
            get; set;
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
    }
}
