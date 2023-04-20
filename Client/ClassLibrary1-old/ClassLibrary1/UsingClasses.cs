using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public static class UsingClasses
    {
        private static void main()
        {
            int x = 3;
            int y = 4;

            int a => { return x + y; } ;
        }
    }
}
