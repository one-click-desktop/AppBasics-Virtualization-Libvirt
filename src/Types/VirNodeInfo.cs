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
        [MarshalAs(UnmanagedType.SysUInt)]
        public ulong Memory;
        /// <summary>
        /// The number of active CPUs.
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)]
        public uint Cpus;
        /// <summary>
        /// Expected CPU frequency.
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)]
        public uint Mhz;
        /// <summary>
        /// The number of NUMA cell, 1 for uniform mem access.
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)]
        public uint Nodes;
        /// <summary>
        /// Number of CPU socket per node.
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)]
        public uint Sockets;
        /// <summary>
        /// Number of core per socket.
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)]
        public uint Cores;
        /// <summary>
        /// Number of threads per core.
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)]
        public uint Threads;
    }
}
