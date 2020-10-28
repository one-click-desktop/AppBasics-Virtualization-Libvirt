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
using IDNT.AppBasics.Virtualization.Libvirt.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    /// <summary>
    /// This is a struct used to simply the C# bindings, for C# bindings internal use only.
    /// </summary>
    public struct VirOpenAuthManagedCB
    {
        /// <summary>
        /// Pointer to user data of the ConnectOpenAuth
        /// </summary>
        public IntPtr cbdata;
        /// <summary>
        /// The C# delegate which must be called
        /// </summary>
        public ConnectAuthCallback cbManaged;
    }
}
