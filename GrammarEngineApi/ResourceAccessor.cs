using System;
using System.IO;
using System.Reflection;

namespace GrammarEngineApi
{
    public class ResourceAccessor
    {
        private readonly Assembly _assembly;
        private readonly string _assemblyName;

        public ResourceAccessor()
        {
            _assembly = Assembly.GetExecutingAssembly();
            _assemblyName = _assembly.GetName().Name;
        }

        public byte[] Binary(string name)
        {
            using (var stream = new MemoryStream())
            {
                var resource = _assembly.GetManifestResourceStream(GetName(name));
                if (resource == null)
                {
                    throw new InvalidOperationException("Resource not available.");
                }
                
                resource.CopyTo(stream);

                return stream.ToArray();
            }
        }

        private string GetName(string name) =>
            name.StartsWith(_assemblyName) ? name : $"{_assemblyName}.{name}";
    }
}