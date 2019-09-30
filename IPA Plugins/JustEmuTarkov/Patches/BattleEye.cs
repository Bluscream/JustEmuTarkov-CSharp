using System;
using System.Collections;

namespace JustEmuTarkov.Patches
{
    public class BattleEye {
        public bool Succeed { get; set; }
        public bool Prefix(object  __instance, object battleEyeLogDelegate) {
            Logger.Log("Bypassing BattleEye login");
            Type beClass = typeof(Comfort.Common.Operation).Assembly.GetType("Class893");
            Array.Find(beClass.GetProperties(), x => x.Name.Equals("Succeed")).SetValue(__instance, true, new object[] { });
            return false;
        }
    }
}
