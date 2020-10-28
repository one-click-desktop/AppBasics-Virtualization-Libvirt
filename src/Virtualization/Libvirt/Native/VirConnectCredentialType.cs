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
using System.Text;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    ///<summary>
    /// Types of credentials
    ///</summary>
    public enum VirConnectCredentialType
    {
        ///<summary>
        /// Identity to act as
        ///</summary>
        VIR_CRED_USERNAME = 1,
        ///<summary>
        /// Identify to authorize as
        ///</summary>
        VIR_CRED_AUTHNAME = 2,
        ///<summary>
        /// RFC 1766 languages, comma separated
        ///</summary>
        VIR_CRED_LANGUAGE = 3,
        ///<summary>
        /// client supplies a nonce
        ///</summary>
        VIR_CRED_CNONCE = 4,
        /// <summary>
        /// Passphrase secret
        /// </summary>
        VIR_CRED_PASSPHRASE = 5,
        /// <summary>
        /// Challenge response
        /// </summary>
        VIR_CRED_ECHOPROMPT = 6,
        /// <summary>
        /// Challenge response
        /// </summary>
        VIR_CRED_NOECHOPROMPT = 7,
        /// <summary>
        /// Authentication realm
        /// </summary>
        VIR_CRED_REALM = 8,
        /// <summary>
        /// Externally managed credential More may be added - expect the unexpected
        /// </summary>
        VIR_CRED_EXTERNAL = 9
    }
}
