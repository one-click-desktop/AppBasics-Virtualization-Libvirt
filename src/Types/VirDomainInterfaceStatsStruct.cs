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
    ///<summary>
    /// Domain interface statistics
    ///</summary>
    [StructLayout(LayoutKind.Sequential)]
    public class VirDomainInterfaceStatsStruct
    {
        ///<summary>
        /// Bytes received
        ///</summary>
        [MarshalAs(UnmanagedType.I8)]
        public long rx_bytes;
        /// <summary>
        /// Packets received
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long rx_packets;
        /// <summary>
        /// Errors received
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long rx_errs;
        /// <summary>
        /// Drops received
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long rx_drop;
        /// <summary>
        /// Bytes sended
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long tx_bytes;
        /// <summary>
        /// Packets sended
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long tx_packets;
        /// <summary>
        /// Errors sended
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long tx_errs;
        /// <summary>
        /// Drops sended
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long tx_drop;
    }
}
