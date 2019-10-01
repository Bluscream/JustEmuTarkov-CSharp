using Harmony;
using IllusionPlugin;
using System;
using System.Linq;

namespace JustEmuTarkov
{
    public class Mod : IPlugin
    {
        public string Name { get { return "JustEmuTarkov"; } }
        public string Version { get { return "1.0"; } }
        private bool GameInitialized = false;

        [Obsolete]
        public Mod()
        {
            Logger.Log("Plugin Initialized");
            PatchMethods();
            Patches.SSL.Disable();
        }

        public void OnApplicationStart()
        {
            Logger.Log("OnApplicationStart");
        }

        public void OnLevelWasInitialized(int level)
        {
            Logger.Log("OnLevelWasInitialized {0}", level);
            if (!GameInitialized) OnGameInitialized();
        }

        public void OnLevelWasLoaded(int level)
        {
            Logger.Log("OnLevelWasLoaded {0}", level);
        }

        private void OnGameInitialized()
        {
            GameInitialized = true;
            Logger.Log("OnGameInitialized");
        }

        private void PatchMethods()
        {
            var harmonyInstance = HarmonyInstance.Create("JustEmuTarkov");
#if DEBUG
            HarmonyInstance.DEBUG = true;
#endif
            Logger.Log("Patching Login");
            var success = Patches.Login.Patch(harmonyInstance);
            if (success) Logger.Log("Patched Login");
            Logger.Log("Patching BattlEye");
            success = Patches.BattlEye.Patch(harmonyInstance);
            if (success) Logger.Log("Patched BattlEye");
            Logger.Log("Patching ZoomFix");
            success = Patches.ZoomFix.Patch(harmonyInstance);
            if (success) Logger.Log("Patched ZoomFix");

#if DEBUG
            var patchedMethods = harmonyInstance.GetPatchedMethods().Select(p => p.DeclaringType?.Name + ":" + p.Name);
            Logger.Log("Patched Methods: {0}", string.Join(", ", patchedMethods.ToArray()));
#endif
        }

        public void OnUpdate()
        {
            // Logger.Log("OnUpdate");
        }

        public void OnFixedUpdate()
        {
            // Logger.Log("OnFixedUpdate");
        }

        public void OnApplicationQuit()
        {
            Logger.Log("OnApplicationQuit");
        }
    }
}