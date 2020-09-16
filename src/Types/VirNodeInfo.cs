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
using System.Runtime.InteropServices;
using System.Text;

namespace Libvirt
{
    /// <summary>
    /// Structure to handle node informations
    /// <seealso cref="https://libvirt.org/html/libvirt-libvirt-host.html#virNodeInfo"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class VirNodeInfo
    {
        /// <summary>
        /// String indicating the CPU model.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Model;
        /// <summary>
        /// Memory size in kilobytes
        /// </summary>
        [MarshalAs(UnmanagedType.U8)]
        public ulong Memory;
        /// <summary>
        /// The number of active CPUs.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint Cpus;
        /// <summary>
        /// expected CPU frequency, 0 if not known or on unusual architectures
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint Mhz;
        /// <summary>
        /// the number of NUMA cell, 1 for unusual NUMA topologies or uniform memory access; check capabilities XML for the actual NUMA topology
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint Nodes;
        /// <summary>
        /// number of CPU sockets per node if nodes > 1, 1 in case of unusual NUMA topology
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint Sockets;
        /// <summary>
        /// number of cores per socket, total number of processors in case of unusual NUMA topology
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint Cores;
        /// <summary>
        /// number of threads per core, 1 in case of unusual numa topology
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint Threads;
    }
}
