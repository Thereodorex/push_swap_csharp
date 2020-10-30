using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace PushSwap {
    public class StackProcessor
    {
        private int _commandsCount;
        public LinkedList<int> stackA;
        public LinkedList<int> stackB;
        public Queue<String> commandsList;

        public StackProcessor(string filename)
        {
            stackA = new LinkedList<int>();
            stackB = new LinkedList<int>();
            _commandsCount = 0;
            commandsList = new Queue<String>();
            using (StreamReader sr = new StreamReader(filename))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    int value = Int32.Parse(line);
                    if (stackA.Find(value) != null)
                        throw new Exception("Duplicate numbers");
                    stackA.AddLast(value);
                }
            }
        }

        private string StackToString(LinkedList<int> stack)
        {
            return stack.Aggregate("", (current, n) => current + (" " + n));
        }

        private void PrintStackA()
        {
            Console.WriteLine("Stack A:");
            Console.WriteLine(StackToString(stackA));
        }

        private void PrintStackB()
        {
            Console.WriteLine("Stack B:");
            Console.WriteLine(StackToString(stackB));
        }

        public void Print()
        {
            PrintStackA();
            PrintStackB();
        }

        private void pb()
        {
            if (stackA.Count > 0)
            {
                stackB.AddFirst(stackA.First.Value);
                stackA.RemoveFirst();
            }
        }

        private void pa()
        {
            if (stackB.Count > 0)
            {
                stackA.AddFirst(stackB.First.Value);
                stackB.RemoveFirst();
            }
        }

        private void swap(LinkedList<int> list)
        {
            if (list.Count > 1)
            {
                int first = list.First.Value;
                list.RemoveFirst();
                list.AddAfter(list.First, first);
            }
        }

        private void sa()
        {
            swap(stackA);
        }

        private void sb()
        {
            swap(stackB);
        }

        private void Reverse(LinkedList<int> list)
        {
            if (list.Count > 1)
            {
                int first = list.First.Value;
                list.RemoveFirst();
                list.AddLast(first);
            }
        }

        private void ra()
        {
            Reverse(stackA);
        }

        private void rb()
        {
            Reverse(stackB);
        }

        private void rr()
        {
            ra();
            rb();
        }

        private void Rereverse(LinkedList<int> list)
        {
            if (list.Count > 1)
            {
                int last = list.Last.Value;
                list.RemoveLast();
                list.AddFirst(last);
            }
        }

        private void rra()
        {
            Rereverse(stackA);
        }

        private void rrb()
        {
            Rereverse(stackB);
        }

        private void rrr()
        {
            rra();
            rrb();
        }

        public void Execute(string command)
        {
            Type thisType = this.GetType();
            MethodInfo method = thisType.GetMethod(command,
            BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null && command.Length > 1 && command.Length < 4)
            {
                method.Invoke(this, new object[]{});
                _commandsCount += 1;
                commandsList.Enqueue(command);
            }
            else
                Console.WriteLine("Unknown command");
        }

        public bool IsAscending(LinkedList<int> stack)
        {
            LinkedListNode<int> currentNode = stack.First;
            while (currentNode.Next != null)
            {
                if (currentNode.Value > currentNode.Next.Value)
                    return false;
                currentNode = currentNode.Next;
            }
            return true;
        }

        public bool CheckSuccess()
        {
            return (stackB.Count == 0 && IsAscending(stackA));
        }

        public int GetCount()
        {
            return _commandsCount;
        }

        public void PrintResult()
        {
            if (CheckSuccess())
                Console.WriteLine("SUCCESS(" + _commandsCount + ")");
            else
                Console.WriteLine("FAILURE(" + _commandsCount + ")");
        }
    }
}