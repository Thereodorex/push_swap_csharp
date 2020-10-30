using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace PushSwap
{
    class Program
    {
        private readonly String _commandsFileName;
        private readonly StackProcessor _proc;

        public Program(String profile, String numbersFileName, String commandsFileName) {
            _commandsFileName = commandsFileName;
            _proc = new StackProcessor(numbersFileName);
            if (profile == "profile=debug")
                RunDebugMode();
            else if (profile == "profile=checker")
                RunCheckerMode();
            else if (profile == "profile=push_swap")
                RunPushSwapMode();
            _proc.PrintResult();
        }

        private void RunDebugMode() {
            _proc.Print();
            string line = Console.ReadLine();
            while (line != "42")
            {
                _proc.Execute(line);
                _proc.Print();
                if (_proc.CheckSuccess())
                {
                    _proc.PrintResult();
                    break;
                }
                line = Console.ReadLine();
            }
        }

        private void RunCheckerMode() {
            using (StreamReader sr = new StreamReader(_commandsFileName))
            {
                while (sr.Peek() >= 0)
                {
                    string command = sr.ReadLine();
                    _proc.Execute(command);
                }
            }
        }

        private void RunPushSwapMode() {
            new PushSwap(_proc, _commandsFileName);
        }

        static void Main(string[] args)
        {
            new Program(args[0], args[1], args[2]);
        }
    }
}
