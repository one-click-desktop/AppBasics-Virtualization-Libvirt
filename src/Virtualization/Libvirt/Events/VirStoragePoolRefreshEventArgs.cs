using System;
using System.Collections.Generic;
using System.Text;

namespace IDNT.AppBasics.Virtualization.Libvirt.Events
{
    public class VirStoragePoolRefreshEventArgs : EventArgs
    {
        public Guid UniqueId { get; internal set; }
    }
}
