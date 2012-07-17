using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MyFirstSRPG.SRPGGameLibrary
{
	/// <summary>
	/// ID,Name,HP,STR,MAG,SKL,SPD,DEF,BLD,MOV,Sword,Lance,Axe,Bow,Staff,Fire,Thunder,Wind,Light,Dark,Perk,UpperClassID,Tile1,Region1,Offset1,Tile2,Region2,Offset2,Comment
	/// </summary>
	public class ActorClass
	{
		public static Dictionary<int, ActorClass> Classes;

		public int ID { get; private set; }

		public string Name { get; private set; }

		public int HP { get; private set; }

		public int STR { get; private set; }

		public int MAG { get; private set; }

		public int SKL { get; private set; }

		public int SPD { get; private set; }

		public int DEF { get; private set; }

		public int BLD { get; private set; }

		public int MOV { get; private set; }

		public int Sword { get; private set; }

		public int Lance { get; private set; }

		public int Axe { get; private set; }

		public int Bow { get; private set; }

		public int Staff { get; private set; }

		public int Fire { get; private set; }

		public int Thunder { get; private set; }

		public int Wind { get; private set; }

		public int Light { get; private set; }

		public int Dark { get; private set; }

		public int[] PerkIDs { get; private set; }

		public int UpperClassID { get; private set; }

		public string Tile1 { get; private set; }

		public Rectangle Region1 { get; private set; }

		public Point Offset1 { get; private set; }

		public string Tile2 { get; private set; }

		public Rectangle Region2 { get; private set; }

		public Point Offset2 { get; private set; }

		public int Sfx { get; private set; }

		public ActorClass(int id, string name, int hp, int str, int mag, int skl, int spd, int def, int bld, int mov, int sword, int lance, int axe, int bow, int staff, int fire, int thunder, int wind, int light, int dark, int[] perkIDs, int upperClassID, string tile1, Rectangle region1, Point offset1, string tile2, Rectangle region2, Point offset2, int sfx)
		{
			this.ID = id;
			this.Name = name;
			this.HP = hp;
			this.STR = str;
			this.MAG = mag;
			this.SKL = skl;
			this.SPD = spd;
			this.DEF = def;
			this.BLD = bld;
			this.MOV = mov;
			this.Sword = sword;
			this.Lance = lance;
			this.Axe = axe;
			this.Bow = bow;
			this.Staff = staff;
			this.Fire = fire;
			this.Thunder = thunder;
			this.Wind = wind;
			this.Light = light;
			this.Dark = dark;
			this.PerkIDs = perkIDs;
			this.UpperClassID = upperClassID;
			this.Tile1 = tile1;
			this.Region1 = region1;
			this.Offset1 = offset1;
			this.Tile2 = tile2;
			this.Region2 = region2;
			this.Offset2 = offset2;
			this.Sfx = sfx;
		}
	}

	public class ActorClassReader : ContentTypeReader<ActorClass>
	{
		protected override ActorClass Read(ContentReader input, ActorClass existingInstance)
		{
			ActorClass actorClass = existingInstance;

			if (actorClass == null)
			{
				actorClass = new ActorClass(
					input.ReadInt32(),
					input.ReadString(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadObject<int[]>(),
					input.ReadInt32(),
					input.ReadString(),
					input.ReadObject<Rectangle>(),
					input.ReadObject<Point>(),
					input.ReadString(),
					input.ReadObject<Rectangle>(),
					input.ReadObject<Point>(),
					input.ReadInt32()
				);
			}

			return actorClass;
		}
	}
}