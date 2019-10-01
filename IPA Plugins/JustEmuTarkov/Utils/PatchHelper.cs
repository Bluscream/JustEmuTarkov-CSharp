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

        public static bool PatchMethods(HarmonyInstance harmonyInstance, List<PatchClass> patches, MethodInfo patcherPrefix = null)
        {
            var success = new List<bool>();
            foreach (var patch in patches)
            {
                success.Add(PatchMethod(harmonyInstance, patch.Class, patch.Method, patch.PatchWithClass, patcherPrefix));
            }

            return !success.Contains(false);
        }

        public static bool PatchMethod(HarmonyInstance harmonyInstance, Type Class, MethodInfo method, Type patchClass = null, MethodInfo patcherPrefix = null)
        {
            if (patchClass is null) patchClass = new StackTrace().GetFrame(1).GetMethod().ReflectedType;
            if (Class is null || method is null)
            {
                Logger.Error("Patch for {0} was not found!", patchClass);
                return false;
            }
            var replStr = $"{Class.Name}::{method.Name} with {patchClass.FullName}";
            try
            {
                Logger.Debug("Preparing patch for {0}", replStr);
                HarmonyMethod harmonyMethod;
                if (patcherPrefix is null) harmonyMethod = GetPatch(Class);
                else harmonyMethod = new HarmonyMethod(patcherPrefix);
                Logger.Debug("Patching {0}", replStr);
                harmonyInstance.Patch(method, harmonyMethod);
                Logger.Debug("Patched {0}", replStr);
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to patch {0} {1}", replStr, ex.Message.Enclose());
                return false;
            }
            return true;
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