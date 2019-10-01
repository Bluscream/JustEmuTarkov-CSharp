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
        public static bool Patch(HarmonyInstance harmonyInstance)
        {
            var patches = new List<PatchHelper.PatchClass>();
            var asm = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Assembly-CSharp");
            var beCheck = new PatchHelper.PatchClass() { PatchWithClass = typeof(BattleEye) };
            foreach (var _class in asm.GetTypes())
            {
                /* if (_class.Name == "Class893")
                {
                    Logger.Trace("properties " + _class.GetProperties().Length);
                    Logger.Trace("fields " + _class.GetFields().Length);
                    Logger.Trace("methods " + _class.GetMethods().Length);
                    Logger.Trace("properties " + _class.GetProperties().Length);
                } */
                if (_class.GetProperties().Length != 5) continue;
                if (_class.GetFields().Length != 0) continue;
                var classMethods = _class.GetMethods();
                if (classMethods.Length != 17) continue;
                foreach (var method in classMethods)
                {
                    if (method.ReturnType.ToString() != "System.Collections.IEnumerator") continue;
                    var p = method.GetParameters();
                    if (p.Length != 1) continue;
                    if (p.Length != 1 || p[0].ParameterType.FullName != "BattlEye.BEClient+LogDelegate") continue;
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
            return PatchHelper.PatchMethods(harmonyInstance, patches);
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