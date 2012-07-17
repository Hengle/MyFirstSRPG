using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MyFirstSRPG.SRPGGame;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public class PathFinder
	{
		private PathPoint[,] pathPoints;
		private List<PathPoint> testPointList;
		private List<PathPoint> foundPointList;
		private PathPoint startPoint;

		public PathFinder()
		{
			this.testPointList = new List<PathPoint>();
			this.foundPointList = new List<PathPoint>();
		}

		#region private

		private void RefreshPathPoints()
		{
			this.testPointList.Clear();
			this.foundPointList.Clear();
			this.startPoint = null;

			foreach (var p in this.pathPoints)
			{
				p.Cost = -1;
				p.MinCost = 0;
				p.PrevPoint = null;
			}
		}

		private void UpdatePathPoint(PathPoint basePoint, PathPoint point)
		{
			int g = basePoint.Cost;

			if (point.PrevPoint != null)
			{
				g += point.PrevPoint.Cost;
			}

			if (g < point.Cost)
			{
				point.PrevPoint = basePoint;
				point.Cost = g;
			}
		}

		private void UpdateRangePoint(PathPoint basePoint, PathPoint point)
		{
			var g = basePoint.Cost + point.Cost;

			if (g < point.Cost)
			{
				point.Cost = g;
			}
		}

		private void AddPathPoint(PathPoint basePoint, PathPoint endPoint, PathPoint point)
		{
			point.Cost = basePoint.Cost + point.Cost;
			point.MinCost = Math.Abs(endPoint.X - point.X) + Math.Abs(endPoint.Y - point.Y);
			point.PrevPoint = basePoint;
			this.testPointList.Add(point);
		}

		private void AddRangePoint(PathPoint basePoint, PathPoint point, int mp)
		{
			point.Cost = basePoint.Cost + point.Cost;

			if (point.Cost <= mp)
			{
				this.testPointList.Add(point);
			}
		}

		private PathPoint FindPathPoint(int x, int y)
		{
			return this.FindPathPoint(x, y, -1);
		}

		private PathPoint FindPathPoint(int x, int y, int mp)
		{
			PathPoint point = this.GetPathPoint(x, y);

			if (point == null || !point.Terrain.IsGround || (mp > -1 && point.Cost > mp) || this.foundPointList.Contains(point))
			{
				return null;
			}

			return point;
		}

		private PathPoint[] FindPathPoints(PathPoint basePoint)
		{
			return this.FindPathPoints(basePoint, -1);
		}

		private PathPoint[] FindPathPoints(PathPoint basePoint, int mp)
		{
			PathPoint[] points = new PathPoint[4]
			{
				this.FindPathPoint(basePoint.X, basePoint.Y - 1, mp),
				this.FindPathPoint(basePoint.X, basePoint.Y + 1, mp),
				this.FindPathPoint(basePoint.X - 1, basePoint.Y, mp),
				this.FindPathPoint(basePoint.X + 1, basePoint.Y, mp)
			};

			return points.Where(v => v != null).ToArray();
		}

		#endregion private

		#region public

		public void SetPathPoints(Terrain[,] terrains)
		{
			this.pathPoints = new PathPoint[terrains.GetLength(0), terrains.GetLength(1)];

			for (int x = 0; x < terrains.GetLength(0); x++)
			{
				for (int y = 0; y < terrains.GetLength(1); y++)
				{
					this.pathPoints[x, y] = new PathPoint(x, y, terrains[x, y]);
				}
			}
		}

		public PathPoint GetPathPoint(int x, int y)
		{
			if (x < 0 || x >= this.pathPoints.GetLength(0) || y < 0 || y >= this.pathPoints.GetLength(1))
			{
				return null;
			}

			return this.pathPoints[x, y];
		}

		public PathPoint[] GetMoveRange(PathPoint startPoint, int mp)
		{
			this.RefreshPathPoints();
			this.startPoint = startPoint;
			startPoint.Cost = 0;
			this.testPointList.Add(startPoint);
			PathPoint[] nextPoints;
			PathPoint basePoint;

			while (this.testPointList.Count > 0)
			{
				this.testPointList = this.testPointList.OrderBy(t => t.Distance).ToList();
				basePoint = this.testPointList[0];
				this.testPointList.RemoveAt(0);
				this.foundPointList.Add(basePoint);

				nextPoints = this.FindPathPoints(basePoint, mp);

				foreach (PathPoint point in nextPoints)
				{
					if (this.testPointList.Contains(point))
					{
						this.UpdateRangePoint(basePoint, point);
					}
					else
					{
						this.AddRangePoint(basePoint, point, mp);
					}
				}
			}

			return this.foundPointList.ToArray();
		}

		public AttackRangePoint[] GetAttackRange(IEnumerable<PathPoint> rangePoints, params WeaponRange[] weaponRanges)
		{
			if (rangePoints == null || rangePoints.Count() == 0 || weaponRanges == null || weaponRanges.Length == 0 || weaponRanges.All(w => w == WeaponRange.R0))
			{
				return new AttackRangePoint[0];
			}

			List<PathPoint> attackPoints = new List<PathPoint>();
			PathPoint point;
			int blindDistance;
			int attackDistance;

			foreach (var wr in weaponRanges.Distinct())
			{
				attackPoints.Clear();
				blindDistance = 0;
				attackDistance = 0;

				if (wr == WeaponRange.R1)
				{
					attackDistance = 1;
				}
				else if (wr == WeaponRange.R1_2)
				{
					attackDistance = 2;
				}
				else if (wr == WeaponRange.R2)
				{
					blindDistance = 1;
					attackDistance = 2;
				}
				else if (wr == WeaponRange.R2_3)
				{
					blindDistance = 1;
					attackDistance = 3;
				}
				else if (wr == WeaponRange.R3_10)
				{
					blindDistance = 2;
					attackDistance = 10;
				}

				foreach (var basePoint in rangePoints)
				{
					for (int x = -attackDistance; x <= attackDistance; x++)
					{
						for (int y = -(attackDistance - x); y <= attackDistance - x; y++)
						{
							if (Math.Abs(x) + Math.Abs(y) > blindDistance && Math.Abs(x) + Math.Abs(y) <= attackDistance)
							{
								point = this.GetPathPoint(basePoint.X + x, basePoint.Y + y);

								if (point == null || attackPoints.Contains(point))
								{
									continue;
								}

								point.PrevPoint = basePoint;
								attackPoints.Add(point);
							}
						}
					}
				}

				var result = attackPoints.Except(rangePoints);

				if (result.Count() > 0)
				{
					return result.Select(p => new AttackRangePoint(p.MapPoint, p.PrevPoint.MapPoint, wr)).ToArray();
				}
			}

			return new AttackRangePoint[0];
		}

		public PathPoint[] GetMoveTracks(PathPoint startPoint, PathPoint endPoint, int mov = -1)
		{
			this.RefreshPathPoints();
			this.startPoint = startPoint;
			this.testPointList.Add(startPoint);
			PathPoint[] connectedTiles;
			PathPoint basePoint;
			bool foundPath = false;

			while (this.testPointList.Count > 0)
			{
				this.testPointList = this.testPointList.OrderBy(p => p.Distance).ToList();
				basePoint = this.testPointList[0];
				this.testPointList.RemoveAt(0);
				this.foundPointList.Add(basePoint);
				connectedTiles = this.FindPathPoints(basePoint);

				foreach (PathPoint point in connectedTiles)
				{
					if (this.testPointList.Contains(point))
					{
						this.UpdatePathPoint(basePoint, point);
					}
					else
					{
						this.AddPathPoint(basePoint, endPoint, point);
					}
				}

				if (this.testPointList.Contains(endPoint))
				{
					foundPath = true;
				}
			}

			if (this.testPointList.Contains(endPoint))
			{
				foundPath = true;
			}

			if (foundPath)
			{
				List<PathPoint> points = new List<PathPoint>();
				PathPoint point = endPoint;

				while (point != null)
				{
					points.Add(point);
					point = point.PrevPoint;
				}

				if (mov > 0)
				{
					return points.Where(p => p.Cost <= mov).OrderBy(p => p.Cost).ToArray();
				}

				return points.OrderBy(p => p.Cost).ToArray();
			}

			return new PathPoint[0];
		}

		public Point[] GetMoveTracks(Point startMapPoint, Point endMapPoint, int mov = -1)
		{
			PathPoint startPoint = this.GetPathPoint(startMapPoint.X, startMapPoint.Y);
			PathPoint endPoint = this.GetPathPoint(endMapPoint.X, endMapPoint.Y);
			return this.GetMoveTracks(startPoint, endPoint, mov).Select(p => new Point(p.X, p.Y)).ToArray();
		}

		public Point[] GetMoveRange(Point startMapPoint, int mp)
		{
			PathPoint startPoint = this.GetPathPoint(startMapPoint.X, startMapPoint.Y);
			return this.GetMoveRange(startPoint, mp).Select(p => new Point(p.X, p.Y)).ToArray();
		}

		public AttackRangePoint[] GetAttackRange(IEnumerable<Point> rangeMapPoints, params WeaponRange[] weaponRanges)
		{
			var rangePoints = rangeMapPoints.Select(p => this.GetPathPoint(p.X, p.Y));
			return this.GetAttackRange(rangePoints, weaponRanges);
		}

		#endregion public
	}

	public class PathPoint : IEquatable<PathPoint>
	{
		public int X { get; private set; }

		public int Y { get; private set; }

		public Point MapPoint { get { return new Point(this.X, this.Y); } }

		public Terrain Terrain { get; private set; }

		public int Distance { get { return this.Cost + this.MinCost; } }

		public PathPoint PrevPoint;
		public int MinCost;

		public int Cost
		{
			get { return this.cost == -1 ? this.Terrain.MPCost : this.cost; }
			set { this.cost = value; }
		}

		private int cost = -1;

		public PathPoint(int x, int y, Terrain terrain)
		{
			this.X = x;
			this.Y = y;
			this.Terrain = terrain;
		}

		public bool Equals(PathPoint other)
		{
			if (other == null)
			{
				return false;
			}

			return this.X.Equals(other.X) && this.Y.Equals(other.Y);
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode() + this.Y.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{{X:{0} Y:{1} Cost:{2}}}", this.X, this.Y, this.Terrain);
		}
	}

	public struct AttackRangePoint
	{
		public Point MapPoint;
		public Point SourcePoint;
		public WeaponRange WeaponRange;

		public AttackRangePoint(Point mapPoint, Point srcPoint, WeaponRange weaponRange)
		{
			this.MapPoint = mapPoint;
			this.SourcePoint = srcPoint;
			this.WeaponRange = weaponRange;
		}
	}
}