﻿using System;
using System.Runtime.InteropServices;

namespace LibvirtBindings
{
    class StringWithoutNativeCleanUpMarshaler : ICustomMarshaler
    {
        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new StringWithoutNativeCleanUpMarshaler();
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return Marshal.PtrToStringAnsi(pNativeData);
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            return Marshal.StringToHGlobalAnsi((string) ManagedObj);
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
        }

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public int GetNativeDataSize()
        {
            throw new NotImplementedException();
        }
    }
}