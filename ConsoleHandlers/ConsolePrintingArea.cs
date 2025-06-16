using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.ConsoleHandlers
{
    public class ConsolePrintingArea
    {
        /// <summary>
        /// Строки области отрисовки консоли
        /// </summary>
        public List<string> Lines { get; } = new();

        private int _BaseLineIndex = 0;
        /// <summary>
        /// Индекс текущей базовой строки
        /// </summary>
        public int BaseLineIndex 
        { 
            get => _BaseLineIndex; 
            set => _BaseLineIndex = Math.Clamp(value, 0, Lines.Count); 
        }

        private object _LockObject { get; } = new();

        public (StringBuilder content, int height, int width) GetPrintingFrame(int screenHeight, int screenWidth)
        {
            StringBuilder sb = new();

            int currentHeight = 0;

            lock (_LockObject)
            {
                for (int i = Lines.Count - 1 - BaseLineIndex; i >= 0; i--)
                {
                    var line = Lines[i];

                    int lineHeight = 1 + line.Length / screenWidth;

                    if ((currentHeight + lineHeight) > screenHeight)
                    {
                        currentHeight = screenHeight;

                        var subLine = line.Substring(0, screenWidth * (screenHeight - currentHeight));

                        sb.Insert(0, subLine);
                    }
                    else
                    {
                        currentHeight += lineHeight;

                        sb.Insert(0, line);
                    }

                    if (currentHeight == screenHeight)
                    {
                        break;
                    }
                    else
                    {
                        sb.Insert(0, "\n");
                    }
                }
            }

            return (sb, currentHeight, screenWidth);
        }

        public (StringBuilder content, int height, int width) GetPrintingFrame(int screenWidth)
        {
            StringBuilder sb = new();

            int currentHeight = 0;

            lock (_LockObject)
            {
                for (int i = Lines.Count - 1 - BaseLineIndex; i >= 0; i--)
                {
                    var line = Lines[i];

                    int lineHeight = 1 + line.Length / screenWidth;

                    currentHeight += lineHeight;

                    sb.Insert(0, line);

                    if (i > 0)
                    {
                        sb.Insert(0, "\n");
                    }
                }
            }

            return (sb, currentHeight, screenWidth);
        }

        public void AppendChar(char symbole)
        {
            lock (_LockObject)
            {
                if (Lines.Count == 0)
                {
                    Lines.Add("");
                }
                Lines[Lines.Count - 1] = Lines[Lines.Count - 1] + symbole;
            }
        }

        internal void RemoveLastChar()
        {
            lock (_LockObject)
            {
                if (Lines.Count > 0)
                {
                    string line = Lines[Lines.Count - 1];

                    if (line.Length == 1)
                    {
                        Lines.Remove(line);
                    }
                    else
                    {
                        Lines[Lines.Count - 1] = line.Substring(0, line.Length - 1);
                    }
                }
            }
        }

        public void AppendLine(string line, bool followContent)
        {
            lock (_LockObject)
            {
                _AppendLine(line, followContent);
            }
        }

        private void _AppendLine(string line, bool followContent)
        {
            var subLines = line.Replace("\t", "    ").Replace("\r", "").Split("\n");

            int oldLinesCount = Lines.Count;

            foreach (var subLine in subLines)
            {
                Lines.Add(subLine);
            }

            if (!followContent)
            {
                BaseLineIndex += Lines.Count - oldLinesCount;
            }
        }

        public void AppendLines(IEnumerable<string> lines, bool followContent)
        {
            foreach (var line in lines)
            {
                _AppendLine(line, followContent);
            }
        }

        public void Clear()
        {
            lock (_LockObject)
            {
                Lines.Clear();
            }
            BaseLineIndex = 0;
        }
    }
}
