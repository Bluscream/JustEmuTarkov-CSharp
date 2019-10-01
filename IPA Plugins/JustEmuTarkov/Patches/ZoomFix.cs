using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using JustEmuTarkov.Utils;

namespace JustEmuTarkov.Patches
{
    internal class ZoomFix
    {
        public static bool Patch(HarmonyInstance harmonyInstance)
        {
            var patches = new List<PatchHelper.PatchClass>();
            var beCheck = new PatchHelper.PatchClass { Class = typeof(EFT.UI.PocketMapTile), PatchWithClass = typeof(ZoomFix) };
            var classMethods = beCheck.Class.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in classMethods)
            {
                var isPublicVirtual = method.IsPublic && method.IsVirtual;
                if (method.IsPrivate && !isPublicVirtual) continue;
                if (method.ReturnType.ToString() != "System.Void") continue;
                var p = method.GetParameters();
                if (p.Length != 0) continue;
                var b = method.GetMethodBody();
                if (b is null) continue;
                var v = b.LocalVariables;
                if (v.Count != 1) continue;
                if (v[0].LocalType.Name != "Texture") continue;
                beCheck.Method = method;
                break;
            }
            patches.Add(beCheck);
            return PatchHelper.PatchMethods(harmonyInstance, patches, Extensions.GetMethodInfo(() => ZoomFix.Prefix(null)));
        }

        public static bool Prefix(EFT.UI.PocketMapTile __instance)
        {
            // var texture = typeof(UnityEngine.Texture);
            var texture = (typeof(EFT.UI.PocketMapTile).GetProperties().First(p => p.Name == "_image").GetType().GetProperty("texture"));
            if (texture.GetValue(__instance, null) != null)
            {
                texture.SetValue(__instance, null, new object[] { });
                Logger.Debug("Zoom fix applied");
            }
            return false;
        }
    }
}