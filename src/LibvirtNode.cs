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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Libvirt
{
    public class LibvirtNode : IDisposable
    {
        private readonly LibvirtConnection _connection;

        internal LibvirtNode(LibvirtConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException("connection");
        }

        public string Hostname
        {
            get { return NativeVirConnect.GetHostname(_connection.ConnectionPtr); }
        }

        public ulong MemFreeBytes
        {
            get { return NativeVirNode.GetFreeMemory(_connection.ConnectionPtr); }
        }

        private VirNodeInfo _virNodeInfo = null;

        public VirNodeInfo GetInfo()
        {
            if (_virNodeInfo == null && NativeVirNode.GetInfo(_connection.ConnectionPtr, (_virNodeInfo = new VirNodeInfo())) < 0)
            {
                _virNodeInfo = null;
                throw new LibvirtQueryException();
            }
            return _virNodeInfo;
        }

        public string CpuModelName { get { return GetInfo().Model; } }

        public uint CpuFrequencyMhz { get { return GetInfo().Mhz; } }

        public uint CpuSocketCount { get { return GetInfo().Sockets; } }

        public uint CpuCores { get { return GetInfo().Cores; } }

        public uint CpuThreads { get { return GetInfo().Threads; } }

        public ulong MemoryBytes { get { return GetInfo().Memory; } }

        #region IDisposable implementation
        private Int32 _isDisposing = 0;

        /// <summary>
        /// Disposes the connection.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _isDisposing, 1, 0) == 1)
                return;
        }
        #endregion
    }
}
