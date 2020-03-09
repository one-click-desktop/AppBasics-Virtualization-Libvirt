/*
 * Copyright (C)
 *   Arnaud Champion <arnaud.champion@devatom.fr>
 *   Jaromír Červenka <cervajz@cervajz.com>
 *
 * See COPYING.LIB for the License of this software
 */

using System.Runtime.InteropServices;

namespace Libvirt
{
    /// <summary>
    /// The Event class expose all the event related methods
    /// </summary>
    public class NativeVirEvent
    {
        static NativeVirEvent()
        {
            NativeVirInitialize.Initialize();
        }

        ///<summary>
        /// Function to install callbacks
        ///</summary>
        ///<param name="addHandle">the virEventAddHandleFunc which will be called (a delegate)</param>
        ///<param name="updateHandle">the virEventUpdateHandleFunc which will be called (a delegate)</param>
        ///<param name="removeHandle">the virEventRemoveHandleFunc which will be called (a delegate)</param>
        ///<param name="addTimeout">the virEventAddTimeoutFunc which will be called (a delegate)</param>
        ///<param name="updateTimeout">the virEventUpdateTimeoutFunc which will be called (a delegate)</param>
        ///<param name="removeTimeout">the virEventRemoveTimeoutFunc which will be called (a delegate)</param>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virEventRegisterImpl")]
        public static extern void RegisterImpl([MarshalAs(UnmanagedType.FunctionPtr)] EventAddHandleFunc addHandle,
                                               [MarshalAs(UnmanagedType.FunctionPtr)] EventUpdateHandleFunc updateHandle,
                                               [MarshalAs(UnmanagedType.FunctionPtr)] EventRemoveHandleFunc removeHandle,
                                               [MarshalAs(UnmanagedType.FunctionPtr)] EventAddTimeoutFunc addTimeout,
                                               [MarshalAs(UnmanagedType.FunctionPtr)] EventUpdateTimeoutFunc updateTimeout,
                                               [MarshalAs(UnmanagedType.FunctionPtr)] EventRemoveTimeoutFunc removeTimeout);

        /// <summary>
        /// Registers a default event implementation based on the poll() system call. This is a generic implementation that can be used by any client application which does not have a need to integrate with an external event loop impl.
        /// For proper event handling, it is important that the event implementation is registered before a connection to the Hypervisor is opened.
        /// </summary>
        /// <returns>0 on success, -1 on failure.</returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virEventRegisterDefaultImpl")]
        public static extern int RegisterDefaultImpl();

        /// <summary>
        /// Run one iteration of the event loop. Applications will generally want to have a thread which invokes this method in an infinite loop. Furthermore, it is wise to set up a pipe-to-self handler (via virEventAddHandle()) or a timeout (via virEventAddTimeout()) before calling this function, as it will block forever if there are no registered events.
        /// </summary>
        /// <returns></returns>
        [DllImport("libvirt-0.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "virEventRunDefaultImpl")]
        public static extern int RunDefaultImpl();
    }
}
