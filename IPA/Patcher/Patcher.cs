﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IPA.Patcher
{
    internal class PatchedModule
    {
        private static readonly string[] ENTRY_TYPES = { "Input", "Display" };

        private FileInfo _File;
        private ModuleDefinition _Module;

        public static PatchedModule Load(string engineFile)
        {
            return new PatchedModule(engineFile);
        }

        private PatchedModule(string engineFile)
        {
            _File = new FileInfo(engineFile);

            LoadModules();
        }

        private void LoadModules()
        {
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(_File.DirectoryName);

            var parameters = new ReaderParameters
            {
                AssemblyResolver = resolver,
            };

            _Module = ModuleDefinition.ReadModule(_File.FullName, parameters);
        }

        public bool IsPatched {
            get {
                foreach (var @ref in _Module.AssemblyReferences)
                {
                    if (@ref.Name == "IllusionInjector") return true;
                }
                return false;
            }
        }

        public void Patch()
        {
            // First, let's add the reference
            var nameReference = new AssemblyNameReference("IllusionInjector", new Version(1, 0, 0, 0));
            var injectorPath = Path.Combine(_File.DirectoryName, "IllusionInjector.dll");
            var injector = ModuleDefinition.ReadModule(injectorPath);

            _Module.AssemblyReferences.Add(nameReference);
            int patched = 0;
            foreach (var type in FindEntryTypes())
            {
                if (PatchType(type, injector))
                {
                    patched++;
                }
            }

            if (patched > 0)
            {
                _Module.Write(_File.FullName);
            }
            else
            {
                throw new Exception("Could not find any entry type!");
            }
        }

        private bool PatchType(TypeDefinition targetType, ModuleDefinition injector)
        {
            var targetMethod = targetType.Methods.FirstOrDefault(m => m.IsConstructor && m.IsStatic);
            if (targetMethod != null)
            {
                var methodReference = _Module.Import(injector.GetType("IllusionInjector.Injector").Methods.First(m => m.Name == "Inject"));
                targetMethod.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, methodReference));
                return true;
            }
            return false;
        }

        private IEnumerable<TypeDefinition> FindEntryTypes()
        {
            return _Module.GetTypes().Where(m => ENTRY_TYPES.Contains(m.Name));
        }
    }
}