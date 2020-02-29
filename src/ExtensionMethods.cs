using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Libvirt
{
    static public class ExtensionMethods
    {
        static public Guid ToGuid(this char[] uuid)
        {
            if (uuid == null || uuid.Length != 16)
                throw new ArgumentException("uuid");

            var rfc4122bytes = uuid.Select(t => Convert.ToByte(t)).ToArray();

            Array.Reverse(rfc4122bytes, 0, 4);
            Array.Reverse(rfc4122bytes, 4, 2);
            Array.Reverse(rfc4122bytes, 6, 2);

            return new Guid(rfc4122bytes);
        }

        static public char[] ToUUID(this Guid guid)
        {
            var rfc4122bytes = guid.ToByteArray();

            Array.Reverse(rfc4122bytes, 0, 4);
            Array.Reverse(rfc4122bytes, 4, 2);
            Array.Reverse(rfc4122bytes, 6, 2);

            return rfc4122bytes.Select(t => Convert.ToChar(t)).ToArray();
        }
    }

}
