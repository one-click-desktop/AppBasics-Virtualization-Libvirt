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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libvirt
{
    public enum VirDomainEventID : int
    {
        VIR_DOMAIN_EVENT_ID_LIFECYCLE = 0,

        VIR_DOMAIN_EVENT_ID_REBOOT = 1,

        VIR_DOMAIN_EVENT_ID_RTC_CHANGE = 2,

        VIR_DOMAIN_EVENT_ID_WATCHDOG = 3,

        VIR_DOMAIN_EVENT_ID_IO_ERROR = 4,

        VIR_DOMAIN_EVENT_ID_GRAPHICS = 5,

        VIR_DOMAIN_EVENT_ID_IO_ERROR_REASON = 6,

        VIR_DOMAIN_EVENT_ID_CONTROL_ERROR = 7,

        VIR_DOMAIN_EVENT_ID_BLOCK_JOB = 8,

        VIR_DOMAIN_EVENT_ID_DISK_CHANGE = 9,

        VIR_DOMAIN_EVENT_ID_TRAY_CHANGE = 10,

        VIR_DOMAIN_EVENT_ID_PMWAKEUP = 11,

        VIR_DOMAIN_EVENT_ID_PMSUSPEND = 12,

        VIR_DOMAIN_EVENT_ID_BALLOON_CHANGE = 13,

        VIR_DOMAIN_EVENT_ID_PMSUSPEND_DISK = 14,

        VIR_DOMAIN_EVENT_ID_DEVICE_REMOVED = 15,

        VIR_DOMAIN_EVENT_ID_BLOCK_JOB_2 = 16,

        VIR_DOMAIN_EVENT_ID_TUNABLE = 17,

        VIR_DOMAIN_EVENT_ID_AGENT_LIFECYCLE = 18,

        VIR_DOMAIN_EVENT_ID_DEVICE_ADDED = 19,

        VIR_DOMAIN_EVENT_ID_MIGRATION_ITERATION = 20,

        VIR_DOMAIN_EVENT_ID_JOB_COMPLETED = 21,

        VIR_DOMAIN_EVENT_ID_DEVICE_REMOVAL_FAILED = 22,

        VIR_DOMAIN_EVENT_ID_METADATA_CHANGE = 23,

        VIR_DOMAIN_EVENT_ID_BLOCK_THRESHOLD = 24,

        VIR_DOMAIN_EVENT_ID_LAST = 25,
    }
}
