/*
 * Libvirt-dotnet
 * 
 * Copyright 2020 IDNT (https://www.idnt.net) and Libvirt-dotnet contributors..
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
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace Libvirt
{
    /// <summary>
    /// Libvirt exception
    /// </summary>
    public class LibvirtException : Exception
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public LibvirtException()
            : base(NativeVirErrors.GetLastMessage())
        {
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="message">Exception message</param>
        public LibvirtException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public LibvirtException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
