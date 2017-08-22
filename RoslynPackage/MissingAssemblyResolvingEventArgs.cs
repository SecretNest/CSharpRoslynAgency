using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SecretNest.CSharpRoslynAgency
{
    /// <summary>
    /// Represents an argument of the MissingAssemblyResolving.
    /// </summary>
    public class MissingAssemblyResolvingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the path or name of the assembly / module which reference this missing assembly
        /// </summary>
        public string Display { get; }


        /// <summary>
        /// Gets whether the assembly / module which reference this missing assembly is a module.
        /// </summary>
        public bool IsModule { get; }

        /// <summary>
        /// Gets whether the assembly / module which reference this missing assembly is EmbedInteropTypes.
        /// </summary>
        public bool EmbedInteropTypes { get; }

        /// <summary>
        /// Gets the aliases of the assembly / module which reference this missing assembly.
        /// </summary>
        public IEnumerable<string> Aliases { get; }

        /// <summary>
        /// Gets or sets the image of the missing assembly / module.
        /// </summary>
        public byte[] MissingAssemblyImage { get; set; }

        /// <summary>
        /// Gets the name of the assembly / module which is missing.
        /// </summary>
        public AssemblyName AssemblyName { get; }

        /// <summary>
        /// Initializes an instance of the MissingAssemblyResolvingEventArgs.
        /// </summary>
        /// <param name="display">Path or name of the assembly / module which reference this missing assembly</param>
        /// <param name="reference">Instance of AssemblyReference referring to the missing assembly.</param>
        public MissingAssemblyResolvingEventArgs(string display, AssemblyReference reference)
        {
            Display = display;
            AssemblyName = reference.Name;
            IsModule = reference.IsModule;
            EmbedInteropTypes = reference.EmbedInteropTypes;
            Aliases = reference.Aliases;
        }
    }
}