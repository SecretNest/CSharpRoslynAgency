using SecretNest.CSharpRoslynAgency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetCoreTest
{
    public class Test
    {
        string code = @"
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UserCode
{
    public class UserCodeClass
    {
        public bool Check(string message)
        {
            List<string> x = new List<string>();
            GenericMethod((dynamic)x);
            //Console.WriteLine(message);
            return true;
        }

        public void GenericMethod<T>(List<T> data)
        {
            var implant = new Implant.Implant<object>();
            implant.Print(data);
            implant.PrintFile();
        }
    }
}";

        Dictionary<string, Lazy<byte[]>> platformAssembliesFullName = new Dictionary<string, Lazy<byte[]>>();
        Dictionary<string, Lazy<byte[]>> platformAssembliesName = new Dictionary<string, Lazy<byte[]>>();
        Builder builder = new Builder();
        long EightBytesToLong(byte[] data)
        {
            return BitConverter.ToInt64(data, 0);
        }
        string[] locationsOfAllReferences = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES").ToString().Split(';');

        public Test()
        {
            builder.MissingAssemblyResolving += Builder_MissingAssemblyResolving;
            foreach (var file in locationsOfAllReferences)
            {
                var name = System.Runtime.Loader.AssemblyLoadContext.GetAssemblyName(file);
                var lazy = new Lazy<byte[]>(() =>
                {
                    var result = File.ReadAllBytes(file);
                    Console.WriteLine("Loaded file: " + file);
                    return result;
                }, false);
                platformAssembliesFullName.Add(name.FullName, lazy);
                platformAssembliesName.Add(name.Name, lazy);
            }
        }

        //public void AutoTest()
        //{
        //    var names = platformAssembliesFullName.Keys.ToArray();
        //    bool[] skipped = new bool[names.Length];

        //    for (int i = 0; i < skipped.Length; i++)
        //    {
        //        skipped[i] = true;
        //        if (!TestOne(skipped, names))
        //        {
        //            skipped[i] = false;
        //        }
        //    }

        //    Console.WriteLine();
        //    Console.WriteLine();
        //    Console.WriteLine("Required:");
        //    for (int i = 0; i < skipped.Length; i++)
        //    {
        //        if (!skipped[i])
        //            Console.WriteLine(names[i]);
        //    }
        //    Console.Read();
        //}

        //public bool TestOne(bool[] skipped, string[] names)
        //{
        //    List<AssemblyReference> references = new List<AssemblyReference>();
        //    for (int i = 0; i < skipped.Length; i++)
        //    {
        //        if (!skipped[i]) references.Add(new AssemblyReference(new AssemblyName(names[i])));
        //    }

        //    if (builder.Build("TestAssembly" + Guid.NewGuid().ToString("N"), new string[] { code }, references, out var assemblyImage, out var errors))
        //    {
        //        Assembly generated = null;
        //        using (MemoryStream stream = new MemoryStream(assemblyImage))
        //        {
        //            stream.Seek(0, SeekOrigin.Begin);
        //            generated = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(stream);
        //        }

        //        Type type = generated.GetType("UserCode.UserCodeClass");

        //        object obj = Activator.CreateInstance(type);
        //        MethodInfo method = type.GetTypeInfo().GetDeclaredMethod("Check");
        //        var check = (bool)method.Invoke(obj, new object[] { "pass" });
        //        if (check != true)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public void Run()
        {
            List<AssemblyReference> references = new List<AssemblyReference>();
            references.Add(new AssemblyReference(new AssemblyName(platformAssembliesFullName.FirstOrDefault(i => i.Key.StartsWith("NetCoreImplant,")).Key)));
            references.Add(new AssemblyReference(new AssemblyName(platformAssembliesFullName.FirstOrDefault(i => i.Key.StartsWith("Microsoft.CSharp,")).Key)));
            references.Add(new AssemblyReference(new AssemblyName(platformAssembliesFullName.FirstOrDefault(i => i.Key.StartsWith("System.Collections,")).Key)));
            references.Add(new AssemblyReference(new AssemblyName(platformAssembliesFullName.FirstOrDefault(i => i.Key.StartsWith("System.Linq.Expressions,")).Key)));
            references.Add(new AssemblyReference(new AssemblyName(platformAssembliesFullName.FirstOrDefault(i => i.Key.StartsWith("System.Runtime,")).Key)));
            references.Add(new AssemblyReference(new AssemblyName(platformAssembliesFullName.FirstOrDefault(i => i.Key.StartsWith("System.Private.CoreLib,")).Key)));

            if (builder.Build("TestAssembly", new string[] { code }, references, out var assemblyImage, out var errors))
            {
                Assembly generated = null;
                using (MemoryStream stream = new MemoryStream(assemblyImage))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    generated = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(stream);
                }

                Type type = generated.GetType("UserCode.UserCodeClass");

                object obj = Activator.CreateInstance(type);
                MethodInfo method = type.GetTypeInfo().GetDeclaredMethod("Check");
                Console.WriteLine("Run created assembly.");
                var check = (bool)method.Invoke(obj, new object[] { "pass" });
                if (check != true)
                {
                    Console.WriteLine("Return not valid.");
                }
                Console.WriteLine("Done. Press any key to quit...");
            }
            else
            {
                foreach (var item in errors)
                {
                    Console.WriteLine("Id: {0}; Message: {1}\nLocation: {2}", item.Id, item.Message, item.Location);
                }
            }
        }

        private void Builder_MissingAssemblyResolving(object sender, MissingAssemblyResolvingEventArgs e)
        {
            var fullName = e.AssemblyName.FullName;
            Console.WriteLine("Request from MissingAssemblyResolving: " + fullName);
            byte[] result;
            if (platformAssembliesFullName.TryGetValue(fullName, out var value))
            {
                result = value.Value;
                Console.WriteLine("Matched with full name: " + fullName);
            }
            else if (platformAssembliesName.TryGetValue(e.AssemblyName.Name, out value))
            {
                result = value.Value;
                Console.WriteLine("Matched with name: " + fullName);
            }
            else
            {
                result = null;
                Console.WriteLine("Not resolved: " + fullName);
            }
            e.MissingAssemblyImage = result;
        }
    }
}
