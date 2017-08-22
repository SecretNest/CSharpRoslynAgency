using System;
using System.Collections.Generic;
using System.Reflection;

namespace Implant
{
    public class Implant<TClass>
    {
        public void Print<TArg>(List<TArg> list)
        {
            //Display types and assemblies.
            Console.WriteLine("TClass: {0} ({1})\nTArg: {2} ({3})", typeof(TClass).Name, typeof(TClass).GetTypeInfo().Assembly.CodeBase, typeof(TArg).Name, typeof(TArg).GetTypeInfo().Assembly.CodeBase);
        }

        public void PrintFile()
        {
            //Test File IO
            Console.WriteLine(@"Printing C:\Windows\win.ini");
            Console.WriteLine(System.IO.File.ReadAllText(@"C:\Windows\win.ini"));
        }
    }
}
