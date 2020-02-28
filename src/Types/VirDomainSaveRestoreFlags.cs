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
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum VirDomainSaveRestoreFlags : uint
    {
        Empty = 0,

        /// <summary>
        /// (0x1; 1 << 0) Avoid file system cache pollution
        /// </summary>
        VIR_DOMAIN_SAVE_BYPASS_CACHE = 1,

        /// <summary>
        /// (0x2; 1 << 1) Favor running over paused
        /// </summary>
        VIR_DOMAIN_SAVE_RUNNING = 2,

        /// <summary>
        /// (0x4; 1 << 2) Favor paused over running
        /// </summary>
        VIR_DOMAIN_SAVE_PAUSED = 4
    }
}
