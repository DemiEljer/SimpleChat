using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Commands
{
    public class Command
    {
        /// <summary>
        /// Кличевые слова команды
        /// </summary>
        public string[] KeyWords { get; } = Array.Empty<string>();
        /// <summary>
        /// Описание команды
        /// </summary>
        public string Description { get; } = "";
        /// <summary>
        /// Логика 
        /// </summary>
        private Func<string[], bool>? _LogicHandler { get; set; } = null;

        public Command(string[] keyWords, string description, Func<string[], bool>? logicHandler)
        {
            KeyWords = keyWords.Select(kw => kw.Trim()).ToArray();
            Description = description;
            _LogicHandler = logicHandler;
        }

        public bool CheckKeyWord(string[] commandElements)
        {
            if (commandElements is not null
                && commandElements.Length > 0)
            {
                return KeyWords.Where(e => e.ToLower() == commandElements[0].ToLower()).Count() > 0;
            }
            else
            {
                return false;
            }
        }

        public bool TryInvoke(string command) => TryInvoke(command.Split(" ").Where(e => !string.IsNullOrEmpty(e)).ToArray());

        public bool TryInvoke(string[] commandElements)
        {
            if (CheckKeyWord(commandElements))
            {
                return _InvokeLogic(commandElements);
            }
            else
            {
                return false;
            }
        }

        private bool _InvokeLogic(string[] commandElements)
        {
            if (commandElements.Length > 1)
            {
                commandElements = commandElements.Take(new Range(1, commandElements.Length)).ToArray();
            }
            else
            {
                commandElements = Array.Empty<string>();
            }

            return _LogicHandler?.Invoke(commandElements) == true;
        }
    }
}
