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
using System.Runtime.InteropServices;

namespace IDNT.AppBasics.Virtualization.Libvirt.Native
{
    ///<summary>
    /// class for libvirt errors binding
    ///</summary>
    public class NativeVirErrors
    {
        /// <summary>
        /// The error object is kept in thread local storage, so separate threads can safely access this concurrently.
        /// Reset the last error caught on that connection.
        /// </summary>
        /// <param name="conn">
        /// A <see cref="IntPtr"/> pointer to the hypervisor connection.
        /// </param>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virConnResetLastError")]
        public static extern void ConnResetLastError(IntPtr conn);

        /// <summary>
        /// Set a connection error handling function, if @handler is NULL it will reset to default
        /// which is to pass error back to the global library handler.
        /// </summary>
        /// <param name="conn">
        /// A <see cref="IntPtr"/>pointer to the hypervisor connection.
        /// </param>
        /// <param name="userData">
        /// A <see cref="IntPtr"/>pointer to the user data provided in the handler callback.
        /// </param>
        /// <param name="handler">
        /// A <see cref="ErrorFunc"/>function to get called in case of error or NULL
        /// </param>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virConnSetErrorFunc")]
        public static extern void ConnSetErrorFunc(IntPtr conn, IntPtr userData, [MarshalAs(UnmanagedType.FunctionPtr)] ErrorFunc handler);

        /// <summary>
        /// Copy the content of the last error caught at the library level.
        /// The error object is kept in thread local storage, so separate threads can safely access this concurrently.
        /// One will need to free the result with virResetError().
        /// </summary>
        /// <param name="to">
        /// A <see cref="VirError"/> target to receive the copy.
        /// </param>
        /// <returns>
        /// 0 if no error was found and the error code otherwise and -1 in case of parameter error.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virCopyLastError")]
        public static extern int CopyLastError([Out] VirError to);

        /// <summary>
        /// Default routine reporting an error to stderr.
        /// </summary>
        /// <param name="err">
        /// A <see cref="VirError"/> pointer to the error.
        /// </param>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virDefaultErrorFunc")]
        public static extern void DefaultErrorFunc([In] VirError err);

        /// <summary>
        /// Resets and frees the given error.
        /// </summary>
        /// <param name="err">
        /// A <see cref="VirError"/> error to free.
        /// </param>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virFreeError")]
        public static extern void FreeError(VirError err); // Does not work, anybody know why?

        /// <summary>
        /// Provide a pointer to the last error caught at the library level.
        /// The error object is kept in thread local storage, so separate threads can safely access this concurrently.
        /// </summary>
        /// <returns>
        /// A pointer to the last error or NULL if none occurred.
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virGetLastError")]
        private static extern IntPtr GetLastErrorImpl();
        
        /// <summary>
        /// Provide the last error caught at the library level. 
        /// The error object is kept in thread local storage, so separate threads can safely access this concurrently.
        /// </summary>
        /// <returns>
        /// The last error or NULL if none occurred.
        /// </returns>
        public static VirError GetLastError()
        {
            IntPtr errPtr = GetLastErrorImpl();
            if (errPtr == IntPtr.Zero)
                return null;
            return (VirError)Marshal.PtrToStructure(errPtr, typeof (VirError));
        }

        /// <summary>
        /// Returns the message for the last error
        /// </summary>
        /// <returns>Error message</returns>
        public static string GetLastMessage()
        {
            var error = GetLastError();
            if (error == null)
                return "Unknown Error.";
            return error.ToString();
        }

        /// <summary>
        /// Reset the error being pointed to.
        /// </summary>
        /// <param name="err">
        /// A <see cref="VirError"/> pointer to the to clean up.
        /// </param>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virResetError")]
        public static extern void ResetError(VirError err);

        /// <summary>
        /// Reset the last error caught at the library level. The error object is kept in thread local storage,
        /// so separate threads can safely access this concurrently, only resetting their own error object.
        /// </summary>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virResetLastError")]
        public static extern void ResetLastError();

        /// <summary>
        /// Save the last error into a new error object.
        /// </summary>
        /// <returns>
        /// A <see cref="VirError"/> pointer to the copied error or NULL if allocation failed.
        /// It is the caller's responsibility to free the error with virFreeError().
        /// </returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virSaveLastError")]
        public static extern VirError SaveLastError();

        /// <summary>
        /// Set a library global error handling function, if @handler is NULL, it will reset to default printing on stderr.
        /// The error raised there are those for which no handler at the connection level could caught.
        /// </summary>
        /// <param name="userData">
        /// A <see cref="IntPtr"/>pointer to the user data provided in the handler callback.
        /// </param>
        /// <param name="handler">
        /// A <see cref="ErrorFunc"/>function to get called in case of error or NULL.
        /// </param>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virSetErrorFunc")]
        public static extern void SetErrorFunc(IntPtr userData, [MarshalAs(UnmanagedType.FunctionPtr)] ErrorFunc handler);
    }
}
