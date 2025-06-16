using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.ConsoleHandlers
{
    public class ConsoleCommandsHistory
    {
        private List<string> _CommandsHistory { get; } = new();

        private int _CurrentCommandIndex { get; set; } = 0;

        public string GetNextCommand()
        {
            _CurrentCommandIndex = Math.Clamp(++_CurrentCommandIndex, 1, _CommandsHistory.Count + 1);

            if (_CurrentCommandIndex == (_CommandsHistory.Count + 1))
            {
                return "";
            }
            else
            {
                return _CommandsHistory[_CommandsHistory.Count - _CurrentCommandIndex];
            }
        }

        public string GetPrevCommand()
        {
            _CurrentCommandIndex = Math.Clamp(--_CurrentCommandIndex, 0, _CommandsHistory.Count);

            if (_CurrentCommandIndex == 0)
            {
                return "";
            }
            else
            {
                return _CommandsHistory[_CommandsHistory.Count - _CurrentCommandIndex];
            }
        }

        public void AddCommand(string command)
        {
            _CommandsHistory.Add(command);
            _CurrentCommandIndex = 0;
        }

        public void ResetIndex()
        {
            _CurrentCommandIndex = 0;
        }
    }
}
