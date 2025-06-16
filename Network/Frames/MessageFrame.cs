using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Network.Frames
{
    public class MessageFrame
    {
        [BinarySerializerLibrary.Attributes.BinaryTypeString()]
        public string? ClientFrom { get; set; }
        [BinarySerializerLibrary.Attributes.BinaryTypeString()]
        public string? ClientTo { get; set; }
        [BinarySerializerLibrary.Attributes.BinaryTypeString()]
        public string? MessageContent { get; set; }
    }
}
