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
using System.Runtime.InteropServices;
using System.Text;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VirTypedParameter
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct _VALUEUNION
        {
            [FieldOffset(0)]
            public Int32 IntValue;

            [FieldOffset(0)]
            public UInt32 UIntValue;

            [FieldOffset(0)]
            public Int64 LongValue;

            [FieldOffset(0)]
            public UInt64 ULongValue;

            [FieldOffset(0)]
            public double DoubleValue;

            [FieldOffset(0)]
            public char CharValue;

            [FieldOffset(0)]
            public IntPtr StringValue;
        }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = VirConstants.VIR_TYPED_PARAM_FIELD_LENGTH)]
        public string Name;

        public VirTypedParamType Type;

        public _VALUEUNION Value;

    }
}
