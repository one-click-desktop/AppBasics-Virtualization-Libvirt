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

namespace Libvirt
{
    [Serializable]
    [XmlRoot(ElementName = "address", Namespace = "")]
    public class VirXmlDeviceAddress
    {
        [XmlAttribute(AttributeName = "type")]
        public VirXmlAddressType Type { get; set; }

        [XmlAttribute(AttributeName = "domain")]
        public string Domain { get; set; }

        [XmlAttribute(AttributeName = "bus")]
        public string Bus { get; set; }

        [XmlAttribute(AttributeName = "slot")]
        public string Slot { get; set; }

        [XmlAttribute(AttributeName = "function")]
        public string Function { get; set; }

        [XmlAttribute(AttributeName = "controller")]
        public string Controller { get; set; }

        [XmlAttribute(AttributeName = "target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "unit")]
        public string Unit { get; set; }

        public override string ToString()
        {
            // Mimic lspci syntax
            switch (Type)
            {
                case VirXmlAddressType.DRIVE:
                    return $"{Type.ToString()}/{Controller}:{Bus}:{Target}:{Unit}";
                case VirXmlAddressType.PCI:
                    return $"{Type.ToString()}/{Bus.Substring(2)}:{Slot.Substring(2)}.{Function.Substring(2)}";
                default:
                    return "Unknown address type.";
            }
        }
    }
}
