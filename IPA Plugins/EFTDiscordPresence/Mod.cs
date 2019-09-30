using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IllusionPlugin;
using UnityEngine;
using UnityEngine.Networking;
using EFT;

namespace EFTDiscordPresence
{
    public class Mod : IPlugin {
        public string Name { get { return "Discord Rich Presence"; } }
        public string Version { get { return "1.0"; } }
        private bool GameInitialized = false;

        private const string discordAppId = "628201599025545226";

		private static DiscordRpc.RichPresence presence;
		private static DiscordRpc.EventHandlers eventHandlers;
		private static bool running = false;
		private static string uuid = "";
		private static string roomId = "";
		private static string roomSecret = "";
		private static int playersInRoom = 0;

        public Mod() {
            Logger.Log("Plugin Initialized");
            eventHandlers = default(DiscordRpc.EventHandlers);
			eventHandlers.errorCallback = delegate(int code, string message) {
				Logger.Error(string.Concat(new object[] {
					"[Discord] (E", code, ") ", message
				}));
			};
            InitRPC();
            presence.state = "Mod loaded"; DiscordRpc.UpdatePresence(ref presence);
        }

        private void InitRPC(bool update = false) {
            presence.state = "Not running";
			presence.details = "Not logged in";
			presence.largeImageKey = "logo2";
			presence.partySize = 0;
			presence.partyMax = 0;
			presence.startTimestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			presence.partyId = "";
			presence.largeImageText = "Escape from Tarkov";
			DiscordRpc.Initialize(discordAppId, ref eventHandlers, true, string.Empty);
            if (update) DiscordRpc.UpdatePresence(ref presence);
            running = true;
		    Logger.Log("[Discord] RichPresence Initialised");
        }

        public void OnApplicationStart() {
            Logger.Log("OnApplicationStart");
            if (!running) return;
            presence.state = "OnApplicationStart"; DiscordRpc.UpdatePresence(ref presence);
        }
        public void OnLevelWasLoaded(int level) {
            Logger.Log("OnLevelWasLoaded {0}", level);
            if (!running) return;
            presence.state = "OnLevelWasLoaded"; DiscordRpc.UpdatePresence(ref presence);
        }
        public void OnLevelWasInitialized(int level) {
            Logger.Log("OnLevelWasInitialized {0}", level);
            if (!GameInitialized) OnGameInitialized();
            if (!running) return;
            presence.state = "OnLevelWasInitialized"; DiscordRpc.UpdatePresence(ref presence);
        }

        private void OnGameInitialized() {
            GameInitialized = true;
            Logger.Log("OnGameInitialized");
            if (!running) return;
            presence.state = "OnGameInitialized"; DiscordRpc.UpdatePresence(ref presence);
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
            if (!running) return;
            presence.state = "OnApplicationQuit"; DiscordRpc.UpdatePresence(ref presence);
			DiscordRpc.Shutdown();
        }

    }
}

