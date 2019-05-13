using Terraria.ModLoader;
using Terraria;
using System.IO;

namespace TeamAutoJoin
{
	static class config
	{
		private static bool loaded = false;
		public  static byte team   = 0;

		private static readonly string configfolder = Path.Combine(Main.SavePath, "Mod Configs");
		private static readonly string configpath   = Path.Combine(configfolder, "TeamAutoJoin");

		public static void SaveTeam(byte team)
		{
			config.team = team;
			if (!Directory.Exists(configfolder))
			{
				Directory.CreateDirectory(configfolder);
			}
			using (BinaryWriter stream = new BinaryWriter(File.Open(configpath, FileMode.OpenOrCreate)))
			{
				stream.Write(team);
			}
		}

		public static byte LoadTeam()
		{
			loaded = true;

			if (!File.Exists(configpath))
			{
				SaveTeam(0);
				return team;
			}
			using (BinaryReader stream = new BinaryReader(File.Open(configpath, FileMode.Open)))
			{
				team = stream.ReadByte();
			}

			return team;
		}

		public static byte GetTeam()
		{
			if (loaded)
				return team;

			return LoadTeam();
		}
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
				team = config.GetTeam();
				Main.LocalPlayer.team = team;
			}
			base.OnEnterWorld(player);
		}

		public override void SendClientChanges(ModPlayer localPlayer)
		{
			if (team != Main.LocalPlayer.team)
			{
				team = Main.LocalPlayer.team;
				config.SaveTeam((byte)team);
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
