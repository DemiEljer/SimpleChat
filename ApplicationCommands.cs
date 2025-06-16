using CustomTCPServerLibrary.Base;
using SimpleChat.Commands;
using SimpleChat.ConsoleHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat
{
    public static class ApplicationCommands
    {
        public static void InitCommands(ApplicationHandler app)
        {
            app.Commands.AppendCommand
            (
                new string[] { "!i", "!info" }
                , "Информация о приложении"
                , (args) =>
                {
                    if (args.Length > 0)
                    {
                        return false;
                    }

                    List<string> outputContent = new();

                    outputContent.Add("SimpleChat v1.0.0");
                    outputContent.Add("> Данное приложение позволяет создать сервер для реализации механизма " +
                        "простого тестового общения, также, оно позволяет подключиться к серверу и отправлять сообщения в качестве клиента.");
                    outputContent.Add("> Для запуска клиента необходимо использовать команды \"!user\", \"!init\" и \"!connect\".");
                    outputContent.Add("> Для запуска сервера необходимо использовать команду \"!start\".");
                    outputContent.Add("> Для отправки сообщения команду \"!message\", указав первым аргументом имя пользователя, а далее - текст сообщения.");
                    outputContent.Add("> Для более подробной информации о принципах взаимодействия с приложением введите команду (\"!h\").");

                    ConsoleHandler.AppendOutputContent(outputContent);

                    return true;
                }
            );
            app.Commands.AppendCommand
            (
                new string[] { "!h", "!help" }
                , "Показать список команд"
                , (args) =>
                {
                    if (args.Length > 0)
                    {
                        return false;
                    }

                    List<string> outputContent = new();
                    outputContent.Add("> Список команд:");
                    foreach (var command in app.Commands.Commands)
                    {
                        outputContent.Add($"\"{string.Join("\", \"", command.KeyWords)}\" - {command.Description};");
                    }
                    outputContent.Add("> Кнопки управления:");
                    outputContent.Add("Esc - возврат к отслеживанию последних выводимых сообщений;");
                    outputContent.Add("Стрелка вверх/вниз - просмотр истории вывода;");
                    outputContent.Add("Стрелка вправо/влево - просмотр истории введенных команд;");
                    outputContent.Add("Enter - ввод команды;");
                    outputContent.Add("Backspace - удаление символов команды;");
                    outputContent.Add("Delete - удаление всего содержимого команды;");

                    ConsoleHandler.AppendOutputContent(outputContent);

                    return true;
                }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!e", "!exit" }
                 , "Прекратить работу приложения"
                 , (args) =>
                 {
                     if (args.Length > 0)
                     {
                         return false;
                     }

                     app.ApplicationState = false;

                     return true;
                 }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!c", "!clear" }
                 , "Очистить область вывода"
                 , (args) =>
                 {
                     if (args.Length > 0)
                     {
                         return false;
                     }

                     ConsoleHandler.Clear();
                     ConsoleHandler.FollowOutputEnd = true;

                     return true;
                 }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!us", "!user" }
                 , "Установить или прочитать имя пользователя (\"!us [username]\")"
                 , (args) =>
                 {
                     if (args.Length == 0)
                     {
                         ConsoleHandler.AppendOutputContent($"> Имя пользователя: {app.Client.UserName}.");
                     }
                     else if (args.Length == 1)
                     {
                         if (!app.Client.Client.IsActive)
                         {
                             app.Client.UserName = args[0];
                             ConsoleHandler.AppendOutputContent($"> Имя пользователя изменено: {app.Client.UserName}.");
                         }
                         else
                         {
                             ConsoleHandler.AppendOutputContent($"> Невозможно поменять имя пользователя во время активной работы клиента.");
                         }
                     }
                     else
                     {
                         return false;
                     }

                     return true;
                 }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!in", "!init" }
                 , "Задать адрес конечной точки пользователя (\"!in xx.xx.xx.xx:yyy\")"
                 , (args) =>
                 {
                     if (args.Length != 1)
                     {
                         return false;
                     }

                     if (!app.Client.Client.IsActive)
                     {
                         if (app.Client.SetEndPoint(GetEndPoint(args[0])))
                         {
                             ConsoleHandler.AppendOutputContent($"> Конечная точка клиента установлена: {app.Client.Client.EndPoint}.");
                         }
                         else
                         {
                             ConsoleHandler.AppendOutputContent($"> Ошибка чтения конечной точки клиента: {args[0]}.");
                         }
                     }
                     else
                     {
                         ConsoleHandler.AppendOutputContent($"> Невозможно поменять конечную точку во время активной работы клиента.");
                     }

                     return true;
                 }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!ct", "!connect" }
                 , "Подключиться к серверу (\"!ct xx.xx.xx.xx:yyy\")"
                 , (args) =>
                 {
                     if (args.Length != 1)
                     {
                         return false;
                     }

                     if (app.Client.SetServerEndPoint(GetEndPoint(args[0])))
                     {
                         ConsoleHandler.AppendOutputContent($"> Конечная точка сервера установлена: {app.Client.Client.ServerEndpoint}.");
                     }
                     else
                     {
                         ConsoleHandler.AppendOutputContent($"> Ошибка чтения конечной точки сервера: {args[0]}.");
                     }

                     if (app.Client.Start())
                     {
                         ConsoleHandler.AppendOutputContent($"> Клиент был запущен.");
                         app.ClientStartingFlag = true;
                     }
                     else
                     {
                         ConsoleHandler.AppendOutputContent($"> Ошибка запуска клиента или клиент уже запущен.");
                     }

                     return true;
                 }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!dt", "!disconnect" }
                 , "Отключиться от сервера"
                 , (args) =>
                 {
                     if (args.Length > 0)
                     {
                         return false;
                     }

                     if (app.Client.Stop())
                     {
                         ConsoleHandler.AppendOutputContent("> Работа клиента остановлена.");
                     }
                     else
                     {
                         ConsoleHandler.AppendOutputContent("> Работа клиента уже была остановлена.");
                     }

                     return true;
                 }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!cl", "!clients" }
                 , "Получить список клиентов сервера"
                 , (args) =>
                 {
                     if (args.Length > 0)
                     {
                         return false;
                     }

                     if (!app.Client.RequestClientsList())
                     {
                         ConsoleHandler.AppendOutputContent("> Невозможно отправить запрос на сервер.");
                     }

                     return true;
                 }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!ms", "!message" }
                 , "Отправить сообщение (\"!ms <user_name> <message>\")"
                 , (args) =>
                 {
                     if (args.Length < 2)
                     {
                         return false;
                     }

                     if (!app.Client.SendMessage(args[0], string.Join(" ", args.Take(new Range(1, args.Length)))))
                     {
                         ConsoleHandler.AppendOutputContent("> Невозможно отправить запрос на сервер.");
                     }

                     return true;
                 }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!ss", "!status" }
                 , "Вывести информацию о статусах подключения"
                 , (args) =>
                 {
                     if (args.Length > 0)
                     {
                         return false;
                     }

                     List<string> outputContent = new();
                     outputContent.Add("> Статусы клиента:");
                     outputContent.Add(app.Client.GetStatus());
                     outputContent.Add("> Статусы сервера:");
                     outputContent.Add(app.Server.GetStatus());

                     ConsoleHandler.AppendOutputContent(outputContent);

                     return true;
                 }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!st", "!start" }
                 , "Запустить работу сервера (\"!st xx.xx.xx.xx:yyy\")"
                 , (args) =>
                 {
                     if (args.Length != 1)
                     {
                         return false;
                     }

                     if (app.Server.SetEndPoint(GetEndPoint(args[0])))
                     {
                         ConsoleHandler.AppendOutputContent($"> Конечная точка сервера установлена: {app.Client.Client.EndPoint}.");
                     }
                     else
                     {
                         ConsoleHandler.AppendOutputContent($"> Ошибка чтения конечной точки сервера: {args[0]}.");
                     }

                     if (app.Server.Start())
                     {
                         ConsoleHandler.AppendOutputContent($"> Сервер был запущен.");
                         app.ServerStartingFlag = true;
                     }
                     else
                     {
                         ConsoleHandler.AppendOutputContent($"> Ошибка запуска сервера или сервер уже запущен.");
                     }

                     return true;
                 }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!sp", "!stop" }
                 , "Остановить работу сервера"
                 , (args) =>
                 {
                     if (args.Length > 0)
                     {
                         return false;
                     }

                     if (app.Server.Stop())
                     {
                         ConsoleHandler.AppendOutputContent("> Работа сервера остановлена.");
                     }
                     else
                     {
                         ConsoleHandler.AppendOutputContent("> Работа сервера уже была остановлена.");
                     }

                     return true;
                 }
            );
            app.Commands.AppendCommand
            (
                 new string[] { "!rs", "!restart" }
                 , "Перезапустить работу клиента или сервера (\"!rs [client]/[server]\")"
                 , (args) =>
                 {
                     if (args.Length > 1)
                     {
                         return false;
                     }

                     if (args.Length == 0)
                     {
                         if (app.ClientStartingFlag)
                         {
                             if (app.Client.Start())
                             {
                                 ConsoleHandler.AppendOutputContent($"> Клиент был запущен.");
                             }
                             else
                             {
                                 ConsoleHandler.AppendOutputContent($"> Ошибка запуска клиента или клиент уже запущен.");
                             }
                         }
                         else
                         {
                             ConsoleHandler.AppendOutputContent($"> Клиент не был до этого запущен.");
                         }
                         if (app.ServerStartingFlag)
                         {
                             if (app.Server.Start())
                             {
                                 ConsoleHandler.AppendOutputContent($"> Сервер был запущен.");
                                 app.ServerStartingFlag = true;
                             }
                             else
                             {
                                 ConsoleHandler.AppendOutputContent($"> Ошибка запуска сервера или сервер уже запущен.");
                             }
                         }
                         else
                         {
                             ConsoleHandler.AppendOutputContent($"> Сервер не был до этого запущен.");
                         }
                     }
                     else if (args[0] == "client")
                     {
                         if (app.ClientStartingFlag)
                         {
                             if (app.Client.Start())
                             {
                                 ConsoleHandler.AppendOutputContent($"> Клиент был запущен.");
                             }
                             else
                             {
                                 ConsoleHandler.AppendOutputContent($"> Ошибка запуска клиента или клиент уже запущен.");
                             }
                         }
                         else
                         {
                             ConsoleHandler.AppendOutputContent($"> Клиент не был до этого запущен.");
                         }
                     }
                     else if (args[0] == "server")
                     {
                         if (app.ServerStartingFlag)
                         {
                             if (app.Server.Start())
                             {
                                 ConsoleHandler.AppendOutputContent($"> Сервер был запущен.");
                                 app.ServerStartingFlag = true;
                             }
                             else
                             {
                                 ConsoleHandler.AppendOutputContent($"> Ошибка запуска сервера или сервер уже запущен.");
                             }
                         }
                         else
                         {
                             ConsoleHandler.AppendOutputContent($"> Сервер не был до этого запущен.");
                         }
                     }
                     else
                     {
                         return false;
                     }

                     return true;
                 }
            );
        }

        private static NetEndPoint? GetEndPoint(string address)
        {
            var addressElements = address.Split(':');

            try
            {
                return new NetEndPoint(IPAddress.Parse(addressElements[0]), int.Parse(addressElements[1]));
            }
            catch
            {
                return null;
            }
        }
    }
}
