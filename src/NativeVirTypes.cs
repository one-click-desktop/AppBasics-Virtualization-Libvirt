/*
 * Copyright (C)
 *   Arnaud Champion <arnaud.champion@devatom.fr>
 *   Jaromír Červenka <cervajz@cervajz.com>
 *
 * See COPYING.LIB for the License of this software
 */

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Libvirt
{
    #region delegates
    ///<summary>
    /// Signature of a function to use when there is an error raised by the library.
    ///</summary>
    ///<param name="userData">user provided data for the error callback</param>
    ///<param name="error">the error being raised.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ErrorFunc(IntPtr userData, VirError error);
    ///<summary>
    /// Callback for receiving file handle events. The callback will be invoked once for each event which is pending.
    ///</summary>
    ///<param name="watch">watch on which the event occurred</param>
    ///<param name="fd">file handle on which the event occurred</param>
    ///<param name="events">bitset of events from virEventHandleType constants</param>
    ///<param name="opaque">user data registered with handle</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EventHandleCallback(int watch, int fd, int events, IntPtr opaque);
  
    ///<summary>
    /// Part of the EventImpl, this callback Adds a file handle callback to listen for specific events. The same file handle can be registered multiple times provided the requested event sets are non-overlapping If the opaque user data requires free'ing when the handle is unregistered, then a 2nd callback can be supplied for this purpose.
    ///</summary>
    ///<param name="fd">file descriptor to listen on</param>
    ///<param name="events">bitset of events on which to fire the callback</param>
    ///<param name="cb">the callback to be called when an event occurrs</param>
    ///<param name="opaque">user data to pass to the callback</param>
    ///<param name="ff">the callback invoked to free opaque data blob</param>
    ///<returns>a handle watch number to be used for updating and unregistering for events</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int EventAddHandleFunc(int fd, int events, [MarshalAs(UnmanagedType.FunctionPtr)] EventHandleCallback cb, IntPtr opaque, [MarshalAs(UnmanagedType.FunctionPtr)] VirFreeCallback ff);
    ///<summary>
    /// Part of the EventImpl, this user-provided callback is notified when events to listen on change
    ///</summary>
    ///<param name="watch">file descriptor watch to modify</param>
    ///<param name="events">new events to listen on</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EventUpdateHandleFunc(int watch, int events);
    ///<summary>
    /// Part of the EventImpl, this user-provided callback is notified when an fd is no longer being listened on. If a virEventHandleFreeFunc was supplied when the handle was registered, it will be invoked some time during, or after this function call, when it is safe to release the user data.
    ///</summary>
    ///<param name="watch">file descriptor watch to stop listening on</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int EventRemoveHandleFunc(int watch);
    ///<summary>
    /// callback for receiving timer events
    ///</summary>
    ///<param name="timer">timer id emitting the event</param>
    ///<param name="opaque">user data registered with handle</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EventTimeoutCallback(int timer, IntPtr opaque);
    ///<summary>
    /// Part of the EventImpl, this user-defined callback handles adding an event timeout. If the opaque user data requires free'ing when the handle is unregistered, then a 2nd callback can be supplied for this purpose.
    ///</summary>
    ///<param name="timeout">The timeout to monitor</param>
    ///<param name="cb">the callback to call when timeout has expired</param>
    ///<param name="opaque">user data to pass to the callback</param>
    ///<param name="ff">the callback invoked to free opaque data blob</param>
    /// <returns>A timer value</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int EventAddTimeoutFunc(int timeout, [MarshalAs(UnmanagedType.FunctionPtr)] EventTimeoutCallback cb, IntPtr opaque, [MarshalAs(UnmanagedType.FunctionPtr)] VirFreeCallback ff);
    ///<summary>
    /// Part of the EventImpl, this user-defined callback updates an event timeout.
    ///</summary>
    ///<param name="timer">the timer to modify</param>
    ///<param name="timeout">the new timeout value</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EventUpdateTimeoutFunc(int timer, int timeout);
    ///<summary>
    /// Part of the EventImpl, this user-defined callback removes a timer If a virEventTimeoutFreeFunc was supplied when the handle was registered, it will be invoked some time during, or after this function call, when it is safe to release the user data.
    ///</summary>
    ///<param name="timer">the timer to remove</param>
    /// <returns>0 on success, -1 on failure</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int EventRemoveTimeoutFunc(int timer);
    ///<summary>
    /// Authentication callback
    ///</summary>
    ///<param name="creds">virConnectCredential array</param>
    ///<param name="ncred">number of virConnectCredential in cred</param>
    ///<param name="cbdata">user data passed to callback</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int ConnectAuthCallbackUnmanaged(IntPtr creds, uint ncred, IntPtr cbdata);
    /// <summary>
    /// Authentication callback
    /// </summary>
    /// <param name="creds">ConnectCredential array</param>
    /// <param name="cbdata">user data passed to callback</param>
    /// <returns></returns>
    public delegate int ConnectAuthCallback(ref VirConnectCredential[] creds, IntPtr cbdata);
	/// <summary>
	/// Callback for receiving stream events. The callback will be invoked once for each event which is pending.
	/// </summary>
	/// <param name="stream">stream on which the event occurred</param>
	/// <param name="events">bitset of events from virEventHandleType constants</param>
	/// <param name="opaque">user data registered with handle</param>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void StreamEventCallback(IntPtr stream, int events, IntPtr opaque);
	/// <summary>
	/// The virStreamSinkFunc callback is used together with the virStreamRecvAll function for libvirt
	/// to provide the data that has been received. The callback will be invoked multiple times, providing
	/// data in small chunks. The application should consume up 'nbytes' from the 'data' array of data and
	/// then return the number actual number of bytes consumed. The callback will continue to be invoked until
	/// it indicates the end of the stream has been reached. A return value of -1 at any time will abort the
	/// receive operation
	/// </summary>
	/// <param name="st">the stream object</param>
	/// <param name="data">preallocated array to be filled with data</param>
	/// <param name="nbytes">size of the data array</param>
	/// <param name="opaque">optional application provided data</param>
	/// <returns>the number of bytes filled, 0 upon end of file, or -1 upon error</returns>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int StreamSinkFunc(IntPtr st, IntPtr data, int nbytes, IntPtr opaque);
    #endregion
}
