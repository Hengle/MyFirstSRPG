using Microsoft.Xna.Framework.Content;
using System;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public class MapPoint : IEquatable<MapPoint>
	{
		public int X;
		public int Y;
		public MapTile Tile;

		public MapPoint(int x, int y, MapTile tile)
		{
			this.X = x;
			this.Y = y;
			this.Tile = tile;
		}

		public bool Equals(MapPoint other)
		{
			if (other == null)
			{
				return false;
			}

			return this.X.Equals(other.X) && this.Y.Equals(other.Y);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as MapPoint);
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode() + this.Y.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{{X:{0} Y:{1} Cost:{2}}}", this.X, this.Y, this.Tile.Cost);
		}
	}
}
