using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.ConsoleHandlers
{
    public static class ConsoleHandler
    {
        private static Thread? _ReadingThread { get; set; }
        private static Thread? _PrintingThread { get; set; }
        private static ConsolePrintingArea _OutputArea { get; } = new();
        private static ConsolePrintingArea _InputArea { get; } = new();
        private static ConsoleCommandsHistory _History { get; } = new();

        public static bool IsActive { get; private set; } = false;

        public static event Action<string>? InputEvent;

        public static bool FollowOutputEnd { get; set; } = true;

        private static string _PreviousOutputFrameContent { get; set; } = "";
        private static string _PreviousInputFrameContent { get; set; } = "";
        private static bool _PreviousFollowOutputEnd { get; set; } = true;

        public static void Start()
        {
            IsActive = true;

            _ReadingThread = new Thread(() =>
            {
                SpinWait waiter = new();

                while (IsActive)
                {
                    _ReadingThreadLogic();

                    waiter.SpinOnce();
                }

            });
            _PrintingThread = new Thread(() =>
            {
                SpinWait waiter = new();

                while (IsActive)
                {
                    _PrintingThreadLogic();

                    waiter.SpinOnce();
                }
            });

            _ReadingThread.Start();
            _PrintingThread.Start();
        }

        public static void Stop()
        {
            IsActive = false;
        }

        private static void _PrintingThreadLogic()
        {
            int currentScreenHeight = Console.WindowHeight;
            int currentScreenWidth = Console.WindowWidth;

            var inputAreaContent = _InputArea.GetPrintingFrame(currentScreenWidth);

            int targetOutputAreaContentHeight = 0;

            if (inputAreaContent.height == 0)
            {
                targetOutputAreaContentHeight = currentScreenHeight - 2;
            }
            else
            {
                targetOutputAreaContentHeight = currentScreenHeight - 1 - inputAreaContent.height;
            }

            var outputAreaContent = _OutputArea.GetPrintingFrame(currentScreenHeight - 2, currentScreenWidth);

            StringBuilder outputFrameBuilder = new StringBuilder();

            outputFrameBuilder.Append(outputAreaContent.content);
            outputFrameBuilder.Append("\r\n");
            for (int i = outputAreaContent.height; i < (targetOutputAreaContentHeight); i++)
            {
                outputFrameBuilder.Append("\r\n");
            }

            StringBuilder inputFrameBuilder = new StringBuilder();

            inputFrameBuilder.AppendLine(new string(Enumerable.Range(0, currentScreenWidth).Select(e => '-').ToArray()));
            inputFrameBuilder.Append(inputAreaContent.content);

            var newFrameOutputContent = outputFrameBuilder.ToString();
            var newFrameInputContent = inputFrameBuilder.ToString();

            if (newFrameOutputContent != _PreviousOutputFrameContent
                || newFrameInputContent != _PreviousInputFrameContent
                || FollowOutputEnd != _PreviousFollowOutputEnd)
            {
                _PreviousFollowOutputEnd = FollowOutputEnd;
                _PreviousOutputFrameContent = newFrameOutputContent;
                _PreviousInputFrameContent = newFrameInputContent;

                Console.Clear();

                if (FollowOutputEnd)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

                Console.Write(newFrameOutputContent);

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write(newFrameInputContent);
            }
        }

        public static void AppendOutputContent(string content)
        {
            _OutputArea.AppendLine(content, FollowOutputEnd);
        }

        public static void AppendOutputContent(IEnumerable<string> content)
        {
            _OutputArea.AppendLines(content, FollowOutputEnd);
        }

        private static void _ReadingThreadLogic()
        {
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.Enter)
            {
                if (_InputArea.Lines.Count > 0)
                {
                    InputEvent?.Invoke(_InputArea.Lines.Last());
                }
                if (_InputArea.Lines.Count > 0)
                {
                    _History.AddCommand(_InputArea.Lines.First());
                }
                _InputArea.Clear();
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                if (!FollowOutputEnd)
                {
                    FollowOutputEnd = !FollowOutputEnd;
                    if (FollowOutputEnd)
                    {
                        _OutputArea.BaseLineIndex = 0;
                    }
                }
            }
            else if (key.Key == ConsoleKey.UpArrow)
            {
                if (_OutputArea.Lines.Count > 1)
                {
                    FollowOutputEnd = false;
                    _OutputArea.BaseLineIndex++;
                }
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                if (_OutputArea.Lines.Count > 1)
                {
                    FollowOutputEnd = false;
                    _OutputArea.BaseLineIndex--;
                }
            }
            else if (key.Key == ConsoleKey.LeftArrow)
            {
                _InputArea.Clear();
                _InputArea.AppendLine(_History.GetNextCommand(), true);
            }
            else if (key.Key == ConsoleKey.RightArrow)
            {
                _InputArea.Clear();
                _InputArea.AppendLine(_History.GetPrevCommand(), true);
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                _InputArea.RemoveLastChar();
            }
            else if (key.Key == ConsoleKey.Delete)
            {
                _InputArea.Clear();
                _History.ResetIndex();
            }
            else
            {
                _InputArea.AppendChar(key.KeyChar);
            }
        }

        public static void Clear()
        {
            _OutputArea.Clear();
            _InputArea.Clear();
        }
    }
}
