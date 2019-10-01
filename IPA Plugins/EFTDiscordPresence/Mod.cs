using IllusionPlugin;
using UnityEngine;
using DiscordRPC;
using System.Linq;
using System.Runtime.InteropServices;
using System;

namespace EFTDiscordPresence
{
    public class Mod : MonoBehaviour, IPlugin {
        public string Name { get { return "Discord Rich Presence"; } }
        public string Version { get { return "1.0"; } }
        private bool GameInitialized = false;
        private bool debug = false;

        private DiscordRpcClient client;
        private static RichPresence presence = new RichPresence() {
		    Assets = new DiscordRPC.Assets() {
			    LargeImageKey = "logo2",
			    LargeImageText = Application.productName
		    },
            Details = "Example Project",
		    State = "csharp example",
	    };

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        private const string discordAppId = "628201599025545226";

        public Mod() {
            #if DEBUG
            debug = true;
            #endif
            Logger.Log("Plugin Initialized");
            // LoadLibrary("EscapeFromTarkov_Data/Plugins/discord-rpc.dll");
        }

        private void InitRPC() {
            client = new DiscordRpcClient(discordAppId, autoEvents: false);
            // client.AutoEvents = false;
            // client.Configuration = new Configuration() { ApiEndpoint = "", Enviroment = "Production" };
            // client.Logger = (DiscordRPC.Logging.ILogger)Debug.logger;
	        client.Logger = new DiscordRPC.Logging.ConsoleLogger() { Level = DiscordRPC.Logging.LogLevel.Trace };
	        client.OnReady += (sender, e) => {
		        Logger.Log("Received Ready from user {0}", e.User.Username);
	        };
	        client.OnPresenceUpdate += (sender, e) => {
		        Logger.Log("Received Update! {0}", e.Presence);
	        };
	        client.OnClose += (sender, e) => {
		        Logger.Log("OnClose {0} ({1})", e.Code, e.Reason);
	        };
	        client.OnError += (sender, e) => {
		        Logger.Log("OnError {0} ({1})", e.Code, e.Message);
	        };
	        client.OnConnectionEstablished += (sender, e) => {
		        Logger.Log("OnConnectionEstablished {0}: {1}", e.ConnectedPipe, e.Type);
	        };
	        client.OnConnectionFailed += (sender, e) => {
		        Logger.Log("OnConnectionFailed {0}: {1}", e.FailedPipe, e.Type);
	        };
	        client.OnRpcMessage += (sender, e) => {
		        Logger.Log("OnRpcMessage {0}", e.Type);
	        };
	        client.OnSubscribe += (sender, e) => {
		        Logger.Log("OnSubscribe {0} {1}", e.Event, e.Type);
	        };
	        client.OnUnsubscribe += (sender, e) => {
		        Logger.Log("OnUnsubscribe {0} {1}", e.Event, e.Type);
	        };
	        client.OnSpectate += (sender, e) => {
		        Logger.Log("OnSpectate {0} {1}", e.Secret, e.Type);
	        };
	        client.OnJoin += (sender, e) => {
		        Logger.Log("OnSpectate {0} {1}", e.Secret, e.Type);
	        };
	        client.OnJoinRequested += (sender, e) => {
		        Logger.Log("OnSpectate {0} {1}", e.User.Username, e.Type);
	        };
	        client.OnPresenceUpdate += (sender, e) => {
		        Logger.Log("OnSpectate {0} {1}", e.Name, e.ApplicationID, e.Type);
	        };
	        client.Initialize();
		    Logger.Log("[Discord] RichPresence Initialised");
        }

        public void OnApplicationStart() {
            InitRPC();
            Logger.Log("OnApplicationStart");
            var go = new GameObject("name").
            // StartCoroutine(Classes.DiscordManager);
            presence.State = "OnApplicationStart";
            client.SetPresence(presence);
        }
        public void OnLevelWasLoaded(int level) {
            Logger.Log("OnLevelWasLoaded {0}", level);
            presence.State = $"OnLevelWasLoaded {level}";
            client.SetPresence(presence);
        }
        public void OnLevelWasInitialized(int level) {
            if (!GameInitialized) OnGameInitialized();
            Logger.Log("OnLevelWasInitialized {0}", level);
            presence.State = $"OnLevelWasInitialized {level}";
            client.SetPresence(presence);
        }

        private void OnGameInitialized() {
            GameInitialized = true;
            Logger.Log("OnGameInitialized");
            client.UpdateState("OnGameInitialized");
            Logger.Log("Loaded assemblies: {0}", string.Join(debug ? "\n" : ", ", AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name).ToArray()));
		    Logger.Log($"[Discord] {client.SteamID}");
		    Logger.Log($"[Discord] {client.TargetPipe}");
		    Logger.Log($"[Discord] {client.ApplicationID}");
		    Logger.Log($"[Discord] {client.AutoEvents}");
		    if (client.Configuration != null) {
                Logger.Log($"[Discord] {client.Configuration.ApiEndpoint}");
		        Logger.Log($"[Discord] {client.Configuration.CdnHost}");
		        Logger.Log($"[Discord] {client.Configuration.Enviroment}");
            }
		    Logger.Log($"[Discord] {client.CurrentPresence.Details}");
		    Logger.Log($"[Discord] {client.CurrentPresence.State}");
		    Logger.Log($"[Discord] {client.HasRegisteredUriScheme}");
		    Logger.Log($"[Discord] {client.IsDisposed}");
		    Logger.Log($"[Discord] {client.IsInitialized}");
		    Logger.Log($"[Discord] {client.Logger.Level}");
		    Logger.Log($"[Discord] {client.MaxQueueSize}");
		    Logger.Log($"[Discord] {client.ProcessID}");
		    Logger.Log($"[Discord] {client.ShutdownOnly}");
		    Logger.Log($"[Discord] {client.SkipIdenticalPresence}");
		    Logger.Log($"[Discord] {client.Subscription}");
            client.Invoke();
                
        }

        public void OnUpdate() {
        }
        public void OnFixedUpdate() {
            client.Invoke();
        }
        public void OnApplicationQuit()
        {
            Logger.Log("OnApplicationQuit");
			client.Dispose();
        }

    }
}

