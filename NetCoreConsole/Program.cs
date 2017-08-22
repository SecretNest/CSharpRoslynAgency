using NetCoreTest;
using System;

namespace NetCoreConsole
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