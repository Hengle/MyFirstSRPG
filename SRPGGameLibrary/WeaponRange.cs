namespace MyFirstSRPG.SRPGGameLibrary
{
	public struct WeaponRange
	{
		public static readonly WeaponRange R0 = new WeaponRange("");
		public static readonly WeaponRange R1 = new WeaponRange("1");
		public static readonly WeaponRange R1_2 = new WeaponRange("1~2");
		public static readonly WeaponRange R2 = new WeaponRange("2");
		public static readonly WeaponRange R2_3 = new WeaponRange("2~3");
		public static readonly WeaponRange R3_10 = new WeaponRange("3~10");

		private string range;

		private WeaponRange(string range)
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

		public static bool operator ==(WeaponRange a, WeaponRange b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(WeaponRange a, WeaponRange b)
		{
			return !(a == b);
		}

		public static WeaponRange GetRange(string str)
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
	}
}