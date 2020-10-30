using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace PushSwap {
    public class PushSwap
    {
        private readonly StackProcessor _proc;

        public PushSwap(StackProcessor proc, string filename)
        {
            _proc = proc;
            while (_proc.stackA.Count > 2)
                _proc.Execute("pb");
            while (_proc.stackB.Count > 0)
                MakeNextMove();
            while (!_proc.CheckSuccess())
                _proc.Execute("ra");
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (string line in _proc.commandsList)
                    sw.WriteLine(line);
            }
        }

        private int[] FindBestMove(int[] bestMove, int stepA, int stepB)
        {
            var currentMove = new int[]{stepA, stepB,
                _proc.stackA.Count - stepA, _proc.stackB.Count - stepB};
            if (stepA + stepB - Math.Min(stepA, stepB) < bestMove.Sum())
                return new int[]{stepA, stepB, 0, 0};
            if (_proc.stackA.Count + _proc.stackB.Count - stepA - stepB - 
                Math.Min(_proc.stackA.Count - stepA, _proc.stackB.Count - stepB) < bestMove.Sum())
                return new int[]{0, 0, _proc.stackA.Count - stepA, _proc.stackB.Count - stepB};
            if (stepA + _proc.stackB.Count - stepB < bestMove.Sum())
                return new int[]{stepA, 0, 0, _proc.stackB.Count - stepB};
            if (stepB + _proc.stackA.Count - stepA < bestMove.Sum())
                return new int[]{0, stepB, _proc.stackA.Count - stepA, 0};
            return bestMove;
        }

        private bool IsNumberGood(LinkedListNode<int> ptrA, LinkedListNode<int> ptrB) {
            if (ptrA.Value == _proc.stackA.Min() && ptrB.Value < ptrA.Value)
                return true;
            if (GetPrevValue(_proc.stackA, ptrA) == _proc.stackA.Max() && ptrB.Value > _proc.stackA.Max())
                return true;
            if (GetPrevValue(_proc.stackA, ptrA) < ptrB.Value && ptrA.Value > ptrB.Value)
                return true;
            return false;
        }

        private void MakeNextMove()
        {
            LinkedListNode<int> ptrA = _proc.stackA.First;
            int stepA = 0;
            var bestMove = new int[]{_proc.stackA.Count, _proc.stackB.Count, 0, 0};
            while (ptrA != null)
            {
                LinkedListNode<int> ptrB = _proc.stackB.First;
                int stepB = 0;
                while (ptrB != null)
                {
                    if (IsNumberGood(ptrA, ptrB))
                        bestMove = FindBestMove(bestMove, stepA, stepB);
                    ptrB = ptrB.Next;
                    stepB += 1;
                }
                ptrA = ptrA.Next;
                stepA += 1;
            }
            while (bestMove[0] > 0 && bestMove[1] > 0)
            {
                _proc.Execute("rr");
                bestMove[0] -= 1;
                bestMove[1] -= 1;
            }
            while (bestMove[2] > 0 && bestMove[3] > 0)
            {
                _proc.Execute("rrr");
                bestMove[2] -= 1;
                bestMove[3] -= 1;
            }
            while (bestMove[0] > 0)
            {
                _proc.Execute("ra");
                bestMove[0] -= 1;
            }
            while (bestMove[1] > 0)
            {
                _proc.Execute("rb");
                bestMove[1] -= 1;
            }
            while (bestMove[2] > 0)
            {
                _proc.Execute("rra");
                bestMove[2] -= 1;
            }
            while (bestMove[3] > 0)
            {
                _proc.Execute("rrb");
                bestMove[3] -= 1;
            }
            _proc.Execute("pa");
        }

        private int GetPrevValue(LinkedList<int> stack, LinkedListNode<int> node)
        {
            if (node == stack.First) return stack.Last.Value;
            return node.Previous.Value;
        }
    }
}