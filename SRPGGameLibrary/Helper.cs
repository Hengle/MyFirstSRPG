using System.Linq;
using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public static class Helper
	{
		public static int Min(int a, int b)
		{
			return a > b ? b : a;
		}

		public static int Max(int a, int b)
		{
			return a > b ? a : b;
		}

		public static Color ConvertToColor(uint value)
		{
			byte a = 0xff;

			if (value > 0xffffff)
			{
				a = (byte)(value >> 24);
			}

			byte r = (byte)(value >> 16);
			byte g = (byte)(value >> 8);
			byte b = (byte)(value >> 0);
			return new Color(r, g, b, a);
		}

		public static WeaponRange GetAttackRange(string str)
		{
			WeaponRange range = WeaponRange.R0;

			switch (str)
			{
				case "1":
					range = WeaponRange.R1;
					break;
				case "1~2":
					range = WeaponRange.R1_2;
					break;
				case "2":
					range = WeaponRange.R2;
					break;
				case "2~3":
					range = WeaponRange.R2_3;
					break;
				case "3~10":
					range = WeaponRange.R3_10;
					break;
			}

			return range;
		}

		public static Rectangle GetRectangle(string region)
		{
			if (!string.IsNullOrEmpty(region))
			{
				int[] values = region.Split('|').Select(i => i.ParseInt32()).ToArray();
				return new Rectangle(values[0], values[1], values[2], values[3]);
			}

			return Rectangle.Empty;
		}
	}
}