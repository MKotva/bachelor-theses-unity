using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexTest
{
    public class TestObject
    {
        public float Speed { get; set; }

        public void PrintTest()
        {
            Console.WriteLine("Test1");
        }
        public int PrintTest(float first)
        {
            Console.WriteLine($"Test1 iteration {first}");
            return 0;
        }
        public int PrintTest(float first, float second)
        {
            Console.WriteLine($"Test {first}, {second}");
            return 0;
        }

        public int PrintString(string text, float second)
        {
            Console.WriteLine($"{text} : {second}");
            return 0;
        }

        public int PrintTest(bool test)
        {
            if(test)
                Console.WriteLine("Test2");
            return 0;
        }

        public void PrintSpeed()
        {
            Console.WriteLine($"Speed {Speed}");
        }
    }
}
