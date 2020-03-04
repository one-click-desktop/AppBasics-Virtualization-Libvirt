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
using System.Text;

namespace Libvirt.Metrics
{
    public class GuestCpuUtilizationMetric
    {
        private readonly int _cpuCores = 0;
        private decimal _lastValue = 0;
        private List<Int16> _values = null;
        private readonly object _lock = new object();
        private DateTime _tsLastValue;

        public GuestCpuUtilizationMetric(int cpuCores, int historySize = 300)
        {
            if (cpuCores < 1)
                throw new ArgumentException("A minimum of one cpu core must be specified.", "cpuCores");
            _cpuCores = cpuCores;
            _tsLastValue = DateTime.UtcNow;
            _values = new List<Int16>(historySize < 2 ? 2 : historySize) { 0 };
        }

        /// <summary>
        /// Returns the guests cpu utilization in percent
        /// </summary>
        public decimal Current
        {
            get { lock (_lock) return _values[0]; }
        }

        /// <summary>
        /// Returns a series of cpu utilizations (last historySize elements) with the most recent values first.
        /// </summary>
        public IEnumerable<Int16> Values
        {
            get { lock (_lock) return _values.ToArray(); }
        }

        internal void Update(ulong cpuTime, ulong systemTime, ulong userTime)
        {
            var utcNow = DateTime.UtcNow;

            decimal time = (decimal)(cpuTime - systemTime - userTime) / (decimal)1000000000;
            
            var pct = (decimal)100 * (decimal)((decimal)(time - _lastValue) / (decimal)utcNow.Subtract(_tsLastValue).TotalSeconds) / (decimal)_cpuCores;
            if (pct > 100 || pct < 0)
                pct = 0;

            lock (_lock)
            {
                _values.Insert(0, Convert.ToInt16(pct));
                if (_values.Count == _values.Capacity)
                    _values.RemoveAt(_values.Capacity - 1);
            }

            _lastValue = time;
            _tsLastValue = utcNow;
        }
    }
}
