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

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    /// <summary>
    /// The class expose some useful native functions
    /// </summary>
    internal class NativeFunctions
    {
        // TODO : this is a temporary workaround for virConnectOpenAuth callback, this should be removed
        /// <summary>
        /// duplicate a string. The strdup function shall return a pointer to a new string, which is a duplicate of the string pointed to by s1.
        /// </summary>
        /// <param name="strSource">Pointer to the string that should be duplicated</param>
        /// <returns>a pointer to a new string on success. Otherwise, it shall return a null pointer</returns>
        [DllImport("msvcrt.dll", EntryPoint = "_strdup", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr StrDup(IntPtr strSource);

        // TODO : this is a temporary workaround for virConnectOpenAuth callback, this should be removed
        [DllImport("msvcrt.dll", EntryPoint = "free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Free(IntPtr ptr);


        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr fopen(String filename, String mode);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern Int32 fclose(IntPtr file);
    }
}
