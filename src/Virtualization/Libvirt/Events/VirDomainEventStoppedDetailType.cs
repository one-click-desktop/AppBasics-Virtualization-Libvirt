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

namespace IDNT.AppBasics.Virtualization.Libvirt.Events
{
    /// <summary>
    /// Details on the caused of the 'stopped' lifecycle event
    /// </summary>
    public enum VirDomainEventStoppedDetailType
    {
        /// <summary>
        /// Normal shutdown
        /// </summary>
        VIR_DOMAIN_EVENT_STOPPED_SHUTDOWN = 0,
        /// <summary>
        /// Forced poweroff from host
        /// </summary>
        VIR_DOMAIN_EVENT_STOPPED_DESTROYED = 1,
        /// <summary>
        /// Guest crashed
        /// </summary>
        VIR_DOMAIN_EVENT_STOPPED_CRASHED = 2,
        /// <summary>
        /// Migrated off to another host
        /// </summary>
        VIR_DOMAIN_EVENT_STOPPED_MIGRATED = 3,
        /// <summary>
        /// Saved to a state file
        /// </summary>
        VIR_DOMAIN_EVENT_STOPPED_SAVED = 4,
        /// <summary>
        /// Host emulator/mgmt failed
        /// </summary>
        VIR_DOMAIN_EVENT_STOPPED_FAILED = 5
    }

}
