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
using IDNT.AppBasics.Virtualization.Libvirt.Metrics;
using IDNT.AppBasics.Virtualization.Libvirt.Native;
using IDNT.AppBasics.Virtualization.Libvirt.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace IDNT.AppBasics.Virtualization.Libvirt
{
    /// <summary>
    /// Represents a libvirt storage volume
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "disk", Namespace = "")]
    [XmlInclude(typeof(VirXmlDomainDisk))]
    public class LibvirtDiskDevice : VirXmlDomainDisk, IDisposable
    {
        private LibvirtDomain _dom;

        private LibvirtDiskDevice()
        {
        }

        internal LibvirtDiskDevice Init(LibvirtDomain dom)
        {
            _dom = dom;
            return this;
        }

        public LibvirtStorageVolume Volume => _dom.Connection.GetVolumeByDiskSource(this.Source);

        public VirDomainBlockStatsStruct GetBlockStats()
        {
            VirDomainBlockStatsStruct stats;
            if (NativeVirDomain.BlockStats(_dom.DomainPtr, this.Target.Device, out stats) < 0)
                return null;

            return stats;
        }

        #region Object overrides
        public override int GetHashCode()
        {
            return this.Target.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is LibvirtDiskDevice && ((LibvirtDiskDevice)obj).Target.ToString().Equals(this.Target.ToString());
        }
        #endregion

        #region IDisposable implementation
        private Int32 _isDisposing = 0;

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _isDisposing, 1, 0) == 1)
                return;

            Trace.WriteLine($"Disposing disk device {this.ToString()}.");
        }
        #endregion
    }
}
