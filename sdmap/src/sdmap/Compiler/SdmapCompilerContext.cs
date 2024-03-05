using sdmap.Functional;
using sdmap.Macros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace sdmap.Compiler
{
    public class SdmapCompilerContext
    {
        private readonly Assembly CallAssembly;
        private readonly string CallAssemblyName;

        public IDictionary<string, SqlEmiter> Emiters { get; }

        public Stack<string> NsStack { get; }

        public MacroManager MacroManager { get; } = new MacroManager();

        private SdmapCompilerContext(IDictionary<string, SqlEmiter> emiters, Stack<string> nsStacks)
        {
            Emiters = emiters;
            NsStack = nsStacks;
        }

        private SdmapCompilerContext(Assembly callAssembly, IDictionary<string, SqlEmiter> emiters, Stack<string> nsStacks)
        {
            CallAssembly=callAssembly;
            CallAssemblyName=callAssembly.GetName().Name;
            Emiters = emiters;
            NsStack = nsStacks;
        }

        public string CurrentNs => string.Join(".", NsStack.Reverse());

        public string GetFullNameInCurrentNs(string contextId)
        {

            return string.Join(".", NsStack.Reverse().Concat(new List<string> { contextId }));
        }

        public Result<SqlEmiter> TryGetEmiter(string contextId, string currentNs)
        {
            if (!string.IsNullOrWhiteSpace(currentNs)) contextId=$"{currentNs}.{contextId}";
            var fullName = contextId;
            if (!string.IsNullOrWhiteSpace(CallAssemblyName)) fullName=$"{CallAssemblyName}.{contextId}";
            if (Emiters.ContainsKey(fullName))
            {
                return Result.Ok(Emiters[fullName]);
            }
            return Result.Fail<SqlEmiter>($"Syntax '{contextId}' not found in current scope.");
        }

        public SqlEmiter GetEmiter(string contextId, string currentNs)
        {
            return TryGetEmiter(contextId, currentNs).Value;
        }

        public Result TryAdd(string contextId, SqlEmiter emiter)
        {

            contextId = GetFullNameInCurrentNs(contextId);
            string fullName = contextId;
            if (!string.IsNullOrWhiteSpace(CallAssemblyName)) fullName=$"{CallAssemblyName}.{contextId}";
            if (Emiters.ContainsKey(fullName))
            {
                return Result.Fail($"Syntax already defined: '{fullName}'.");
            }

            Emiters.Add(fullName, emiter);
            return Result.Ok();
        }

        public static SdmapCompilerContext CreateEmpty()
        {
            return CreateByContext(new Dictionary<string, SqlEmiter>());
        }

        public static SdmapCompilerContext CreateByContext(Dictionary<string, SqlEmiter> context)
        {
            return Create(context, new Stack<string>(new string[] { }));
        }

        public static SdmapCompilerContext Create(IDictionary<string, SqlEmiter> emiters, Stack<string> nsStack)
        {
            return new SdmapCompilerContext(emiters, nsStack);
        }

        public static SdmapCompilerContext Create(Assembly CallAssembly, IDictionary<string, SqlEmiter> emiters, Stack<string> nsStack)
        {
            return new SdmapCompilerContext(CallAssembly, emiters, nsStack);
        }
    }
}
