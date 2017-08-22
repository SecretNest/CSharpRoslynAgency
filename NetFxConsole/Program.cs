using NetFxTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFxConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test();
            //test.AutoTest();
            test.Run();

            Console.ReadKey(true);
        }
    }
}
