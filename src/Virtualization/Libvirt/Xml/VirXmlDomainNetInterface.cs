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
using System.Xml.Serialization;

namespace IDNT.AppBasics.Virtualization.Libvirt.Xml
{
    [Serializable]
    [XmlRoot(ElementName = "interface", Namespace = "")]
    public class VirXmlDomainNetInterface
    {
        [XmlAttribute(AttributeName = "type")]
        public VirXmlDomainInterfaceType Type { get; set; }

        [XmlElement("mac")]
        public VirXmlDomainInterfaceMAC MAC { get; set; }

        [XmlElement("source")]
        public VirXmlDomainInterfaceSource Source { get; set; }

        [XmlElement("target")]
        public VirXmlDomainInterfaceTarget Target { get; set; }

        [XmlElement("model")]
        public VirXmlDomainInterfaceModel Model { get; set; }

        [XmlElement("address")]
        public VirXmlDeviceAddress Address { get; set; }

        [XmlElement("alias")]
        VirXmlDomainInterfaceAlias Alias { get; set; }

        public override string ToString()
        {
            return $"{Type} {Source} {MAC} ({Address})";
        }
    }
}
