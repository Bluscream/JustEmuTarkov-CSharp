using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IPA.Patcher
{
    internal class VirtualizedModule
    {
        private const string ENTRY_TYPE = "Display";

        private FileInfo _File;
        private ModuleDefinition _Module;

        public static VirtualizedModule Load(string engineFile)
        {
            return new VirtualizedModule(engineFile);
        }

        private VirtualizedModule(string assemblyFile)
        {
            _File = new FileInfo(assemblyFile);

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="module"></param>
        public void Virtualize()
        {
            foreach (var type in _Module.Types)
            {
                VirtualizeType(type);
            }

            var success = PatchBattleye();

            _Module.Write(_File.FullName);
        }

        private void VirtualizeType(TypeDefinition type)
        {
            if (type.IsSealed)
            {
                // Unseal
                type.IsSealed = false;
            }

            if (type.IsInterface) return;
            if (type.IsAbstract) return;

            // These two don't seem to work.
            if (type.Name == "SceneControl" || type.Name == "ConfigUI") return;

            Console.WriteLine("Virtualizing {0}", type.Name);
            // Take care of sub types
            foreach (var subType in type.NestedTypes)
            {
                VirtualizeType(subType);
            }

            foreach (var method in type.Methods)
            {
                if (method.IsManaged
                    && method.IsIL
                    && !method.IsStatic
                    && !method.IsVirtual
                    && !method.IsAbstract
                    && !method.IsAddOn
                    && !method.IsConstructor
                    && !method.IsSpecialName
                    && !method.IsGenericInstance
                    && !method.HasOverrides)
                {
                    method.IsVirtual = true;
                    method.IsPublic = true;
                    method.IsPrivate = false;
                    method.IsNewSlot = true;
                    method.IsHideBySig = true;
                }
            }

            foreach (var field in type.Fields)
            {
                if (field.IsPrivate) field.IsFamily = true;
            }
        }

        private bool PatchBattleye()
        {
            var ret = false;
            TypeDefinition beClass = null;
            MethodDefinition beMethod = null;
            try
            {
                foreach (var _class in _Module.GetTypes())
                {
                    // if (_class.IsPublic || !_class.IsSealed) continue;
                    if (!_class.HasProperties || _class.Properties.Count != 1) continue;
                    if (!_class.HasFields || _class.Fields.Count != 2) continue;
                    if (!_class.HasMethods) continue;
                    foreach (var method in _class.Methods)
                    {
                        if (method.ReturnType.ToString() == "System.Collections.IEnumerator" &&
                            method.HasParameters && method.Parameters.Count == 1)
                        {
                            beMethod = method;
                            break;
                        }
                    }

                    if (beMethod is null) continue;
                    beClass = _class;
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (beClass is null)
            {
                var cc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("[WARNING]");
                Console.WriteLine(
                    "BattlEye class or method was not found! If your game directly closes after launch you know why!");
                Console.WriteLine("[WARNING]");
                Console.ForegroundColor = cc;
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            else
            {
                var beName = string.Format("{0}::{1} ({2}::{3})", beClass.Name, beMethod.Name, beClass.Name.ToUnicode(),
                    beMethod.Name.ToUnicode());
                Console.WriteLine("Found BattlEye check as {0}", beName);
                ;
                var inst = new Collection<Instruction>();
                // print call
                MethodInfo writeLineMethod = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) });
                var sentence = string.Concat("BattlEye check called. Bypassing...");
                inst.Add(Instruction.Create(OpCodes.Ldstr, sentence));
                var writeLine = _Module.Import(writeLineMethod);
                inst.Add(Instruction.Create(OpCodes.Call, writeLine));
                // set success
                inst.Add(Instruction.Create(OpCodes.Ldarg_0));
                inst.Add(Instruction.Create(OpCodes.Ldc_I4_1));
                var _com_mod = ModuleDefinition.ReadModule(Path.Combine(_File.DirectoryName, "Comfort.Unity.dll"));
                var coms = _com_mod.GetTypes();
                var abs_op = coms.First(t => t.Name == "AbstractOperation");
                var succ_type = _Module.Import(abs_op.Properties.First(p => p.Name == "Succeed").SetMethod);
                inst.Add(Instruction.Create(OpCodes.Callvirt, succ_type));
                // reutrn empty IEnumerator
                var obj_type = _Module.Import(typeof(object));
                var obj_type_get_enu = _Module.Import(typeof(Array).GetMethod("GetEnumerator"));
                inst.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                inst.Add(Instruction.Create(OpCodes.Newarr, obj_type));
                inst.Add(Instruction.Create(OpCodes.Call, obj_type_get_enu));
                inst.Add(Instruction.Create(OpCodes.Ret));
                beMethod.Body.Instructions.Clear();
                foreach (var _ins in inst)
                {
                    beMethod.Body.Instructions.Add(_ins);
                }

                Console.WriteLine("Patched BattlEye!");
                ret = true;
            }

            return ret;
        }

        public bool IsVirtualized {
            get {
                var awakeMethods = _Module.GetTypes().SelectMany(t => t.Methods.Where(m => m.Name == "Awake"));
                if (awakeMethods.Count() == 0) return false;

                return ((float)awakeMethods.Count(m => m.IsVirtual) / awakeMethods.Count()) > 0.5f;
            }
        }
    }

    public static class Extenstions
    {
        public static string ToUnicode(this string input, bool upper = false)
        {
            if (input[0] < 255) return input;
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                var char_str = string.Format("{0:x4}", (int)c);
                if (upper) char_str = char_str.ToUpperInvariant();
                if (c > 255) sb.Append("\\u" + char_str);
                else sb.Append(c);
            }
            return sb.ToString();
        }
    }
}