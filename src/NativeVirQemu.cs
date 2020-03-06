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
using System.Runtime.InteropServices;

namespace Libvirt
{
    ///<summary>
    /// class for libvirt qemu specific methods
    ///</summary>
    public class NativeVirQemu
    {
        /// <summary>
        /// The error object is kept in thread local storage, so separate threads can safely access this concurrently.
        /// Reset the last error caught on that connection.
        /// </summary>
        /// <param name="conn">
        /// A <see cref="IntPtr"/> pointer to the hypervisor connection.
        /// </param>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virDomainQemuMonitorCommand")]
        public static extern int MonitorCommand(IntPtr domain, string cmd, out string result, VirDomainQemuMonitorCommandFlags flags);
    }
}
