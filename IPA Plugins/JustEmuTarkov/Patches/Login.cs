using System.Collections.Generic;
using System.Reflection;
using Harmony;
using JustEmuTarkov.Utils;

namespace JustEmuTarkov.Patches
{
    internal class Login
    {
        public static bool Patch(HarmonyInstance harmonyInstance)
        {
            var patches = new List<PatchHelper.PatchClass>();
            var beCheck = new PatchHelper.PatchClass() { Class = typeof(EFT.MainApplication), PatchWithClass = typeof(Login) };
            var classMethods = beCheck.Class.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in classMethods)
            {
                var isPublicVirtual = method.IsPublic && method.IsVirtual;
                if (!method.IsPrivate && !isPublicVirtual) continue;
                if (method.ReturnType.ToString() != "System.Void") continue;
                var p = method.GetParameters();
                if (p.Length != 0) continue;
                var v = method.GetMethodBody().LocalVariables;
                if (v.Count != 4) continue;
                // if (v[0].LocalType.Name != "EFT.Login") continue;
                beCheck.Method = method;
                break;
            }
            patches.Add(beCheck);
            return PatchHelper.PatchMethods(harmonyInstance, patches, Extensions.GetMethodInfo(() => Login.Prefix()));
        }

        public static bool Prefix()
        {
            Logger.Debug("Bypassing login token check");
            return false;
        }
    }
}