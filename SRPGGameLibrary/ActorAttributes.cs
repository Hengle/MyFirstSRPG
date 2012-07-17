using System;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public struct ActorAttributes
	{
		public int HP;
		public int STR;
		public int MAG;
		public int SKL;
		public int SPD;
		public int LUK;
		public int DEF;
		public int BLD;

		public ActorAttributes(int hp, int str, int mag, int skl, int spd, int luk, int def, int bld)
		{
			this.HP = hp;
			this.STR = str;
			this.MAG = mag;
			this.SKL = skl;
			this.SPD = spd;
			this.LUK = luk;
			this.DEF = def;
			this.BLD = bld;
		}
	}
}
