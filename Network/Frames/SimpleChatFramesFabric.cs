using BinarySerializerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Network.Frames
{
    public static class SimpleChatFramesFabric
    {
        public static object? ParseFrame(byte[]? frameContent)
        {
            var baseFrame = BinarySerializer.Deserialize<SimpleChatBaseFrame>(frameContent);

            if (baseFrame is not null)
            {
                if (baseFrame.Message is not null)
                {
                    return baseFrame.Message;
                }
                else if (baseFrame.ClientListRequest is not null)
                {
                    return baseFrame.ClientListRequest;
                }
                else if (baseFrame.ClientsList is not null)
                {
                    return baseFrame.ClientsList;
                }
                else if (baseFrame.Registration is not null)
                {
                    return baseFrame.Registration;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static SimpleChatBaseFrame CreateClientListRequestFrame()
        {
            return new()
            {
                ClientListRequest = new()
            };
        }

        public static SimpleChatBaseFrame CreateClientsListFrame(IEnumerable<string> clients)
        {
            return new()
            {
                ClientsList = new()
                {
                    Clients = clients.ToArray()
                }
            };
        }

        public static SimpleChatBaseFrame CreateMessageFrame(string from, string to, string content)
        {
            return new()
            {
                Message = new()
                {
                    ClientFrom = from,
                    ClientTo = to,
                    MessageContent = content
                }
            };
        }

        public static SimpleChatBaseFrame CreateRegistrationFrame(string clientName)
        {
            return new()
            {
                Registration = new()
                { 
                    Name = clientName
                }
            };
        }
    }
}
