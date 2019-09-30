using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IllusionPlugin;
using UnityEngine;
using UnityEngine.Networking;
using EFT;
using Harmony;

namespace JustEmuTarkov
{
    public class Mod : IPlugin {
        public string Name { get { return "JustEmuTarkov"; } }
        public string Version { get { return "1.0"; } }
        private bool GameInitialized = false;

        [Obsolete]
        public Mod() {
            Logger.Log("Plugin Initialized");
            PatchMethods();
            Patches.SSL.Disable();
        }

        public void OnApplicationStart() {
            Logger.Log("OnApplicationStart");
        }
        public void OnLevelWasInitialized(int level) {
            Logger.Log("OnLevelWasInitialized {0}", level);
            if (!GameInitialized) OnGameInitialized();
        }
        public void OnLevelWasLoaded(int level)
        {
            Logger.Log("OnLevelWasLoaded {0}", level);
        }

        private void OnGameInitialized() {
            GameInitialized = true;
            Logger.Log("OnGameInitialized");
        }

        private void PatchMethods()
        {
            var harmonyInstance = HarmonyInstance.Create("JustEmuTarkov");
#if DEBUG
            HarmonyInstance.DEBUG = true;
#endif
            Logger.Log("Patching BattlEye");
            Utils.PatchHelper.PatchMethod(harmonyInstance, typeof(EFT.MainApplication), "Class893", "method_0", typeof(Patches.BattleEye));
            Logger.Log("Patched BattlEye");
            // Utils.PatchHelper.PatchMethod(harmonyInstance, typeof(EFT.MainApplication), "Class1173", "method_0", typeof(Patches.LocalGame));

#if DEBUG
            var patchedMethods = harmonyInstance.GetPatchedMethods().Select(p => p.DeclaringType.Name + ":" + p.Name);
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
