using System;
using Microsoft.Xna.Framework;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGame
{
	public class Camera
	{
		public Point Location;
		public Point MoveDistance;
		public float MoveSpeed;
		public bool IsMoving = false;
		public bool IsLocked = false;

		private GameMain game;
		private Rectangle range;
		private TimeSpan tsMove = TimeSpan.Zero;
		private Point lastLocation;
		private Direction currentDirection;

		public Camera(GameMain game)
		{
			this.game = game;
		}

		public void SetCamera(Size mapSize, Point moveDistance)
		{
			this.SetCamera(mapSize, moveDistance, 1f / 6, Point.Zero);
		}

		public void SetCamera(Size mapSize, Point moveDistance, float moveSpeed, Point location)
		{
			this.MoveDistance = moveDistance;
			this.MoveSpeed = moveSpeed;
			this.lastLocation =
				this.Location = location;

			this.range = GetRange(mapSize, GameMain.ScreenBounds.GetSize());
		}

		public void Move(GameTime gameTime)
		{
			this.Move(gameTime, Direction.Unknown);
		}

		public void Move(GameTime gameTime, Direction direction)
		{
			if (this.IsLocked)
			{
				return;
			}

			if (direction != Direction.Unknown)
			{
				this.currentDirection = direction;
			}

			if (this.currentDirection == Direction.Unknown)
			{
				return;
			}
			else if (!this.IsMoving)
			{
				this.Start();
			}

			this.tsMove += gameTime.ElapsedGameTime;

			if (this.tsMove.TotalSeconds < this.MoveSpeed)
			{
				this.Continue();
			}
			else
			{
				this.Stop();
			}
		}

		public void Start()
		{
			this.lastLocation = this.Location;
			this.tsMove = TimeSpan.Zero;
			this.IsMoving = true;
		}

		private void Continue()
		{
			Point offset = GetOffset(this.currentDirection, this.MoveDistance, (float)(this.tsMove.TotalSeconds / this.MoveSpeed));
			this.Location = this.lastLocation.Expand(offset);
		}

		public void Stop()
		{
			this.Location = this.lastLocation.Expand(GetOffset(this.currentDirection, this.MoveDistance));
			this.lastLocation = this.Location;
			this.tsMove = TimeSpan.Zero;
			this.currentDirection = Direction.Unknown;
			this.IsMoving = false;
		}

		public void Lock()
		{
			if (!(this.Location.X.IsBetween(this.range.Left, this.range.Right) && this.Location.Y.IsBetween(this.range.Top, this.range.Bottom)))
			{
				this.Stop();
				this.Location = this.Location.Clamp(this.range);
			}
		}

		private static Rectangle GetRange(Size mapSize, Size screenSize)
		{
			Rectangle range = Rectangle.Empty;
			range.Width = Helper.Max(0, mapSize.Width - screenSize.Width);
			range.Height = Helper.Max(0, mapSize.Height - screenSize.Height);
			return range;
		}

		public static Point GetOffset(Direction direction, Point distance)
		{
			return GetOffset(direction, distance, 1f);
		}

		public static Point GetOffset(Direction direction, Point distance, float percentage)
		{
			Point offset = Point.Zero;
			Point incremental = distance.Zoom(percentage);

			if (direction.Contains(Direction.Up))
			{
				offset.Y = -incremental.Y;
			}

			if (direction.Contains(Direction.Right))
			{
				offset.X = +incremental.X;
			}

			if (direction.Contains(Direction.Down))
			{
				offset.Y = +incremental.Y;
			}

			if (direction.Contains(Direction.Left))
			{
				offset.X = -incremental.X;
			}

			return offset;
		}
	}
}