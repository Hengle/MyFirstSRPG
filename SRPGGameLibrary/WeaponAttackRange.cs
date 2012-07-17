namespace MyFirstSRPG.SRPGGameLibrary
{
	public struct WeaponAttackRange
	{
		public static readonly WeaponAttackRange R0 = new WeaponAttackRange("");
		public static readonly WeaponAttackRange R1 = new WeaponAttackRange("1");
		public static readonly WeaponAttackRange R1_2 = new WeaponAttackRange("1~2");
		public static readonly WeaponAttackRange R2 = new WeaponAttackRange("2");
		public static readonly WeaponAttackRange R2_3 = new WeaponAttackRange("2~3");
		public static readonly WeaponAttackRange R3_10 = new WeaponAttackRange("3~10");

		private string range;

		private WeaponAttackRange(string range)
		{
			this.range = range;
		}

		public override bool Equals(object obj)
		{
			return this.GetHashCode() == obj.GetHashCode();
		}

		public override int GetHashCode()
		{
			return this.range.GetHashCode();
		}

		public override string ToString()
		{
			return this.range;
		}

		public static bool operator ==(WeaponAttackRange a, WeaponAttackRange b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(WeaponAttackRange a, WeaponAttackRange b)
		{
			return !(a == b);
		}

		public static WeaponAttackRange GetAttackRange(string str)
		{
			WeaponAttackRange range = WeaponAttackRange.R0;

			switch (str)
			{
				case "1":
					range = WeaponAttackRange.R1;
					break;
				case "1~2":
					range = WeaponAttackRange.R1_2;
					break;
				case "2":
					range = WeaponAttackRange.R2;
					break;
				case "2~3":
					range = WeaponAttackRange.R2_3;
					break;
				case "3~10":
					range = WeaponAttackRange.R3_10;
					break;
			}

			return range;
		}
	}
}