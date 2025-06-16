using SimpleChat.Commands;
using SimpleChat.ConsoleHandlers;
using SimpleChat.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat
{
    public class ApplicationHandler
    {
        /// <summary>
        /// Состояние приложения
        /// </summary>
        public bool ApplicationState { get; set; } = false;
        /// <summary>
        /// Поток приложения
        /// </summary>
        private Task? _ApplicationTask { get; set; } = null;
        /// <summary>
        /// Менеджер команд
        /// </summary>
        public CommandsManager Commands { get; } = new();

        public SimpleChatClient Client { get; } = new();

        public SimpleChatServer Server { get; } = new();

        public bool ServerStartingFlag { get; set; } = false;

        public bool ClientStartingFlag { get; set; } = false;

        public Task Start()
        {
            ApplicationState = true;

            _ApplicationTask = new Task(() =>
            {
                SpinWait waiter = new();

                _ApplicationStarting();

                while (ApplicationState)
                {
                    _ApplicationLoop();

                    waiter.SpinOnce();
                }

                _ApplicationStopping();
            });
            _ApplicationTask.Start();

            return _ApplicationTask;
        }

        private void _ApplicationStarting()
        {
            ConsoleHandler.InputEvent += (content) =>
            {
                var result = Commands.TryInvoke(content);

                if (!result)
                {
                    ConsoleHandler.AppendOutputContent($"Ошибка команды (!h - справка)");
                }
            };

            ApplicationCommands.InitCommands(this);

            Commands.TryInvoke("!i");

            Client.ConnectionEvent += () =>
            {
                _ShowMessage($"Клиент был подключен к серверу");
            };

            Client.DisconnectionEvent += () =>
            {
                _ShowMessage($"Клиент был отключен от сервера");
            };

            Client.ClientsListReceiveEven += (clients) =>
            {
                StringBuilder sb = new();
                sb.Append("Список клиентов сервера:");
                foreach (var client in clients)
                {
                    sb.Append($"\r\n\"{client}\"");
                }
                _ShowMessage(sb.ToString());
            };

            Client.MessageIsSentEvent += (to, content) =>
            {
                _ShowMessage($"Отправлено для \"{to}\": \"{content}\"");
            };

            Client.MessageIsReceivedEvent += (from, content) =>
            {
                _ShowMessage($"Принято от \"{from}\": \"{content}\"");
            };

            Server.ClientHasConnectedEvent += (client) =>
            {
                _ShowMessage($"Был подключен клиент \"{client.name}\": {client.client.EndPoint}");
            };

            Server.ClientHasDisconnectedEvent += (client) =>
            {
                _ShowMessage($"Был отключен клиент \"{client.name}\": {client.client.EndPoint}");
            };

            Server.MessageIsSentEvent += (message) =>
            {
                _ShowMessage($"Отправка сообщения: от \"{message.from}\" для \"{message.to}\": \"{message.content}\"");
            };

            Server.MessageSendingErrorEvent += (message) =>
            {
                _ShowMessage($"Ошибка отправки сообщения: от \"{message.from}\" для \"{message.to}\": \"{message.content}\"");
            };

            ConsoleHandler.Start();
        }

        private void _ApplicationStopping()
        {
            ConsoleHandler.Stop();
            Client.Stop();
            Server.Stop();
        }

        private void _ApplicationLoop()
        {
            
        }

        private void _ShowMessage(string messageContent)
        {
            ConsoleHandler.AppendOutputContent($"{DateTime.Now} :: {messageContent}");
        }
    }
}
