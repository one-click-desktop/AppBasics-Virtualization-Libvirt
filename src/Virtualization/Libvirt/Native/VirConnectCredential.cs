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
using IDNT.AppBasics.Virtualization.Libvirt.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    ///<summary>
    /// Credential structure
    ///</summary>
    [StructLayout(LayoutKind.Sequential)]
    public class VirConnectCredential
    {
        public VirConnectCredential()
        {
            result = IntPtr.Zero;
        }
        ///<summary>
        /// One of virConnectCredentialType constants
        ///</summary>
        [MarshalAs(UnmanagedType.I4)]
        public VirConnectCredentialType type;
        ///<summary>
        /// Prompt to show to user
        ///</summary>
        private IntPtr prompt;
        ///<summary>
        /// Additional challenge to show
        ///</summary>
        private IntPtr challenge;
        ///<summary>
        /// Optional default result
        ///</summary>
        private IntPtr defresult;
        ///<summary>
        /// Result to be filled with user response (or defresult). An IntPtr to a marshalled allocated string
        ///</summary>
        private IntPtr result;
        ///<summary>
        /// Length of the result
        ///</summary>
        [MarshalAs(UnmanagedType.U4)]
        private uint resultlen;
        ///<summary>
        /// Prompt to show to user
        ///</summary>
        public string Prompt
        {
            get
            {
                return Marshal.PtrToStringAnsi(prompt);
            }
        }
        ///<summary>
        /// Additional challenge to show
        ///</summary>
        public string Challenge
        {
            get
            {
                return Marshal.PtrToStringAnsi(challenge);
            }
        }
        ///<summary>
        /// Optional default result
        ///</summary>
        public string Defresult
        {
            get
            {
                return Marshal.PtrToStringAnsi(defresult);
            }
        }
        ///<summary>
        /// Result to be filled with user response (or defresult).
        ///</summary>
        public string Result
        {
            get
            {
                return Marshal.PtrToStringAnsi(result);
            }
            set
            {
                IntPtr tmp = Marshal.StringToHGlobalAnsi(value);

                NativeFunctions.Free(result);
                result = NativeFunctions.StrDup(tmp);
                resultlen = (uint)value.Length;

                Marshal.FreeHGlobal(tmp);
            }
        }
    }
}
