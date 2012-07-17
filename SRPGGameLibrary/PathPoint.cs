using System;
using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public class PathPoint : IEquatable<PathPoint>
	{
		public MapPoint MapPoint;
		public PathPoint PrevPoint;
		public int MinMPCost;

		private int mpCost = -1;

		public bool IsBlock { get { return this.MapPoint.Tile.Cost == 99; } }
		public int Distance { get { return this.MPCost + this.MinMPCost; } }

		public int MPCost
		{
			get { return this.mpCost == -1 ? this.MapPoint.Tile.Cost : this.mpCost; }
			set { this.mpCost = value; }
		}

		public PathPoint(MapPoint mapPoint)
		{
			this.MapPoint = mapPoint;
		}

		public bool Equals(PathPoint other)
		{
			return this.MapPoint == other.MapPoint;
		}

		public override string ToString()
		{
			return this.MapPoint.ToString();
		}
	}
}
