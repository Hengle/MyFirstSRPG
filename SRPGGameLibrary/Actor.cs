using System.Collections.Generic;
using System.Linq;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public class Actor
	{

		public int LV;
		public int EXP;
		public int ID;
		public int ClassID;
		public string Name;
		public int CurrentHP;
		public ActorAttributes Basics;
		public ActorAttributes Growth;
		public int Sword;
		public int Lance;
		public int Axe;
		public int Bow;
		public int Staff;
		public int Fire;
		public int Thunder;
		public int Wind;
		public int Light;
		public int Dark;
		public int[] PerkIDs;

		public int ActualHP { get { return this.Basics.HP + this.Class.HP; } }

		public int ActualSTR { get { return this.Basics.STR + this.Class.STR; } }

		public int ActualMAG { get { return this.Basics.MAG + this.Class.MAG; } }

		public int ActualSKL { get { return this.Basics.SKL + this.Class.SKL; } }

		public int ActualSPD { get { return this.Basics.SPD + this.Class.SPD; } }

		public int ActualLUK { get { return this.Basics.LUK; } }

		public int ActualDEF { get { return this.Basics.DEF + this.Class.DEF; } }

		public int ActualBLD { get { return this.Basics.BLD + this.Class.BLD; } }

		public int ActualMOV { get { return this.Class.MOV; } }

		public WeaponRange WeaponRange { get { return this.CurrentWeapon.Range; } }

		public WeaponRange[] WeaponRanges { get { return this.Items.OfType<Weapon>().Select(w => w.Range).ToArray(); } }

		public ActorPerk[] Perks
		{
			get
			{
				if (this.perks == null)
				{
					List<int> perkIDs = new List<int>();

					if (this.PerkIDs != null)
					{
						perkIDs.AddRange(this.PerkIDs);
					}

					if (this.Class.PerkIDs != null)
					{
						perkIDs.AddRange(this.Class.PerkIDs);
					}

					this.perks = ActorPerk.Perks.Where(p => p.Key.In(perkIDs.ToArray())).Select(p => p.Value).ToArray();
				}

				return this.perks;
			}
		}

		public ActorClass Class
		{
			get
			{
				if (this.actorClass == null)
				{
					this.actorClass = ActorClass.Classes[this.ClassID];
					this.Sword += this.actorClass.Sword;
					this.Lance += this.actorClass.Lance;
					this.Axe += this.actorClass.Axe;
					this.Bow += this.actorClass.Bow;
					this.Staff += this.actorClass.Staff;
					this.Fire += this.actorClass.Fire;
					this.Thunder += this.actorClass.Thunder;
					this.Wind += this.actorClass.Wind;
					this.Light += this.actorClass.Light;
					this.Dark += this.actorClass.Dark;
				}

				return this.actorClass;
			}
		}

		public List<Item> Items;

		public Weapon CurrentWeapon
		{
			get
			{
				if (this.Items.Count > 0 && this.Items[0] is Weapon)
				{
					return this.Items[0] as Weapon;
				}

				return Weapon.NoneWeapon;
			}
		}

		public int ActualAttackPower
		{
			get
			{
				if (this.CurrentWeapon == Weapon.NoneWeapon)
				{
					return 0;
				}

				return this.ActualSTR + this.CurrentWeapon.Power;
			}
		}

		public int ActualAttackSpeed
		{
			get
			{
				int atkSPD = this.ActualSPD;

				if (this.CurrentWeapon != Weapon.NoneWeapon)
				{
					atkSPD = this.ActualSPD - (this.CurrentWeapon.Weight - this.ActualBLD).Clamp(0, this.CurrentWeapon.Weight);
				}

				return atkSPD;
			}
		}

		public int ActualAccuracy
		{
			get
			{
				int accuracy = 0;

				if (this.CurrentWeapon != Weapon.NoneWeapon)
				{
					accuracy = this.CurrentWeapon.Hit + this.ActualSKL * 2 + this.ActualLUK;
				}

				return accuracy;
			}
		}

		public int ActualAvoid
		{
			get
			{
				return this.ActualAttackSpeed * 2 + this.ActualLUK;
			}
		}

		public int ActualCritical
		{
			get
			{
				if (this.CurrentWeapon != Weapon.NoneWeapon)
				{
					return this.CurrentWeapon.Critial + this.ActualSKL;
				}

				return 0;
			}
		}

		public int ActualCriticalEvade
		{
			get
			{
				return this.ActualLUK / 2;
			}
		}

		private ActorPerk[] perks;
		private ActorClass actorClass;

		public Actor(int id, int classId, int lv, string name)
			: this(id, classId, lv, 0, name, 0, 0, 0, 0, 0, 0, 0, 0, null)
		{ }

		public Actor(int id, int classID, int lv, int exp, string name, int hp, int str, int mag, int skl, int spd, int luk, int def, int bld, int[] perkIDs)
		{

			this.ID = id;
			this.ClassID = classID;
			this.LV = lv;
			this.EXP = exp;
			this.Name = name;
			this.Basics = new ActorAttributes(hp, str, mag, skl, spd, luk, def, bld);
			this.PerkIDs = perkIDs;
			this.Items = new List<Item>();
			this.CurrentHP = this.ActualHP;
		}

		public Actor(int id, int classID, int lv, int exp, string name, ActorAttributes basics, int[] perkIDs)
		{
			this.ID = id;
			this.ClassID = classID;
			this.LV = lv;
			this.EXP = exp;
			this.Name = name;
			this.Basics = basics;
			this.PerkIDs = perkIDs;
			this.Items = new List<Item>();
			this.CurrentHP = this.ActualHP;
		}
	}
}