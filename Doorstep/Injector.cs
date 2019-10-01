using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Doorstop
{
    public class Injector
    {
        public static void Main(string[] args)
        {
            ExternalConsole.InitConsole();
            Logger.Debug("Start");
            Logger.Log("Loaded assemblies: {0}", string.Join(", ", AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name).ToArray()));
            // Thread t = new Thread(WaitForUnity); t.Start();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            Logger.Debug("End");
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var asmName = args.LoadedAssembly.GetName().Name;
            Logger.Log("Assembly {0} was loaded from {1}", asmName, args.LoadedAssembly.Location.Quote());
            if (asmName == "UnityEngine") OnUnityLoaded();
        }

        private static void OnUnityLoaded()
        {
            Logger.Log("Unity was loaded!");
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Logger.Log("Process exiting!");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Log("Unhandled Exception in Game! IsTerminating: {0}\n{1}", e.IsTerminating, e.ExceptionObject.ToJson());
        }

        private static void InjectStuff()
        {
            Assembly assembly = Assembly.LoadFile("UnhandledException.dll");
            AppDomain.CurrentDomain.Load(assembly.GetName());
            Logger.Warn("Injected file");
        }

        private static void WaitForUnity()
        {
            var loaded = false;
            while (!loaded)
            {
                Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly asm in asms)
                {
                    Logger.Log(asm.GetName().Name);
                    if (asm.GetName().Name == "UnityEngine") { loaded = true; break; }
                };
                Thread.Sleep(50);
            }
            OnUnityLoaded();
        }
    }
}