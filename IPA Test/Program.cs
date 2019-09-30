using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace IPA_Test
{
    class Program {
        static void Main(string[] args) {
            var files = new string[] {
               @"G:\Escape from Tarkov\EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll",
               // @"G:\Escape from Tarkov\EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll.JET",
            };
            foreach (var dll in files)
            {
                var file = new FileInfo(dll);
                Console.WriteLine("Loaded {0}", file.FullName.Quote());
                var _Module = ModuleDefinition.ReadModule(dll);
                TypeDefinition beClass = null;
                MethodDefinition beMethod = null;
                try {
                    foreach (var _class in _Module.GetTypes()) {
                        if (_class.IsPublic || !_class.IsSealed) continue;
                        if (!_class.HasProperties || _class.Properties.Count != 1) continue;
                        if (!_class.HasFields || _class.Fields.Count != 2) continue;
                        if (!_class.HasMethods) continue;
                        foreach (var method in _class.Methods) {
                            if (method.ReturnType.ToString() == "System.Collections.IEnumerator") {
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
                if (beClass is null) {
                    var cc = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("[WARNING]");
                    Console.WriteLine("BattlEye class or method was not found! If your game directly closes after launch you know why!");
                    Console.WriteLine("[WARNING]");
                    Console.ForegroundColor = cc;
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                } else {
                    var beName = string.Format("{0}::{1}", beClass.Name.ToUnicode(), beMethod.Name.ToUnicode());
                    Console.WriteLine("Found BattlEye as {0}", beName);
                    Console.ReadKey();
                    /*var inst = new Collection<Instruction>();
                    inst.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                    inst.Add(Instruction.Create(OpCodes.Newobj));
                    inst.Add(Instruction.Create(OpCodes.Dup, 0x0019AD9B));
                    inst.Add(Instruction.Create(OpCodes.Ldarg_0));
                    inst.Add(Instruction.Create(OpCodes.Stfld, 0x0019AD9D));
                    inst.Add(Instruction.Create(OpCodes.Ret));
                    beMethod.Body.Instructions.Clear();
                    foreach (var ins in inst) {
                        beMethod.Body.Instructions.Add(ins);
                    }*/

                    // https://www.mono-project.com/docs/tools+libraries/libraries/Mono.Cecil/faq/
                    var worker = beMethod.Body.GetILProcessor();

                    var sentence = string.Concat("Code added in ", beName);
                    var insertSentence = worker.Create(OpCodes.Ldstr, sentence);

                    Instruction ins = beMethod.Body.Instructions[0];
                    worker.InsertBefore(ins, insertSentence);
                    Console.WriteLine("Patched BattlEye!");
                    Console.ReadKey();
                    var sFN = file.SplitFileName();
                    var sF = file.Directory.CombineFile(sFN.Key + ".noBE" + sFN.Value);
                    Console.WriteLine("Saving as {0}!", sF.FullName.Quote());
                    _Module.Write(sF.FullName);
                }
            }
            Console.ReadKey();
        }
    }
    public static class Extenstions {
        public static bool ToInt(this byte[] bytes, out int result) {
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            try { result = BitConverter.ToInt32(bytes, 0); return true; } 
            catch (Exception ex) { result = int.MinValue; return false; }
        }
        public static bool ContainsUnicodeCharacter(this string input) {
            const int MaxAnsiCode = 255;
            return input.Any(c => c > MaxAnsiCode);
        }
        public static string ToUnicode(this string input) {
            if (input[0] < 255) return input;
            StringBuilder sb = new StringBuilder();
            foreach (char c in input) {
                if (c > 255) sb.Append("\\u" + string.Format("{0:x4}", (int)c));
                else sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
