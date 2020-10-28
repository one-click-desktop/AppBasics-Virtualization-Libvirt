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
using System.Text;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    ///<summary>
    /// class for libvirt qemu specific methods
    ///</summary>
    public class NativeVirQemu
    {
        private const int MaxStringLength = 1024;

        [DllImport("libvirt-qemu-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virDomainQemuMonitorCommand")]
        private static extern int MonitorCommandImpl(IntPtr domain, string cmd, [Out] StringBuilder result, uint flags);

        public static int MonitorCommand(IntPtr domain, [MarshalAs(UnmanagedType.LPStr)]string cmd, ref string result, VirDomainQemuMonitorCommandFlags flags)
        {
            var sb = new StringBuilder();
            //IntPtr buf = Marshal.AllocCoTaskMem(MaxStringLength + 1);
            //Marshal.WriteByte(buf, MaxStringLength, 0);
            //IntPtr buf2 = buf;
            int ret = MonitorCommandImpl(domain, cmd, sb, (uint)flags);
            if (ret == 0)
                result = sb.ToString();
                //result = MarshalHelper.ptrToString(buf);
                //Marshal.FreeCoTaskMem(buf);
            return ret;
        }
    }
}
