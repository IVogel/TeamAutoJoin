using Terraria.ModLoader;
using Terraria.IO;
using Terraria;
using System.IO;

namespace TeamAutoJoin
{
	internal static class Config
	{
		public static int LoadTeam()
		{
			if(!Config.preferences.Load())
			{
				Config.SaveTeam(0);
				return 0;
			}
			return Config.preferences.Get<int>("team", 0);
		}
		public static void SaveTeam(int team)
		{
			Config.preferences.Clear();
			Config.preferences.Put("team", team);
			Config.preferences.Save(true);
		}
		public static readonly string configPath       = Path.Combine(Main.SavePath, "Mod Configs", "TeamAutoJoin.json");
		public static readonly Preferences preferences = new Preferences(Config.configPath, true, true);
	}

	class TeamAutoJoin : Mod
	{
		public override void Load()
		{
			base.Load();
			Properties = new ModProperties()
			{
				Autoload = true
			};
		}
	}

	class TeamAutoJoinPlayer : ModPlayer
	{
		private bool teamLoaded = false;
		private int  team       = 0;
		public override void OnEnterWorld(Player player)
		{
			if (Main.LocalPlayer == player)
			{
				team = Config.LoadTeam();
				Main.LocalPlayer.team = team;
			}
			base.OnEnterWorld(player);
		}

		// This shit wont save preferences, if it was called from this functuion. WTF?!
		/*
		public override void PlayerDisconnect(Player player)
		{
			Config.SaveTeam(Main.LocalPlayer.team);
			base.PlayerDisconnect(player);
		}
		*/

		public override void SendClientChanges(ModPlayer localPlayer)
		{
			if (team != Main.LocalPlayer.team)
			{
				team = Main.LocalPlayer.team;
				Config.SaveTeam(team);
			}
			if (!teamLoaded)
			{
				if(Main.LocalPlayer.team > 0)
				{
					NetMessage.SendData(45, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
				}
				teamLoaded = true;
			}
			base.SendClientChanges(localPlayer);
		}
	}
}
