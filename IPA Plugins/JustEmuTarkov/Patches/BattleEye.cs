using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Reflection;
using Harmony;
using JustEmuTarkov.Utils;

namespace JustEmuTarkov.Patches
{
    internal class BattlEye
    {
        public static bool Patch(HarmonyInstance harmonyInstance)
        {
            var patches = new List<PatchHelper.PatchClass>();
            var asm = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Assembly-CSharp");
            var beCheck = new PatchHelper.PatchClass() { PatchWithClass = typeof(BattlEye) };
            foreach (var _class in asm.GetTypes())
            {
                /*if (_class.Name == "Class893")
                {
                    Logger.Trace("properties " + _class.GetProperties().Length);
                    Logger.Trace("fields " + _class.GetFields().Length);
                    Logger.Trace("methods " + _class.GetMethods().Length);
                    Logger.Trace("properties " + _class.GetProperties().Length);
                    Logger.Trace("{0}", _class.ToJson());
                }*/
                if (!IsObfuscatedClass(_class) && !IsDeobfuscatedClass(_class)) continue;
                foreach (var method in _class.GetMethods())
                {
                    if (!IsMethod(method)) continue;
                    beCheck.Method = method;
                    break;
                }
                if (beCheck.Method is null) continue;
                beCheck.Class = _class;
                break;
            }
            patches.Add(beCheck);
            return PatchHelper.PatchMethods(harmonyInstance, patches, Extensions.GetMethodInfo(() => BattlEye.Prefix("", true)));
        }

        private static bool IsMethod(MethodInfo method)
        {
            if (method.ReturnType.ToString() != "System.Collections.IEnumerator") return false;
            var p = method.GetParameters();
            if (p.Length != 1) return false;
            if (p.Length != 1 || p[0].ParameterType.FullName != "BattlEye.BEClient+LogDelegate") return false;
            return true;
        }

        private static bool IsDeobfuscatedClass(Type _class)
        {
            if (_class.GetProperties().Length != 4) return false;
            if (_class.GetFields().Length != 0) return false;
            if (_class.GetMethods().Length != 16) return false;
            return true;
        }

        private static bool IsObfuscatedClass(Type _class)
        {
            var classMethods = _class.GetMethods();
            if (classMethods.Length > 1 || classMethods.Length < 30) return true;
            return false;
        }

        public static bool Prefix(object __instance, object battleEyeLogDelegate)
        {
            Logger.Log("Bypassing BattleEye check");
            typeof(Comfort.Common.Operation).Assembly.GetTypes()
                .First(t => t.Name == "AbstractOperation").GetProperties().First(p => p.Name == "Succeed")
                .SetValue(__instance, true, new object[] { });
            return false;
        }
    }
}