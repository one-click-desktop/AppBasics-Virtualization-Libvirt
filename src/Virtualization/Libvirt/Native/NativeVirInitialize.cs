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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;


namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    internal class NativeVirInitialize
    {
        private static bool _isInitialized = false;

#if NET
        internal static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        internal static bool IsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        internal static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#endif

        static internal void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

#if NET
            NativeLibrary.SetDllImportResolver(typeof(NativeVirInitialize).Assembly, MapAndLoad);
#endif
            NativeVirLibrary.InitializeLib();
        }

#if NET
        private static IntPtr MapAndLoad(string libraryName, Assembly assembly, DllImportSearchPath? dllImportSearchPath)
        {
            string mappedName = libraryName;
            switch(libraryName)
            {
                case "libvirt-0.dll":
                    if (IsLinux())
                        mappedName = "libvirt.so.0";
                    else if (IsMacOS())
                        mappedName = "libvirt-0.dylib";
                    else
                        mappedName = "libvirt-0.dll";
                    break;
                case "libvirt-qemu-0.dll":
                    if (IsLinux())
                        mappedName = "libvirt-qemu.so.0";
                    else if (IsMacOS())
                        mappedName = "libvirt-qemu-0.dylib";
                    else
                        mappedName = "libvirt-qemu-0.dll";
                    break;
                case "msvcrt.dll":
                     if (IsLinux())
                        mappedName = "libvirt.so.0";
                    else if (IsMacOS())
                        mappedName = "libc-0.dylib";
                    else
                        mappedName = "msvcrt.dll";
                    break;
            }
            return NativeLibrary.Load(mappedName, assembly, dllImportSearchPath);
        }
#endif
    }
}
