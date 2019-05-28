using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SecretNest.CSharpRoslynAgency
{
    /// <summary>
    /// Represents an assembly for building code using Roslyn.
    /// </summary>
    public class AssemblyReference
    {
        /// <summary>
        /// Gets the name of the assembly / module.
        /// </summary>
        public AssemblyName Name { get; }

        ///// <summary>
        ///// Gets the image of the assembly / module.
        ///// </summary>
        //public byte[] Image { get; }

        /// <summary>
        /// Gets whether this is a module.
        /// </summary>
        public bool IsModule { get; }

        /// <summary>
        /// Gets whether this assembly / module is EmbedInteropTypes.
        /// </summary>
        public bool EmbedInteropTypes { get; }

        /// <summary>
        /// Gets the aliases of the assembly / module.
        /// </summary>
        public IEnumerable<string> Aliases { get; }

        /// <summary>
        /// Initializes an instance of the AssemblyReference.
        /// </summary>
        /// <param name="name">Name of the assembly / module.</param>
        /// <param name="isModule">Whether this is a module.</param>
        /// <param name="embedInteropTypes">Whether this assembly / module is EmbedInteropTypes.</param>
        /// <param name="aliases">Aliases of the assembly / module.</param>
        public AssemblyReference(AssemblyName name, bool isModule = false, bool embedInteropTypes = false, params string[] aliases)
        {
            Name = name;
            //Image = image;
            IsModule = isModule;
            EmbedInteropTypes = embedInteropTypes;
            Aliases = aliases;
        }
    }
}
