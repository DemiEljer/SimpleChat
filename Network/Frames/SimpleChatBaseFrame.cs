using BinarySerializerLibrary;
using BinarySerializerLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Network.Frames
{
    public class SimpleChatBaseFrame
    {
        [BinaryTypeObject()]
        public ClientListRequestFrame? ClientListRequest { get; set; } = null;
        [BinaryTypeObject()]
        public ClientsListFrame? ClientsList { get; set; } = null;
        [BinaryTypeObject()]
        public MessageFrame? Message { get; set; } = null;
        [BinaryTypeObject()]
        public RegistrationFrame? Registration { get; set; } = null;

        /// <summary>
        /// Преобразование в вектор данных
        /// </summary>
        /// <param name="frame"></param>
        public static implicit operator byte[](SimpleChatBaseFrame frame)
        {
            return BinarySerializer.Serialize(frame);
        }
    }
}
