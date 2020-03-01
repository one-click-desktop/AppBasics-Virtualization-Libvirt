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

namespace Libvirt
{
    ///<summary>
    /// Types of storage volume
    ///</summary>
    public enum VirStorageVolType
    {
        /// <summary>
        /// Regular file based volumes.
        /// </summary>
        VIR_STORAGE_VOL_FILE = 0,
        /// <summary>
        /// Block based volumes.
        /// </summary>
        VIR_STORAGE_VOL_BLOCK = 1,
        /// <summary>
        /// Directory-passthrough based volume
        /// </summary>
        VIR_STORAGE_VOL_DIR = 2,
        /// <summary>
        /// Network volumes like RBD (RADOS Block Device)
        /// </summary>
        VIR_STORAGE_VOL_NETWORK = 3,
        /// <summary>
        /// Network accessible directory that can contain other network volumes
        /// </summary>
        VIR_STORAGE_VOL_NETDIR = 4,
        /// <summary>
        /// Ploop based volumes
        /// </summary>
        VIR_STORAGE_VOL_PLOOP = 5
    }
}
