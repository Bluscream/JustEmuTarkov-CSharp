using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using JustEmuTarkov.Utils;

namespace JustEmuTarkov.Patches
{
    public class BattleEye
    {
        public static void Patch(HarmonyInstance harmonyInstance)
        {
            var patches = new List<PatchHelper.PatchClass>();
            var asm = Assembly.GetExecutingAssembly();
            var beCheck = new PatchHelper.PatchClass() { PatchWithClass = typeof(BattleEye) };
            foreach (var _class in asm.GetTypes())
            {
                if (_class.GetProperties().Length != 1) continue;
                if (_class.GetFields().Length != 2) continue;
                var classMethods = _class.GetMethods();
                if (classMethods.Length < 1) continue;
                foreach (var method in classMethods)
                {
                    if (method.ReturnType.ToString() == "System.Collections.IEnumerator" &&
                        method.GetParameters().Length == 1)
                    {
                        beCheck.Method = method;
                        break;
                    }
                }
                if (beCheck.Method is null) continue;
                beCheck.Class = _class;
                break;
            }
            patches.Add(beCheck);
            PatchHelper.PatchMethods(harmonyInstance, patches);
        }

        // public bool Succeed { get; set; }

        public bool Prefix(object __instance, object battleEyeLogDelegate)
        {
            Logger.Log("Bypassing BattleEye check");
            typeof(Comfort.Common.Operation).Assembly.GetTypes()
                .First(t => t.Name == "AbstractOperation").GetProperties().First(p => p.Name == "Succeed")
                .SetValue(__instance, true, new object[] { });
            return false;
        }
    }
}