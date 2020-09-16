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
using System.Runtime.InteropServices;
using System.Text;

namespace Libvirt
{
    public class OpenAuthPasswordAuth : LibvirtAuthentication
    {
        private struct AuthData
        {
            public string user;
            public string password;
        }

        private AuthData authData = new AuthData();

        public string Username { get { return authData.user; } set { authData.user = value; } }

        public string Password { get { return authData.password; } set { authData.password = value; } }

        public VirConnectFlags Flags;

        internal override IntPtr Connect(string uri, LibvirtConfiguration configuration)
        {
            // Fill a structure to pass username and password to callbacks
            IntPtr authDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf(authData));
            try
            {
                Marshal.StructureToPtr(authData, authDataPtr, false);

                // Fill a virConnectAuth structure
                VirConnectAuth auth = new VirConnectAuth
                {
                    cbdata = authDataPtr,               // The authData structure
                    cb = AuthCallback,                  // the method called by callbacks
                    CredTypes = new[]
                                    {
                                    VirConnectCredentialType.VIR_CRED_AUTHNAME,
                                    VirConnectCredentialType.VIR_CRED_PASSPHRASE
                                }          // The list of credentials types
                };

                return NativeVirConnect.OpenAuth(uri, ref auth, (int)Flags);
            }
            finally
            {
                Marshal.DestroyStructure(authDataPtr, typeof(AuthData));
                Marshal.FreeHGlobal(authDataPtr);
            }
        }

        private static int AuthCallback(ref VirConnectCredential[] creds, IntPtr cbdata)
        {
            AuthData authData = (AuthData)Marshal.PtrToStructure(cbdata, typeof(AuthData));
            for (int i = 0; i < creds.Length; i++)
            {
                VirConnectCredential cred = creds[i];
                switch (cred.type)
                {
                    case VirConnectCredentialType.VIR_CRED_AUTHNAME:
                        // Fill the user name
                        cred.Result = authData.user;
                        break;
                    case VirConnectCredentialType.VIR_CRED_PASSPHRASE:
                        // Fill the password
                        cred.Result = authData.password;
                        break;
                    default:
                        return -1;
                }
            }
            return 0;
        }
    }
}
