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
using IDNT.AppBasics.Virtualization.Libvirt.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    /// <summary>
    /// A callback function to be registered, and called when a storage event occurs
    /// </summary>
    /// <param name="conn">virConnect connection </param>
    /// <param name="pool">The storage pool on which the event occured</param>
    /// <param name="opaque">opaque user data</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void VirConnectStoragePoolGenericEventCallback(IntPtr conn, IntPtr pool, IntPtr opaque);

    /// <summary>
    /// A callback function to be registered, and called when a storage event occurs
    /// </summary>
    /// <param name="conn">virConnect connection </param>
    /// <param name="pool">The storage pool on which the event occured</param>
    /// <param name="opaque">opaque user data</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void VirConnectStoragePoolEventLifecycleCallback(IntPtr conn, IntPtr pool, VirStoragePoolEventLifecycleType eventId, int detail, IntPtr opaque);

}
