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

namespace Libvirt
{
    ///<summary>
    /// Structure to handle connection authentication
    ///</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VirConnectAuth
    {
        /// <summary>
        /// List of supported virConnectCredentialType values, should be a IntPtr to an int array or to a virConnectCredentialType array
        /// </summary>
        private IntPtr credtypes;
        ///<summary>
        /// Number of virConnectCredentialType in credtypes
        ///</summary>
        [MarshalAs(UnmanagedType.U4)]
        private uint ncredtype;
        ///<summary>
        /// Callback used to collect credentials, a virConnectAuthCallback delegate in bindings
        ///</summary>
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public ConnectAuthCallback cb;
        ///<summary>
        /// Data transported with callback, should be a IntPtr on what you want
        ///</summary>
        public IntPtr cbdata;
        /// <summary>
        /// List of supported virConnectCredentialType values
        /// </summary>
        public VirConnectCredentialType[] CredTypes
        {
            get
            {
                int[] intCredTypes = new int[ncredtype];
                Marshal.Copy(credtypes, intCredTypes, 0, (int)ncredtype);
                VirConnectCredentialType[] result = new VirConnectCredentialType[ncredtype];
                for (int i = 0; i < intCredTypes.Length; i++)
                {
                    result[i] = (VirConnectCredentialType)intCredTypes[i];
                }
                return result;
            }
            set
            {
                ncredtype = (uint)value.Length;
                credtypes = Marshal.AllocHGlobal(value.Length * sizeof(int));
                int[] vals = new int[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    vals[i] = (int)value[i];
                }
                Marshal.Copy(vals, 0, credtypes, (int)ncredtype);
            }
        }
    }
}
