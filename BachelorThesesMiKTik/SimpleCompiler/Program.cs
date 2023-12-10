using RegexTest.CodeBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RegexTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var text = @"num value = 4 / 2
num value2 = 2.178448 + value * 3
bool boolean = true && false
bool boolean1 = 1 > 2 && 1 != 3

if value < value2
    value = -3
    if boolean
        boolean = true
    else
        value = 0
    fi
fi

num i = 2
num result = 1
while i <= 150
    result = result * i
    i = i + 1
end

num i = 2
while i <= 150
    if i % 2 == 0
        num soma1 = test.PrintTest (i)
    else
        num soma1 = test.PrintTest (i * 100)
    fi
    i = i + 1
end

num value8 = transform.mass.something + 1
transform.mass.something = 1 + 3
num soma1 = call (something,nothing + everything,call())";


            var text1 = @"

num value = 4 / 2
num value2 = 2.178448 + value * 3
bool boolean = value < value2 || true

num i = 2
while i <= 100
    if i % 4 == 0
        if i % 8 == 0
            test.PrintString (""Modulo  8"", i)
        else
            if i % 16 == 0
                test.PrintString (""Modulo  16"", i)
            fi
        fi
        test.PrintString (""Modulo   4"", i)
    elseif i % 3 == 0 
        test.PrintTest (-1 * i)
    else
        test.PrintTest (i * 100)
    fi
    i = i + 1
end

test.PrintTest ()
soma1 = test.PrintTest (value, value2)
soma1 = test.PrintTest (boolean)

test.Speed = 4 / 2
test.PrintTest (test.Speed)
";
            //TODO: Fix no space behind func name
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var match = Patterns.AssingRegex.Match(lines[0]);
            //if (match.Success) 
            //{
            //    Console.WriteLine("Hurray");
            //}
            var enviroment = new Dictionary<string, (object, Type)>();
            var test = new TestObject();
            enviroment.Add("test", (test, test.GetType()));

            var code = new MyCode(text1, enviroment);
            code.Execute();

            Console.ReadKey();
        }
    }
}
