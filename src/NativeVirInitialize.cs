/*
 * Copyright (C)
 *   Arnaud Champion <arnaud.champion@devatom.fr>
 *   Jaromír Červenka <cervajz@cervajz.com>
 *
 * See COPYING.LIB for the License of this software
 */

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;


namespace Libvirt
{
    internal class NativeVirInitialize
    {
        private static bool _isInitialized = false;

#if NETCORE3
        internal static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        internal static bool IsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        internal static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#endif

        static internal void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

#if NETCORE3
            NativeLibrary.SetDllImportResolver(typeof(NativeVirInitialize).Assembly, MapAndLoad);
#endif
            NativeVirLibrary.InitializeLib();
        }

#if NETCORE
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
