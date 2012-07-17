using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public class Weapon : Item
	{
		public static Dictionary<int, Weapon> Weapons;
		public static readonly Weapon NoneWeapon = new Weapon(0, ItemType.None, string.Empty, WeaponRange.R0);

		public WeaponRange Range;
		public int Rank;
		public int Hit;
		public int Power;
		public int Critial;
		public int Weight;

		public Weapon(int id, ItemType type, string name, WeaponRange range, int rank = 50, int hit = 0, int power = 0, int critical = 0, int weight = 0, Rectangle srcRegion = default(Rectangle), string comment = "")
			: base(id, type, name, srcRegion, comment)
		{
			this.Range = range;
			this.Rank = rank;
			this.Hit = hit;
			this.Power = power;
			this.Critial = critical;
			this.Weight = weight;
		}
	}

	public class WeaponReader : ContentTypeReader<Weapon>
	{
		protected override Weapon Read(ContentReader input, Weapon existingInstance)
		{
			Weapon weapon = existingInstance;

			if (weapon == null)
			{
				weapon = new Weapon(
					input.ReadInt32(),
					input.ReadObject<ItemType>(),
					input.ReadString(),
					WeaponRange.GetRange(input.ReadString()),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadInt32(),
					input.ReadObject<Rectangle>(),
					input.ReadString()
				);
			}

			return weapon;
		}
	}
}