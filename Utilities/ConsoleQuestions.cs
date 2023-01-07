/*!
 * Space.NET - a platform independent HTTP Server, running with .NET and C#.
 * https://github.com/TheBarnyOfBarnim/Space.NET
 * Copyright (C) 2023 Endric Barnekow <mail@e-barnekow.de>
 * https://github.com/TheBarnyOfBarnim/Space.NET/blob/master/LICENSE.md
 */

using System;

namespace Space.NET.Utilities
{
    public static class ConsoleQuestions
    {
        public static void WriteQuestionHead(string Title)
        {
            Console.WriteLine();
            Console.WriteLine(" === " + Title + " === ");
        }

        private static void WriteUserInputLine()
        {
            var OldForeColor = Console.ForegroundColor;
            var OldBackColor = Console.BackgroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(" User: ");
            Console.ForegroundColor = OldForeColor;
            Console.BackgroundColor = OldBackColor;
        }


        public static int AskNumberQuestion(string Title, string[] Answers)
        {
            (string, Action)[] NewAnswers = new (string, Action)[Answers.Length];
            for (int i = 0; i < Answers.Length; i++)
            {
                NewAnswers[i].Item1 = Answers[i];
                NewAnswers[i].Item2 = () => { };
            }

            return AskNumberQuestion(Title, NewAnswers);
        }

        public static int AskNumberQuestion(string Title, (string, Action)[] Answers)
        {
            WriteQuestionHead(Title);

            for (int i = 0; i < Answers.Length; i++)
            {
                Console.WriteLine($"  => [{i + 1}] " + Answers[i].Item1);
            }

            Console.WriteLine();
            WriteUserInputLine();

            string str = Console.ReadLine();
            int.TryParse(str, out int n);

            if (n == 0)
                return AskNumberQuestion(Title, Answers);

            Answers[n - 1].Item2.Invoke();

            return n;
        }

        public static int AskIntegerInput(string Title)
        {
            WriteQuestionHead(Title);
            WriteUserInputLine();
            var input = Console.ReadLine();
            while (int.Parse(input) == 0)
            {
                WriteInputError();
                input = Console.ReadLine();
            }
            return int.Parse(input);
        }

        public static string AskStringInput(string Title)
        {
            WriteQuestionHead(Title);
            WriteUserInputLine();
            return Console.ReadLine();
        }

        public static string AskPathInput(string Title)
        {
            WriteQuestionHead(Title);
            WriteUserInputLine();

            return Console.ReadLine().Replace("\"", "");
        }

        public static void WriteCheckingAnswers()
        {
            WriteCheckingAnswers("Checking Answers");
        }

        public static void WriteCheckingAnswers(string CustomMsg)
        {
            Console.WriteLine();
            Console.WriteLine(" ═════ " + CustomMsg + " ═════ ");
        }

        public static void WritePressAnyKeyToRestart()
        {
            Console.WriteLine();
            Console.WriteLine(" ══════════ Press any Key to go BACK ══════════");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static void WriteInputError()
        {
            WriteInputError("! ! !");
        }

        public static void WriteInputError(string CustomMsg)
        {
            var OldForeColor = Console.ForegroundColor;
            var OldBackColor = Console.BackgroundColor;
            Console.WriteLine();
            Console.Write("    ");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(CustomMsg);
            Console.ForegroundColor = OldForeColor;
            Console.BackgroundColor = OldBackColor;
        }


        public static void WriteTitlebar(string Title)
        {
            Console.Clear();
            Console.WriteLine("==========> " + Title + " <==========");
            Console.WriteLine();
        }
    }
}
