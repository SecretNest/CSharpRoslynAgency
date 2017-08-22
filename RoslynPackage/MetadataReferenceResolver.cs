using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace SecretNest.CSharpRoslynAgency
{
    class MetadataReferenceResolver : Microsoft.CodeAnalysis.MetadataReferenceResolver
    {
        public override bool ResolveMissingAssemblies => true;

        public override bool Equals(object other)
        {
            return other.Equals(this);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public GetMissingAssemblyCallback GetMissingAssemblyCallback { get; set; }

        public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string baseFilePath, MetadataReferenceProperties properties)
        {
            return ImmutableArray<PortableExecutableReference>.Empty;
        }

        public override PortableExecutableReference ResolveMissingAssembly(MetadataReference definition, AssemblyIdentity referenceIdentity)
        {
            AssemblyName name = new AssemblyName(referenceIdentity.Name)
            {
                ContentType = referenceIdentity.ContentType,
                CultureName = referenceIdentity.CultureName,
                Flags = referenceIdentity.Flags,
                Version = referenceIdentity.Version
            };
            if (referenceIdentity.IsStrongName)
            {
                if (!referenceIdentity.PublicKey.IsDefaultOrEmpty)
                {
                    byte[] key = new byte[referenceIdentity.PublicKey.Length];
                    referenceIdentity.PublicKey.CopyTo(key);
                    name.SetPublicKey(key);
                }
                if (!referenceIdentity.PublicKeyToken.IsDefaultOrEmpty)
                {
                    byte[] key = new byte[referenceIdentity.PublicKeyToken.Length];
                    referenceIdentity.PublicKeyToken.CopyTo(key);
                    name.SetPublicKeyToken(key);
                }
            }

            string[] aliases;
            if (definition.Properties.Aliases == null || definition.Properties.Aliases.IsDefaultOrEmpty)
            {
                aliases = null;
            }
            else
            {
                aliases = new string[definition.Properties.Aliases.Length];
                definition.Properties.Aliases.CopyTo(aliases);
            }

            return GetMissingAssemblyCallback(definition.Display,
                new AssemblyReference(name, definition.Properties.Kind == MetadataImageKind.Module, definition.Properties.EmbedInteropTypes, aliases));
        }

    }

    delegate PortableExecutableReference GetMissingAssemblyCallback(string display, AssemblyReference reference);
}
