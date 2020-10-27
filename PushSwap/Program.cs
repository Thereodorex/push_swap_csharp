using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace PushSwap
{
    public class StackProcessor
    {
        private string filename;
        private int commandsCount;
        public LinkedList<int> stackA;
        public LinkedList<int> stackB;
        public Queue<String> commandsList;

        public StackProcessor(string filename)
        {
            this.filename = filename;
            stackA = new LinkedList<int>();
            stackB = new LinkedList<int>();
            commandsCount = 0;
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

        private string stackToString(LinkedList<int> stack) {
            string result = "";
            foreach(int n in stack)
            {
                result += " " + n;
            }
            return result;
        }

        private void printStackA()
        {
            Console.WriteLine("Stack A:");
            Console.WriteLine(stackToString(stackA));
        }

        private void printStackB()
        {
            Console.WriteLine("Stack B:");
            Console.WriteLine(stackToString(stackB));
        }

        public void print()
        {
            printStackA();
            printStackB();
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

        private void reverse(LinkedList<int> list)
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
            reverse(stackA);
        }

        private void rb()
        {
            reverse(stackB);
        }

        private void rr()
        {
            ra();
            rb();
        }

        private void rereverse(LinkedList<int> list)
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
            rereverse(stackA);
        }

        private void rrb()
        {
            rereverse(stackB);
        }

        private void rrr()
        {
            rra();
            rrb();
        }

        public void execute(string command)
        {
            Type thisType = this.GetType();
            MethodInfo method = thisType.GetMethod(command,
            BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null && command.Length > 1 && command.Length < 4)
            {
                method.Invoke(this, new object[]{});
                commandsCount += 1;
                commandsList.Enqueue(command);
            }
            else
                Console.WriteLine("Unknown command");
        }

        public bool isAscending(LinkedList<int> stack)
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

        public bool checkSuccess()
        {
            return (stackB.Count == 0 && isAscending(stackA));
        }

        public int getCount()
        {
            return commandsCount;
        }

        public void printResult()
        {
            if (checkSuccess())
                Console.WriteLine("SUCCESS(" + commandsCount + ")");
            else
                Console.WriteLine("FAILURE(" + commandsCount + ")");
        }
    }

    public class PushSwap
    {
        private StackProcessor proc;

        public PushSwap(StackProcessor proc, string filename)
        {
            this.proc = proc;
            while (proc.stackA.Count > 2)
                proc.execute("pb");
            while (proc.stackB.Count > 0)
                makeNextMove();
            while (!proc.checkSuccess())
                proc.execute("ra");
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (string line in proc.commandsList)
                    sw.WriteLine(line);
            }
        }

        private int[] findBestMove(int[] bestMove, int stepA, int stepB)
        {
            var currentMove = new int[]{stepA, stepB,
                proc.stackA.Count - stepA, proc.stackB.Count - stepB};
            if (stepA + stepB - Math.Min(stepA, stepB) < bestMove.Sum())
                return new int[]{stepA, stepB, 0, 0};
            if (proc.stackA.Count + proc.stackB.Count - stepA - stepB - 
                Math.Min(proc.stackA.Count - stepA, proc.stackB.Count - stepB) < bestMove.Sum())
                return new int[]{0, 0, proc.stackA.Count - stepA, proc.stackB.Count - stepB};
            if (stepA + proc.stackB.Count - stepB < bestMove.Sum())
                return new int[]{stepA, 0, 0, proc.stackB.Count - stepB};
            if (stepB + proc.stackA.Count - stepA < bestMove.Sum())
                return new int[]{0, stepB, proc.stackA.Count - stepA, 0};
            return bestMove;
        }

        private bool isNumberGood(LinkedListNode<int> ptrA, LinkedListNode<int> ptrB) {
            if (ptrA.Value == minValue(proc.stackA) && ptrB.Value < ptrA.Value)
                return true;
            if (getPrevValue(proc.stackA, ptrA) == maxValue(proc.stackA) && ptrB.Value > maxValue(proc.stackA))
                return true;
            if (getPrevValue(proc.stackA, ptrA) < ptrB.Value && ptrA.Value > ptrB.Value)
                return true;
            return false;
        }

        private void makeNextMove()
        {
            LinkedListNode<int> ptrA = proc.stackA.First;
            LinkedListNode<int> ptrB = proc.stackB.First;
            int stepA = 0;
            int stepB = 0;
            var bestMove = new int[]{proc.stackA.Count, proc.stackB.Count, 0, 0};
            while (ptrA != null)
            {
                ptrB = proc.stackB.First;
                stepB = 0;
                while (ptrB != null)
                {
                    if (isNumberGood(ptrA, ptrB))
                        bestMove = findBestMove(bestMove, stepA, stepB);
                    ptrB = ptrB.Next;
                    stepB += 1;
                }
                ptrA = ptrA.Next;
                stepA += 1;
            }
            while (bestMove[0] > 0 && bestMove[1] > 0)
            {
                proc.execute("rr");
                bestMove[0] -= 1;
                bestMove[1] -= 1;
            }
            while (bestMove[2] > 0 && bestMove[3] > 0)
            {
                proc.execute("rrr");
                bestMove[2] -= 1;
                bestMove[3] -= 1;
            }
            while (bestMove[0] > 0)
            {
                proc.execute("ra");
                bestMove[0] -= 1;
            }
            while (bestMove[1] > 0)
            {
                proc.execute("rb");
                bestMove[1] -= 1;
            }
            while (bestMove[2] > 0)
            {
                proc.execute("rra");
                bestMove[2] -= 1;
            }
            while (bestMove[3] > 0)
            {
                proc.execute("rrb");
                bestMove[3] -= 1;
            }
            proc.execute("pa");
        }

        private int getPrevValue(LinkedList<int> stack, LinkedListNode<int> node)
        {
            if (node == stack.First) return stack.Last.Value;
            return node.Previous.Value;
        }

        private int minValue(LinkedList<int> stack)
        {
            int min = stack.First.Value;
            foreach(int i in stack)
            {
                if (i < min) min = i;
            }
            return min;
        }

        private int maxValue(LinkedList<int> stack)
        {
            int max = stack.First.Value;
            foreach(int i in stack)
            {
                if (i > max) max = i;
            }
            return max;
        }
    }

    class Program
    {
        private String commandsFileName;
        private String numbersFileName;
        private StackProcessor proc;

        public Program(String profile, String numbers, String commands) {
            commandsFileName = commands;
            numbersFileName = numbers;
            proc = new StackProcessor(numbersFileName);
            if (profile == "profile=debug")
                runDebugMode();
            else if (profile == "profile=checker")
                runCheckerMode();
            else if (profile == "profile=push_swap")
                runPushSwapMode();
            proc.printResult();
        }

        private void runDebugMode() {
            proc.print();
            string line = Console.ReadLine();
            while (line != "42")
            {
                proc.execute(line);
                proc.print();
                if (proc.checkSuccess())
                {
                    proc.printResult();
                    break;
                }
                line = Console.ReadLine();
            }
        }

        private void runCheckerMode() {
            using (StreamReader sr = new StreamReader(commandsFileName))
            {
                while (sr.Peek() >= 0)
                {
                    string command = sr.ReadLine();
                    proc.execute(command);
                }
            }
        }

        private void runPushSwapMode() {
            new PushSwap(proc, commandsFileName);
        }

        static void Main(string[] args)
        {
            new Program(args[0], args[1], args[2]);
        }
    }
}
