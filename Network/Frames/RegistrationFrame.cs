using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Network.Frames
{
    public class RegistrationFrame
    {
        [BinarySerializerLibrary.Attributes.BinaryTypeString()]
        public string? Name { get; set; }
    }
}
