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
    /// <summary>
    /// The Interface class expose all interface related methods
    /// </summary>
    public class NativeVirInterface
    {
        // TODO virInterfaceCreate

        // TODO virInterfaceDefineXML

        // TODO virInterfaceDestroy

        /// <summary>
        /// Free the interface object. The interface itself is unaltered. The data structure is freed and should not be used thereafter.
        /// </summary>
        /// <param name="iface">an interface object</param>
        /// <returns>0 in case of success and -1 in case of failure</returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virInterfaceFree")]
        public static extern int Free(IntPtr iface);

        // TODO virInterfaceGetConnect

        // TODO virInterfaceGetMACString

        // TODO virInterfaceGetName

        /// <summary>
        /// Provide an XML description of the interface. If VIR_INTERFACE_XML_INACTIVE is set, the description may be reused later to redefine the interface with virInterfaceDefineXML(). If it is not set, the ip address and netmask will be the current live setting of the interface, not the settings from the config files.
        /// </summary>
        /// <param name="iface">an interface object</param>
        /// <param name="flags">bitwise-OR of extraction flags.</param>
        /// <returns>a 0 terminated UTF-8 encoded XML instance, or NULL in case of error. The caller must free() the returned value.</returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virInterfaceGetXMLDesc")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringWithoutNativeCleanUpMarshaler))]
        public static extern string GetXMLDesc(IntPtr iface, int flags);

        // TODO virInterfaceIsActive

        // TODO virInterfaceLookupByMACString

        /// <summary>
        /// Try to lookup an interface on the given hypervisor based on its name.
        /// </summary>
        /// <param name="conn">pointer to the hypervisor connection</param>
        /// <param name="name">name for the interface</param>
        /// <returns>a new interface object or NULL in case of failure. If the interface cannot be found, then VIR_ERR_NO_INTERFACE error is raised.</returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virInterfaceLookupByName")]
        public static extern IntPtr LookupByName(IntPtr conn, string name);

        // TODO virInterfaceRef

        // TODO virInterfaceUndefine
    }
}
