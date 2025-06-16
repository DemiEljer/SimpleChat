using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Network.Frames
{
    public class ClientsListFrame
    {
        [BinarySerializerLibrary.Attributes.BinaryTypeString(BinarySerializerLibrary.Enums.BinaryArgumentTypeEnum.Array)]
        public string[]? Clients { get; set; }
    }
}
