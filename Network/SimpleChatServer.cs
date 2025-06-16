using CustomTCPServerLibrary;
using CustomTCPServerLibrary.Base;
using SimpleChat.Network.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Network
{
    public class SimpleChatServer
    {
        public CustomTCPServer Server { get; } = new();

        public event Action? StartingErrorEvent;

        public event Action<(CustomTCPServerClient client, string name)>? ClientHasConnectedEvent;

        public event Action<(CustomTCPServerClient client, string name)>? ClientHasDisconnectedEvent;

        public event Action<(string from, string to, string content)>? MessageIsSentEvent;

        public event Action<(string? from, string? to, string? content)>? MessageSendingErrorEvent;

        private SimpleChatServerClients _Clients { get; } = new();

        public SimpleChatServer()
        {
            Server.ClientHasConnectedEvent += (_, client) =>
            {
                client.ReceiveDataEvent += (_client, data) =>
                {
                    var frameObject = SimpleChatFramesFabric.ParseFrame(data);

                    if (frameObject is not null)
                    {
                        if (frameObject is MessageFrame)
                        {
                            var _frame = (MessageFrame)frameObject;

                            var toClient = _Clients.GetClientByName(_frame.ClientTo);

                            if (toClient is not null
                                && _frame.ClientFrom is not null
                                && _frame.ClientTo is not null
                                && _frame.MessageContent is not null)
                            {
                                toClient.TransmitData(SimpleChatFramesFabric.CreateMessageFrame(_frame.ClientFrom, _frame.ClientTo, _frame.MessageContent));

                                MessageIsSentEvent?.Invoke((_frame.ClientFrom, _frame.ClientTo, _frame.MessageContent));
                            }
                            else
                            {
                                MessageSendingErrorEvent?.Invoke((_frame.ClientFrom, _frame.ClientTo, _frame.MessageContent));
                            }
                        }
                        else if (frameObject is ClientListRequestFrame)
                        {
                            _client.TransmitData(SimpleChatFramesFabric.CreateClientsListFrame(_Clients.GetClientNames()));
                        }
                        else if (frameObject is RegistrationFrame)
                        {
                            var _frame = (RegistrationFrame)frameObject;

                            if (_frame.Name is not null)
                            {
                                ClientHasConnectedEvent?.Invoke(_Clients.AppendClient(_client, _frame.Name));
                            }
                        }
                    }
                };
            };
            Server.ClientHasDisconnectedEvent += (_, client) =>
            {
                var _client = _Clients.RemoveClient(client);

                if (_client.client != null)
                {
                    ClientHasDisconnectedEvent?.Invoke(_client);
                }
            };
            Server.StartingExceptionThrowingEvent += (_, e) =>
            {
                StartingErrorEvent?.Invoke();
            };
            Server.HasStoppedEvent += (_) =>
            {
                _Clients.Clear();
            };
        }

        public bool Start()
        {
            if (Server.EndPoint is not null
                && !Server.IsActive)
            {
                Server.Start();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Stop()
        {
            if (Server.IsActive)
            {
                Server.Stop();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetEndPoint(NetEndPoint? endPoint)
        {
            if (endPoint is null)
            {
                return false;
            }
            Server.SetEndPoint(endPoint);

            return true;
        }

        public string GetStatus()
        {
            StringBuilder sb = new();

            sb.AppendLine($"Состояние активной работы сервера: {Server.IsActive};");
            sb.Append($"Конечная точка сервера: {(Server.EndPoint == null ? "-" : Server.EndPoint)};");
            if (Server.IsActive)
            {
                sb.Append($"\r\nПодключенные клиенты:");
                foreach (var client in _Clients.Clients)
                {
                    sb.Append($"\r\n{client.name} : {client.client.EndPoint};");
                }
            }

            return sb.ToString();
        }
    }
}
