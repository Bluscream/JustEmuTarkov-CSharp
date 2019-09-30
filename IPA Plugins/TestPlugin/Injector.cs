using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TestPlugin
{
    class Injector
    {
        public static void InjectStuff()
        {
            Assembly assembly = Assembly.LoadFrom("UnhandledException.dll");
            Logger.Warn("Loaded {0}", assembly.FullName);
            // AppDomain.CurrentDomain.Load(assembly.GetName());
            // assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == "UnhandledExceptionHandler").First();
            var Class = assembly.GetType("UnhandledExceptionHandler.ErrorLog");
            Logger.Trace("Class: {0}", Class.FullName);
            var method = Class.GetMethod("Start");
            Logger.Trace("Method: {0}", method.Name);
            var instance = Activator.CreateInstance(Class);
            Logger.Trace("Instance: {0}", instance);
            method.Invoke(instance, null);
            Logger.Warn("Executed {0}", assembly.FullName);
        }
    }
}
