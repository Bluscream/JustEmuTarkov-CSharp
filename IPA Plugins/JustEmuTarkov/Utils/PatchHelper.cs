using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JustEmuTarkov.Utils
{
    class PatchHelper {
        internal enum PatchMethods { TargetMethod, Prepare, Prefix, Postfix, Transpiler }
        public static void PatchMethod(HarmonyInstance harmonyInstance, Type nearClass, string className, string methodName, Type patchClass) {
            var Class = nearClass.Assembly.GetType(className);
            var Method = Class.GetMethod(methodName, (BindingFlags)62);
            var replStr = $"{className}.{methodName} with {patchClass.Name}";

            try {
                Logger.Log("Patching {0}", replStr);
                harmonyInstance.Patch(Method, GetPatch(Class.GetType()), null, null);
                Logger.Log("Patched {0}", replStr);
            }
            catch (Exception ex) {
                Logger.Log("[ERROR] Unable to patch {0} {1}", replStr, ex.Message.Enclose());
            }
        }
        internal static HarmonyMethod GetPatch(Type type, PatchMethods patchMethod = PatchMethods.Prefix) => new HarmonyMethod(type.GetMethod(Enum.GetName(typeof(PatchMethods), patchMethod), BindingFlags.Static | BindingFlags.NonPublic));
    }
}