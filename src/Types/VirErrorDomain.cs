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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libvirt
{
    /// <summary>
    /// Enumrate types of domain errors
    /// </summary>
    public enum VirErrorDomain
    {
        /// <summary>
        /// None
        /// </summary>
        VIR_FROM_NONE = 0,
        /// <summary>
        /// Error at Xen hypervisor layer
        /// </summary>
        VIR_FROM_XEN = 1,
        /// <summary>
        /// Error at connection with xend daemon
        /// </summary>
        VIR_FROM_XEND = 2,
        /// <summary>
        /// Error at connection with xen store
        /// </summary>
        VIR_FROM_XENSTORE = 3,
        /// <summary>
        /// Error in the S-Expression code
        /// </summary>
        VIR_FROM_SEXPR = 4,
        /// <summary>
        /// Error in the XML code
        /// </summary>
        VIR_FROM_XML = 5,
        /// <summary>
        /// Error when operating on a domain
        /// </summary>
        VIR_FROM_DOM = 6,
        /// <summary>
        /// Error in the XML-RPC code
        /// </summary>
        VIR_FROM_RPC = 7,
        /// <summary>
        /// Error in the proxy code
        /// </summary>
        VIR_FROM_PROXY = 8,
        /// <summary>
        /// Error in the configuration file handling
        /// </summary>
        VIR_FROM_CONF = 9,
        /// <summary>
        /// Error at the QEMU daemon
        /// </summary>
        VIR_FROM_QEMU = 10,
        /// <summary>
        /// Error when operating on a network
        /// </summary>
        VIR_FROM_NET = 11,
        /// <summary>
        /// Error from test driver
        /// </summary>
        VIR_FROM_TEST = 12,
        /// <summary>
        /// Error from remote driver
        /// </summary>
        VIR_FROM_REMOTE = 13,
        /// <summary>
        /// Error from OpenVZ driver
        /// </summary>
        VIR_FROM_OPENVZ = 14,
        /// <summary>
        /// Error at Xen XM layer
        /// </summary>
        VIR_FROM_XENXM = 15,
        /// <summary>
        /// Error in the Linux Stats code
        /// </summary>
        VIR_FROM_STATS_LINUX = 16,
        /// <summary>
        /// Error from Linux Container driver
        /// </summary>
        VIR_FROM_LXC = 17,
        /// <summary>
        /// Error from storage driver
        /// </summary>
        VIR_FROM_STORAGE = 18,
        /// <summary>
        /// Error from network config
        /// </summary>
        VIR_FROM_NETWORK = 19,
        /// <summary>
        /// Error from domain config
        /// </summary>
        VIR_FROM_DOMAIN = 20,
        /// <summary>
        /// Error at the UML driver
        /// </summary>
        VIR_FROM_UML = 21,
        /// <summary>
        /// Error from node device monitor
        /// </summary>
        VIR_FROM_NODEDEV = 22,
        /// <summary>
        /// Error from xen inotify layer
        /// </summary>
        VIR_FROM_XEN_INOTIFY = 23,
        /// <summary>
        /// Error from security framework
        /// </summary>
        VIR_FROM_SECURITY = 24,
        /// <summary>
        /// Error from VirtualBox driver
        /// </summary>
        VIR_FROM_VBOX = 25,
        /// <summary>
        /// Error when operating on an interface
        /// </summary>
        VIR_FROM_INTERFACE = 26,
        /// <summary>
        /// Error from OpenNebula driver
        /// </summary>
        VIR_FROM_ONE = 27,
        /// <summary>
        /// Error from ESX driver
        /// </summary>
        VIR_FROM_ESX = 28,
        /// <summary>
        /// Error from IBM power hypervisor
        /// </summary>
        VIR_FROM_PHYP = 29,
        /// <summary>
        /// Error from secret storage
        /// </summary>
        VIR_FROM_SECRET = 30,
        /// <summary>
        /// Error from CPU driver
        /// </summary>
        VIR_FROM_CPU = 31,
        /// <summary>
        /// Error from XenAPI
        /// </summary>
        VIR_FROM_XENAPI = 32,
        /// <summary>
        /// Error from network filter driver
        /// </summary>
        VIR_FROM_NWFILTER = 33,
        /// <summary>
        /// Error from Synchronous hooks
        /// </summary>
        VIR_FROM_HOOK = 34,
        /// <summary>
        /// Error from domain snapshot
        /// </summary>
        VIR_FROM_DOMAIN_SNAPSHOT = 35
    }
}
