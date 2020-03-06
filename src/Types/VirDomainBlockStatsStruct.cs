using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Libvirt
{
    ///<summary>
    /// Blocks domain statistics
    ///</summary>
    [StructLayout(LayoutKind.Sequential)]
    public class VirDomainBlockStatsStruct
    {
        /// <summary>
        /// Number of read requests.
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long rd_req;
        /// <summary>
        /// Number of read bytes.
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long rd_bytes;
        /// <summary>
        /// Number of write requests.
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long wr_req;
        /// <summary>
        /// Number of written bytes.
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long wr_bytes;
        /// <summary>
        /// In Xen this returns the mysterious 'oo_req'.
        /// </summary>
        [MarshalAs(UnmanagedType.I8)]
        public long errs;
    }
}
