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

namespace IDNT.AppBasics.Virtualization.Libvirt
{
    /// <summary>
    /// States of a domain
    /// </summary>
    public enum VirDomainState
    {
        /// <summary>
        /// No state.
        /// </summary>
        VIR_DOMAIN_NOSTATE = 0,
        /// <summary>
        /// The domain is running.
        /// </summary>
        VIR_DOMAIN_RUNNING = 1,
        /// <summary>
        /// The domain is blocked on resource.
        /// </summary>
        VIR_DOMAIN_BLOCKED = 2,
        /// <summary>
        /// The domain is paused by user.
        /// </summary>
        VIR_DOMAIN_PAUSED = 3,
        /// <summary>
        /// The domain is being shut down.
        /// </summary>
        VIR_DOMAIN_SHUTDOWN = 4,
        /// <summary>
        /// The domain is shut off.
        /// </summary>
        VIR_DOMAIN_SHUTOFF = 5,
        /// <summary>
        /// The domain is crashed.
        /// </summary>
        VIR_DOMAIN_CRASHED = 6
    }
}
