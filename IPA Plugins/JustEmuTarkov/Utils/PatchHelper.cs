using Harmony;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace JustEmuTarkov.Utils
{
    internal class PatchHelper
    {
        internal enum PatchMethodsEnum { TargetMethod, Prepare, Prefix, Postfix, Transpiler }

        public static void PatchMethods(HarmonyInstance harmonyInstance, List<PatchClass> patches)
        {
            foreach (var patch in patches)
            {
                PatchMethod(harmonyInstance, patch.Class, patch.Method, patch.PatchWithClass);
            }
        }

        public static void PatchMethod(HarmonyInstance harmonyInstance, Type Class, MethodInfo method, Type patchClass)
        {
            if (patchClass is null) patchClass = new StackTrace().GetFrame(1).GetMethod().ReflectedType;
            var replStr = $"{Class.Name}::{method.Name} with {patchClass.FullName}";
            try
            {
                Logger.Log("Preparing patch for {0}", replStr);
                var harmonyMethod = GetPatch(Class.GetType());
                Logger.Log("Patching {0}", replStr);
                harmonyInstance.Patch(method, harmonyMethod, null, null);
                Logger.Log("Patched {0}", replStr);
            }
            catch (Exception ex)
            {
                Logger.Log("[ERROR] Unable to patch {0} {1}", replStr, ex.Message.Enclose());
            }
        }

        private static HarmonyMethod GetPatch(Type type, PatchMethodsEnum patchMethod = PatchMethodsEnum.Prefix) => new HarmonyMethod(type.GetMethod(Enum.GetName(typeof(PatchMethodsEnum), patchMethod)));

        public class PatchClass
        {
            public Type Class { get; set; }
            public MethodInfo Method { get; set; }
            public Type PatchWithClass { get; set; }

            public PatchClass()
            {
            }

            public PatchClass(Type _class, MethodInfo method, Type patchWithClass = null)
            {
                Class = _class; Method = method; PatchWithClass = patchWithClass;
            }
        }
    }
}