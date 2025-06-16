using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Commands
{
    public class CommandsManager
    {
        private List<Command> _Commands { get; } = new();

        public IEnumerable<Command> Commands => _Commands;

        public void AppendCommand(string[] keyWords, string description, Func<string[], bool>? logicHandler)
        {
            _Commands.Add(new Command(keyWords, description, logicHandler));
        }

        public bool TryInvoke(string commandContent)
        {
            bool result = false;

            foreach (var command in _Commands)
            {
                result |= command.TryInvoke(commandContent);
            }

            return result;
        }

        public bool TryInvoke(string[] commandElements)
        {
            bool result = false;

            foreach (var command in _Commands)
            {
                result |= command.TryInvoke(commandElements);
            }

            return result;
        }
    }
}
