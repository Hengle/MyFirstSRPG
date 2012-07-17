using System;
using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public struct Size : IEquatable<Size>
	{
		public int Width;
		public int Height;

		private static Size empty;

		public static Size Empty
		{
			get
			{
				return empty;
			}
		}

		static Size()
		{
			empty = new Size();
		}

		public Size(int width, int height)
		{
			this.Width = width;
			this.Height = height;
		}

		public bool Equals(Size other)
		{
			return (this.Width == other.Width && this.Height == other.Height);
		}

		public override bool Equals(object obj)
		{
			return this.Equals((Size)obj);
		}

		public override int GetHashCode()
		{
			return (this.Width.GetHashCode() + this.Height.GetHashCode());
		}

		public override string ToString()
		{
			return string.Format("{{Width:{0} Height:{1}}}", this.Width, this.Height);
		}

		public static bool operator ==(Size a, Size b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Size a, Size b)
		{
			if (a.Width == b.Width)
			{
				return (a.Height != b.Height);
			}

			return true;
		}

		public Point ToPoint()
		{
			return new Point(this.Width, this.Height);
		}

		public Vector2 ToVector2()
		{
			return new Vector2(this.Width, this.Height);
		}
	}
}