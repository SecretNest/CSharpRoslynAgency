using SecretNest.CSharpRoslynAgency;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetFxTest
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

        public Test()
        {
            builder.MissingAssemblyResolving += Builder_MissingAssemblyResolving;
            List<AssemblyName> allNames = new List<AssemblyName>(Assembly.GetExecutingAssembly().GetReferencedAssemblies());
            foreach (var name in allNames)
            {
                PrepareAssembly(name);
            }

            PrepareAssembly(new AssemblyName("Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"));

            //PrepareAssembly(typeof(Implant.Implant<>).Assembly.GetName());
        }

        void PrepareAssembly(AssemblyName name)
        {
            var lazy = new Lazy<byte[]>(() =>
            {
                Assembly test = Assembly.Load(name);
                var file = test.Location;
                var result = File.ReadAllBytes(file);
                Console.WriteLine("Loaded file: " + file);
                return result;
            }, false);
            platformAssembliesFullName.Add(name.FullName, lazy);
            platformAssembliesName.Add(name.Name, lazy);
        }

        public void Run()
        {
            List<AssemblyReference> references = new List<AssemblyReference>();
            AssemblyName assemblyNameCSharp = new AssemblyName("Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Assembly assemblyCSharp = Assembly.Load(assemblyNameCSharp);
            references.Add(new AssemblyReference(assemblyNameCSharp));
            references.Add(new AssemblyReference(typeof(Implant.Implant<>).Assembly.GetName()));
            references.Add(new AssemblyReference(new AssemblyName(platformAssembliesFullName.FirstOrDefault(i => i.Key.StartsWith("mscorlib,")).Key)));
            references.Add(new AssemblyReference(new AssemblyName(platformAssembliesFullName.FirstOrDefault(i => i.Key.StartsWith("System.Core,")).Key)));

            if (builder.Build("TestAssembly", new string[] { code }, references, out var assemblyImage, out var errors))
            {
                Assembly generated = null;
                generated = Assembly.Load(assemblyImage);

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
            Console.Write("Request from MissingAssemblyResolving: " + fullName);
            byte[] result;
            if (platformAssembliesFullName.TryGetValue(fullName, out var value))
            {
                result = value.Value;
                Console.WriteLine(" Matched with full name.");
            }
            else if (platformAssembliesName.TryGetValue(e.AssemblyName.Name, out value))
            {
                result = value.Value;
                Console.WriteLine(" Matched with name.");
            }
            else
            {
                result = null;
                Console.WriteLine(" Not resolved.");
            }
            e.MissingAssemblyImage = result;
        }
    }
}
