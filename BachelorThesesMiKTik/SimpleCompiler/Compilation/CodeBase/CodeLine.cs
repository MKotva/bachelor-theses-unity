using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexTest.CodeBase
{
    public interface CodeLine
    {
        int LineNumber { get; set; }
        void Execute();
    }
}
