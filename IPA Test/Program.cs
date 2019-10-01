using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace IPA_Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cmd = new Process {
                StartInfo = {
                    FileName = "cmd",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };
            cmd.Start();
            cmd.StandardInput.WriteLine("chcp 65001");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            Console.OutputEncoding = Encoding.UTF8;
            var files = new[] {
               // @"G:\Escape from Tarkov\EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll",
               @"G:\Escape from Tarkov\EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll.ORG",
            };
            foreach (var dll in files)
            {
                var file = new FileInfo(dll);
                Console.WriteLine("Loaded {0}", file.FullName.Quote());
                var _Module = ModuleDefinition.ReadModule(dll);
                TypeDefinition beClass = null;
                MethodDefinition beMethod = null;
                try
                {
                    foreach (var _class in _Module.GetTypes())
                    {
                        if (_class.IsPublic || !_class.IsSealed) continue;
                        if (!_class.HasProperties || _class.Properties.Count != 1) continue;
                        if (!_class.HasFields || _class.Fields.Count != 2) continue;
                        if (!_class.HasMethods) continue;
                        foreach (var method in _class.Methods)
                        {
                            if (method.ReturnType.ToString() == "System.Collections.IEnumerator")
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
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                if (beClass is null)
                {
                    var cc = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("[WARNING]");
                    Console.WriteLine("BattlEye class or method was not found! If your game directly closes after launch you know why!");
                    Console.WriteLine("[WARNING]");
                    Console.ForegroundColor = cc;
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                else
                {
                    var beName = string.Format("{0}::{1} ({2}::{3})", beClass.Name, beMethod.Name, beClass.Name.ToUnicode(), beMethod.Name.ToUnicode());
                    Console.WriteLine("Found BattlEye as {0}", beName);
                    // Console.ReadKey();
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
                    var _com_mod = ModuleDefinition.ReadModule(@"G:\Escape from Tarkov\EscapeFromTarkov_Data\Managed\Comfort.Unity.dll");
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
                    // Console.ReadKey();
                    var sFN = file.SplitFileName();
                    var sF = file.Directory.CombineFile(sFN.Key + ".noBE" + sFN.Value);
                    Console.WriteLine("Saving as {0}!", sF.FullName.Quote());
                    _Module.Write(sF.FullName);
                }
            }
            Console.ReadKey();
        }
    }

    public static class Extenstions
    {
        public static bool ToInt(this byte[] bytes, out int result)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            try { result = BitConverter.ToInt32(bytes, 0); return true; }
            catch (Exception ex) { result = int.MinValue; return false; }
        }

        public static bool ContainsUnicodeCharacter(this string input)
        {
            const int MaxAnsiCode = 255;
            return input.Any(c => c > MaxAnsiCode);
        }

        public static string ToUnicode(this string input)
        {
            if (input[0] < 255) return input;
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if (c > 255) sb.Append("\\u" + string.Format("{0:x4}", (int)c));
                else sb.Append(c);
            }
            return sb.ToString();
        }
    }
}