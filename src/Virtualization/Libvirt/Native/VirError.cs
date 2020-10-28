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

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    /// <summary>
    /// the virError object
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class VirError
    {
        /// <summary>
        /// The error code, a virErrorNumber.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public VirErrorNumber code;
        /// <summary>
        /// What part of the library raised this error.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int domain;
        /// <summary>
        /// Human-readable informative error message.
        /// </summary>
        private IntPtr message;
        /// <summary>
        /// Human-readable informative error message.
        /// </summary>
        public string Message
        {
            get { return Marshal.PtrToStringAnsi(message); }
        }

        /// <summary>
        /// How consequent is the error.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public VirErrorLevel level;
        /// <summary>
        /// Connection if available, deprecated see note above.
        /// </summary>
        public IntPtr conn;
        /// <summary>
        /// Domain if available, deprecated see note above.
        /// </summary>
        public IntPtr dom;
        /// <summary>
        /// Extra string information.
        /// </summary>
        private IntPtr str1;
        /// <summary>
        /// Extra string information.
        /// </summary>
        public string Str1 { get { return Marshal.PtrToStringAnsi(str1); } }
        /// <summary>
        /// Extra string information.
        /// </summary>
        [MarshalAs(UnmanagedType.SysInt)]
        private IntPtr str2;
        /// <summary>
        /// Extra string information.
        /// </summary>
        public string Str2 { get { return Marshal.PtrToStringAnsi(str2); } }
        /// <summary>
        /// Extra string information.
        /// </summary>
        private IntPtr str3;
        /// <summary>
        /// Extra string information.
        /// </summary>
        public string Str3 { get { return Marshal.PtrToStringAnsi(str3); } }
        /// <summary>
        /// Extra number information.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int int1;
        /// <summary>
        /// Extra number information.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int int2;
        /// <summary>
        /// Network if available, deprecated see note above.
        /// </summary>
        public IntPtr net;

        /// <summary>
        /// Returns a string representation of the error
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string extra = !string.IsNullOrEmpty(Str1) ? " " + Str1 : String.Empty;
            extra += !string.IsNullOrEmpty(Str2) ? (string.IsNullOrEmpty(extra) ? "" : " ") + Str2 : String.Empty;
            extra += !string.IsNullOrEmpty(Str3) ? (string.IsNullOrEmpty(extra) ? "" : " ") + Str3 : String.Empty;
            return string.Format($"{level.ToString()} #{code.ToString()} {Message}{extra}");
        }
    }
}
