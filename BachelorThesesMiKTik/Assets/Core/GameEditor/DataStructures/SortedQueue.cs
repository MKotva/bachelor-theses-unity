using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Core.GameEditor.DataStructures
{
    public class SortedQueue<T, T1> where T1 : IComparable
    {
        public int Count { get; private set; }

        private Node first;
        private Node last;

        public SortedQueue(bool isMinimal)
        {

        }

        public void Enqueue(T data, T1 value) 
        {
            var newNode = new Node(data, value);

        }

        public T Dequeue()
        {
            return default(T);
        }


        public T Pop()
        {

        }


        private class Node
        {
            T Data { get; set; }
            T1 Value { get; set; }
            Node Next { get; set; }
            Node Previous { get; set; }

            public Node(T data, T1 value) 
            {
                Data = data;
                Value = value;
                Next = null;
                Previous = null;
            }
        }
    }
}
