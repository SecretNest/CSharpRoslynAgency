using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NetCoreTest
{
    class AssemblyImage
    {
        public byte[] Image { get; }
        public AssemblyName Name { get; }

        public AssemblyImage(string fileName)
        {
            Name = System.Runtime.Loader.AssemblyLoadContext.GetAssemblyName(fileName);
            Image = File.ReadAllBytes(fileName);
            Console.Write("Loaded File: " + fileName);
        }
    }
}
