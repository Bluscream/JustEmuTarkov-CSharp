using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EFTDiscordPresence.Classes
{
   public class DiscordManager : MonoBehaviour
    {
        private static GameObject self;
        public DiscordManager() {
            Logger.Log("DiscordManager()");
        }
        public void Awake() {
            Logger.Log("Awake()");
            DontDestroyOnLoad(this);
        }

        public void Update() {
        }

        public void OnGUI() {
            Logger.Trace("OnGUI");
        }
    }
}
