using CustomTCPServerLibrary;
using CustomTCPServerLibrary.Base;
using SimpleChat.Network.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Network
{
    public class SimpleChatClient
    {
        public CustomTCPClient Client { get; } = new();

        public string UserName { get; set; } = "User";

        public event Action? StartingErrorEvent;

        public event Action<string, string>? MessageIsReceivedEvent;

        public event Action<string, string>? MessageIsSentEvent;

        public event Action<string[]>? ClientsListReceiveEven;

        public event Action? ConnectionEvent;

        public event Action? DisconnectionEvent;

        public SimpleChatClient() 
        {
            Client.ReceiveDataEvent += (_, data) =>
            {
                var frameObject = SimpleChatFramesFabric.ParseFrame(data);

                if (frameObject is not null)
                {
                    if (frameObject is MessageFrame)
                    {
                        var _frame = (MessageFrame)frameObject;

                        if (_frame.ClientTo == UserName
                            && _frame.ClientFrom is not null
                            && _frame.MessageContent is not null)
                        {
                            MessageIsReceivedEvent?.Invoke(_frame.ClientFrom, _frame.MessageContent);
                        }
                    }
                    else if (frameObject is ClientsListFrame)
                    {
                        var _frame = (ClientsListFrame)frameObject;

                        if (_frame.Clients is not null)
                        {
                            ClientsListReceiveEven?.Invoke(_frame.Clients);
                        }
                    }
                }
            };
            Client.ProtocolHasBeenVerifiedEvent += (_) =>
            {
                Client.TransmitData(SimpleChatFramesFabric.CreateRegistrationFrame(UserName));

                ConnectionEvent?.Invoke();
            };
            Client.StartingExceptionThrowingEvent += (_, e) =>
            {
                StartingErrorEvent?.Invoke();
            };
            Client.HasStoppedEvent += (_) =>
            {
                DisconnectionEvent?.Invoke();
            };
        }

        public bool Start()
        {
            if (Client.EndPoint is not null
                && Client.ServerEndpoint is not null
                && !Client.IsActive)
            {
                return Client.Start();
            }
            else
            {
                return false;
            }
        }

        public bool Stop()
        {
            if (Client.IsActive)
            {
                Client.Stop();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RequestClientsList()
        {
            return Client.TransmitData(SimpleChatFramesFabric.CreateClientListRequestFrame());
        }

        public bool SendMessage(string toUser, string content)
        {
            var result = Client.TransmitData(SimpleChatFramesFabric.CreateMessageFrame(UserName, toUser, content));

            if (result)
            {
                MessageIsSentEvent?.Invoke(toUser, content);
            }

            return result;
        }

        public bool SetEndPoint(NetEndPoint? endPoint)
        {
            if (endPoint is null)
            {
                return false;
            }
            Client.SetEndPoint(endPoint);

            return true;
        }

        public bool SetServerEndPoint(NetEndPoint? endPoint)
        {
            if (endPoint is null)
            {
                return false;
            }
            Client.SetServerEndPoint(endPoint);

            return true;
        }

        public string GetStatus()
        {
            StringBuilder sb = new();

            sb.AppendLine($"Статус подключения клиента: {Client.ConnectionStatus};");
            sb.AppendLine($"Конечная точка клиента: {(Client.EndPoint == null ? "-" : Client.EndPoint)};");
            sb.Append($"Конечная точка сервера: {(Client.ServerEndpoint == null ? "-" : Client.ServerEndpoint)};");

            return sb.ToString();
        }
    }
}
