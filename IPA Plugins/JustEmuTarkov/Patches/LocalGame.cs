namespace JustEmuTarkov.Patches
{
    internal class LocalGame
    {/*
			public void Prefix(object __instance) {
            // Array.Find(beClass.GetProperties(), x => x.Name.Equals("localGame_0")).SetValue(__instance, EFT.GameStatus.Stopped, new object[] { });
                this = __instance;
				__instance.localGame_0.Status = global::;
				this.localGame_0.gclass494_0.method_5();
				this.localGame_0.nonWavesSpawnScenario_0.method_2();
				this.localGame_0.wavesSpawnScenario_0.method_3();
				this.localGame_0.gclass493_0.method_13();
				this.localGame_0.gamePlayerOwner_0.Player.vmethod_46(this.exitStatus_0, global::UnityEngine.Mathf.Min((float)this.localGame_0.SessionTime, this.localGame_0.PastTime), this.localGame_0.gclass348_0.Id);
				this.localGame_0.profile_0 = this.localGame_0.list_0[0].Profile;
				int experience = this.localGame_0.list_0[0].Profile.Stats.TotalSessionExperience + this.localGame_0.list_0[0].Profile.Info.Experience;
				global::EFT.LocalGame.MaociLogsThat.Error("Patch created by TheMaoci for: justemutarkov.github.io - If you find it somewhere else tell that to the creators! - version supported (Client.0.11.7.4174)");
				global::EFT.Profile profile = this.localGame_0.list_0[0].Profile;
				string text;
				using (global::System.IO.StreamReader streamReader = new global::System.IO.StreamReader(global::System.Environment.CurrentDirectory.ToString() + "\\client.config.json"))
				{
					text = streamReader.ReadToEnd();
				}
				string[] array = text.Replace("\"{", "").Replace("}\"", "").Split(new char[]
				{
					','
				});
				array[0] = array[0].Replace("\"BackendUrl\":", "");
				array[0] = array[0].Replace("\"", "");
				array[0] = array[0].Replace("{", "").Replace("}", "");
				global::EFT.LocalGame.MaociLogsThat.Error(array[0]);
				string requestUriString = array[0] + "/OfflineRaidSave";
				string text2 = string.Concat(new object[]
				{
					"{ \"status\": \"",
					this.exitStatus_0,
					"\", \"aid\": \"",
					profile.AccountId.ToString(),
					"\"}"
				});
				global::System.IO.File.WriteAllText(global::System.Environment.CurrentDirectory.ToString() + "\\SavedProfile.json", global::Newtonsoft.Json.JsonConvert.SerializeObject(profile.smethod_4(new global::Newtonsoft.Json.JsonConverter[0])).Replace("\"{", "{").Replace("}\"", "}").Replace("\\r\\n", "").Replace("\\", "").Replace("  ", ""));
				text2 = text2.Replace("\"{", "{").Replace("}\"", "}").Replace("  ", "");
				global::System.Net.HttpWebRequest httpWebRequest = (global::System.Net.HttpWebRequest)global::System.Net.WebRequest.Create(requestUriString);
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = "POST";
				using (global::System.IO.StreamWriter streamWriter = new global::System.IO.StreamWriter(httpWebRequest.GetRequestStream()))
				{
					string value = text2;
					streamWriter.Write(value);
					streamWriter.Close();
				}
				foreach (global::EFT.Player player in this.localGame_0.list_0)
				{
					if (player.HandsController != null)
					{
						player.HandsController.imethod_16();
					}
					global::EFT.AssetsManager.AssetPoolObject.smethod_0(player.gameObject);
				}
				this.localGame_0.profile_0.Info.Experience = experience;
				this.localGame_0.list_0.Clear();
				global::StaticManager.Instance.method_0(this.float_0, new global::System.Action(this.method_1));
			}*/
    }
}