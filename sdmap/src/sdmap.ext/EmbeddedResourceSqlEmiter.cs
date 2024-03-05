using sdmap.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace sdmap.ext
{
    public class EmbeddedResourceSqlEmiter : ISdmapEmiter
    {
        private SdmapCompiler _compiler = new SdmapCompiler();

        private readonly IDictionary<string, SdmapCompiler> _compilerDic = new Dictionary<string, SdmapCompiler>();

        public string Emit(string statementId, object parameters)
        {
            return _compiler.Emit(statementId, parameters);
        }
        public string Emit(string assemblyFullName, string statementId, object parameters)
        {
            _compilerDic.TryGetValue(assemblyFullName, out var result);
            return result?.Emit(statementId, parameters);
        }
        public static EmbeddedResourceSqlEmiter CreateFrom(Assembly assembly)
        {
            var emiter = new EmbeddedResourceSqlEmiter();

            foreach (var name in assembly.GetManifestResourceNames()
                .Where(x => x.EndsWith(".sdmap")))
            {
                using (var reader = new StreamReader(assembly.GetManifestResourceStream(name)))
                {
                    emiter._compiler.AddSourceCode(reader.ReadToEnd());
                }
            }

            var compiler = new SdmapCompiler(assembly);
            emiter._compilerDic[assembly.FullName]= compiler;
            foreach (var name in assembly.GetManifestResourceNames()
                    .Where(x => x.EndsWith(".sdmap")))
            {
                using (var reader = new StreamReader(assembly.GetManifestResourceStream(name)))
                {
                    compiler.AddSourceCode(reader.ReadToEnd());
                }
            }
            return emiter;
        }
    }

    public class MultipleAssemblyEmbeddedResourceSqlEmiter : ISdmapEmiter
    {
        private SdmapCompiler _compiler = new SdmapCompiler();
        private readonly IDictionary<string, SdmapCompiler> _compilerDic = new Dictionary<string, SdmapCompiler>();


        public string Emit(string statementId, object parameters)
        {
            return _compiler.Emit(statementId, parameters);
        }

        public string Emit(string assemblyFullName, string statementId, object parameters)
        {
            _compilerDic.TryGetValue(assemblyFullName, out var result);
            return result?.Emit(statementId, parameters);
        }
        public void AddAssembly(Assembly assembly)
        {
            foreach (var name in assembly.GetManifestResourceNames()
                    .Where(x => x.EndsWith(".sdmap")))
            {
                using (var reader = new StreamReader(assembly.GetManifestResourceStream(name)))
                {
                    _compiler.AddSourceCode(reader.ReadToEnd());
                }
            }
            var compiler = new SdmapCompiler(assembly);
            _compilerDic[assembly.FullName]= compiler;
            foreach (var name in assembly.GetManifestResourceNames()
                    .Where(x => x.EndsWith(".sdmap")))
            {
                using (var reader = new StreamReader(assembly.GetManifestResourceStream(name)))
                {
                    compiler.AddSourceCode(reader.ReadToEnd());
                }
            }
        }

        public static MultipleAssemblyEmbeddedResourceSqlEmiter CreateFrom(params Assembly[] assemblies)
        {
            var emiter = new MultipleAssemblyEmbeddedResourceSqlEmiter();

            foreach (var assembly in assemblies)
            {
                emiter.AddAssembly(assembly);
            }

            return emiter;
        }
    }
}
