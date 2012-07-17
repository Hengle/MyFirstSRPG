using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public class PathFinder
	{
		public static PathFinder Instance = new PathFinder();

		public PathPoint[,] PathPoints;
		private List<PathPoint> testPointList;
		private List<PathPoint> foundPointList;

		public PathFinder()
		{
			this.testPointList = new List<PathPoint>();
			this.foundPointList = new List<PathPoint>();
		}

		public void SetPathPoints(MapPoint[,] mapPoints)
		{
			this.PathPoints = new PathPoint[mapPoints.GetLength(0), mapPoints.GetLength(1)];

			for (int y = 0; y < mapPoints.GetLength(0); y++)
			{
				for (int x = 0; x < mapPoints.GetLength(1); x++)
				{
					this.PathPoints[y, x] = new PathPoint(mapPoints[y, x]);
				}
			}
		}

		private void RefreshPathPoints()
		{
			this.testPointList.Clear();
			this.foundPointList.Clear();

			foreach (var p in this.PathPoints)
			{
				p.MPCost = -1;
				p.MinMPCost = 0;
				p.PrevPoint = null;
			}
		}

		public PathPoint GetPathPoint(int x, int y)
		{
			if (x < 0 || x >= this.PathPoints.GetLength(1) || y < 0 || y >= this.PathPoints.GetLength(0))
			{
				return null;
			}

			return this.PathPoints[y, x];
		}

		private void UpdatePathPoint(PathPoint basePoint, PathPoint point)
		{
			int g = basePoint.MPCost;

			if (point.PrevPoint != null)
			{
				g += point.PrevPoint.MPCost;
			}

			if (g < point.MPCost)
			{
				point.PrevPoint = basePoint;
				point.MPCost = g;
			}
		}

		private void UpdateRangePoint(PathPoint basePoint, PathPoint point)
		{
			var g = basePoint.MPCost + point.MPCost;

			if (g < point.MPCost)
			{
				point.MPCost = g;
			}
		}

		private void AddPathPoint(PathPoint basePoint, PathPoint endPoint, PathPoint point)
		{
			point.MPCost = basePoint.MPCost + point.MPCost;
			point.MinMPCost = Math.Abs(endPoint.MapPoint.X - point.MapPoint.X) + Math.Abs(endPoint.MapPoint.Y - point.MapPoint.Y);
			point.PrevPoint = basePoint;
			this.testPointList.Add(point);
		}

		private void AddRangePoint(PathPoint basePoint, PathPoint point, int mp)
		{
			point.MPCost = basePoint.MPCost + point.MPCost;

			if (point.MPCost <= mp)
			{
				this.testPointList.Add(point);
			}
		}

		private PathPoint FindPathPoint(int x, int y)
		{
			PathPoint point = this.GetPathPoint(x, y);

			if (point == null || point.IsBlock || this.foundPointList.Contains(point))
			{
				return null;
			}

			return point;
		}

		private PathPoint FindRangePoint(PathPoint basePoint, int x, int y, int mp)
		{
			PathPoint point = this.GetPathPoint(x, y);

			if (point == null || point.MPCost > mp || point.IsBlock || this.foundPointList.Contains(point))
			{
				return null;
			}

			return point;
		}

		private PathPoint[] FindPathPoints(PathPoint basePoint)
		{
			PathPoint[] points = new PathPoint[4]
			{
				this.FindPathPoint(basePoint.MapPoint.X, basePoint.MapPoint.Y - 1),
				this.FindPathPoint(basePoint.MapPoint.X, basePoint.MapPoint.Y + 1),
				this.FindPathPoint(basePoint.MapPoint.X - 1, basePoint.MapPoint.Y),
				this.FindPathPoint(basePoint.MapPoint.X + 1, basePoint.MapPoint.Y)
			};

			return points.Where(v => v != null).ToArray();
		}

		private PathPoint[] FindRangePoints(PathPoint basePoint, int mp)
		{
			PathPoint[] points = new PathPoint[4]
			{
				this.FindRangePoint(basePoint, basePoint.MapPoint.X, basePoint.MapPoint.Y - 1, mp),
				this.FindRangePoint(basePoint, basePoint.MapPoint.X, basePoint.MapPoint.Y + 1, mp),
				this.FindRangePoint(basePoint, basePoint.MapPoint.X - 1, basePoint.MapPoint.Y, mp),
				this.FindRangePoint(basePoint, basePoint.MapPoint.X + 1, basePoint.MapPoint.Y, mp)
			};

			return points.Where(v => v != null).ToArray();
		}

		public MapPoint[] GetMoveTracks(MapPoint startPoint, MapPoint endPoint)
		{
			PathPoint point1 = this.GetPathPoint(startPoint.X, startPoint.Y);

			if (point1 != null)
			{
				PathPoint point2 = this.GetPathPoint(endPoint.X, endPoint.Y);

				if (point2 != null)
				{
					return this.GetPath(point1, point2);
				}
			}

			return null;
		}

		public MapPoint[] GetPath(PathPoint startPoint, PathPoint endPoint)
		{
			this.RefreshPathPoints();
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

				return points.OrderBy(p => p.MPCost).Select(p => p.MapPoint).ToArray();
			}

			return null;
		}

		public MapPoint[] GetMoveRange(MapPoint startPoint, int mp)
		{
			PathPoint point = this.PathPoints[startPoint.Y, startPoint.X];
			return this.GetRange(point, mp);
		}

		public MapPoint[] GetRange(PathPoint startPoint, int mp)
		{
			this.RefreshPathPoints();
			startPoint.MPCost = 0;
			this.testPointList.Add(startPoint);
			PathPoint[] nextPoints;
			PathPoint basePoint;

			while (this.testPointList.Count > 0)
			{
				this.testPointList = this.testPointList.OrderBy(t => t.Distance).ToList();
				basePoint = this.testPointList[0];
				this.testPointList.RemoveAt(0);
				this.foundPointList.Add(basePoint);

				nextPoints = this.FindRangePoints(basePoint, mp);

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

			return this.foundPointList.Select(p => p.MapPoint).ToArray();
		}

		public MapPoint[] GetAttackRange(MapPoint[] rangePoints, AttackRange attackRange)
		{
			if (rangePoints == null || rangePoints.Length == 0 || attackRange == AttackRange.R0)
			{
				return null;
			}

			List<PathPoint> attackPoints = new List<PathPoint>();
			PathPoint point;
			int blindDistance = 0;
			int attackDistance = 0;

			switch (attackRange)
			{
				case AttackRange.R1:
					attackDistance = 1;
					break;
				case AttackRange.R1_2:
					attackDistance = 2;
					break;
				case AttackRange.R2:
					blindDistance = 1;
					attackDistance = 2;
					break;
				case AttackRange.R2_3:
					blindDistance = 1;
					attackDistance = 3;
					break;
				case AttackRange.R3_10:
					blindDistance = 2;
					attackDistance = 10;
					break;
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

							attackPoints.Add(point);
						}
					}
				}
			}

			return attackPoints.Select(p => p.MapPoint).Except(rangePoints).ToArray();
		}

		public static Direction GetDirection(MapPoint srcPoint, MapPoint destPoint)
		{
			Direction direction = Direction.Unknown;

			if (srcPoint != destPoint)
			{
				if (srcPoint.X == destPoint.X)
				{
					direction = srcPoint.Y > destPoint.Y ? Direction.Up : Direction.Down;
				}
				else
				{
					direction = srcPoint.X > destPoint.X ? Direction.Left : Direction.Right;
				}
			}

			return direction;
		}
	}
}
