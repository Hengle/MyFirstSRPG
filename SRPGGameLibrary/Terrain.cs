namespace MyFirstSRPG.SRPGGameLibrary
{
	public struct Terrain
	{
		public byte MPCost;
		public string Name;
		public bool IsGround;
		public int DEF;
		public int Avoid;
		public bool IsRecoverable;

		public Terrain(string name, byte mpCost = 1, bool isGround = false, int def = 0, int avoid = 0, bool isRecoverable = false)
		{
			this.Name = name;
			this.MPCost = mpCost;
			this.IsGround = isGround;
			this.DEF = def;
			this.Avoid = avoid;
			this.IsRecoverable = isRecoverable;
		}
	}
}