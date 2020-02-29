using System;
using System.Collections.Generic;
using System.Text;

namespace Libvirt
{
    public class VirStoragePoolRefreshEventArgs : EventArgs
    {
        public Guid UniqueId { get; internal set; }
    }
}
