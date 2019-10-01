using IllusionPlugin;

namespace TestPlugin
{
    public class Main : IPlugin
    {
        public string Name { get { return "Test Plugin"; } }
        public string Version { get { return "1.0"; } }
        private static bool GameInitialized = false;

        public Main()
        {
            Logger.Log("Plugin Initialized");
        }

        public void OnApplicationStart()
        {
            // Logger.Init();
            ExternalConsole.InitConsole();
            Logger.Log("OnApplicationStart");
        }

        public void OnLevelWasInitialized(int level)
        {
            Logger.Log("OnLevelWasInitialized {0}", level);
        }

        public void OnLevelWasLoaded(int level)
        {
            if (!GameInitialized) OnGameInitialized();
            Logger.Log("OnLevelWasLoaded {0}", level);
        }

        private void OnGameInitialized()
        {
            GameInitialized = true;
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