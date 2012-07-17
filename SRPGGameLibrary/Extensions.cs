using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public static class Extensions
	{
		public static int ParseInt32(this string source)
		{
			int value;
			int.TryParse(source, out value);
			return value;
		}

		public static bool In<T>(this T source, params T[] array)
		{
			return In(source, (IEnumerable<T>)array);
		}

		public static bool In<T>(this T source, IEnumerable<T> array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}

			return array.Contains(source);
		}

		public static bool IsBetween<T>(this T source, T tMin, T tMax) where T : IComparable<T>, IEquatable<T>
		{
			return source.CompareTo(tMin) >= 0 && source.CompareTo(tMax) <= 0;
		}

		public static int Clamp(this int source, int min, int max)
		{
			if (min > max)
			{
				throw new ArgumentException("'max' less than 'min'");
			}

			if (source < min)
			{
				return min;
			}
			else if (source > max)
			{
				return max;
			}

			return source;
		}

		public static Point Expand(this Point source, Point point)
		{
			return new Point(source.X + point.X, source.Y + point.Y);
		}

		public static Point Expand(this Point source, int x, int y)
		{
			return new Point(source.X + x, source.Y + y);
		}

		public static Point Zoom(this Point source, float scale)
		{
			float x = source.X * scale;
			float y = source.Y * scale;

			if (x < int.MinValue || x > int.MaxValue || y < int.MinValue || y > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("Out of int range");
			}

			return new Point((int)x, (int)y);
		}

		public static Size Zoom(this Size source, float scale)
		{
			float width = source.Width * scale;
			float height = source.Height * scale;

			if (width < int.MinValue || width > int.MaxValue || height < int.MinValue || height > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("Out of int range");
			}

			return new Size((int)width, (int)height);
		}

		public static Rectangle Zoom(this Rectangle source, float scale)
		{
			float width = source.Width * scale;
			float height = source.Height * scale;

			if (width < int.MinValue || width > int.MaxValue || height < int.MinValue || height > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("Out of int range");
			}

			return new Rectangle(source.X, source.Y, (int)width, (int)height);
		}

		public static Point Clamp(this Point source, Rectangle range)
		{
			Point point = Point.Zero;
			point.X = Clamp(source.X, range.Left, range.Right);
			point.Y = Clamp(source.Y, range.Top, range.Bottom);
			return point;
		}

		public static Vector2 ToVector2(this Point source)
		{
			return new Vector2(source.X, source.Y);
		}

		public static Size GetSize(this Rectangle source)
		{
			return new Size(source.Width, source.Height);
		}

		public static Vector2 Expand(this Vector2 source, Point point)
		{
			return new Vector2(source.X + point.X, source.Y + point.Y);
		}

		public static Vector2 Expand(this Vector2 source, float x, float y)
		{
			return new Vector2(source.X + x, source.Y + y);
		}

		public static Point ToPoint(this Vector2 source)
		{
			return new Point((int)source.X, (int)source.Y);
		}

		public static Size ToSize(this Vector2 source)
		{
			return new Size((int)source.X, (int)source.Y);
		}

		public static bool Contains(this Direction source, Direction direction)
		{
			return (source & direction) == direction;
		}

		public static T[,] ToPivot<T>(this T[,] source)
		{
			T[,] output = new T[source.GetLength(1), source.GetLength(0)];

			for (int i = 0; i < source.GetLength(1); i++)
			{
				for (int j = 0; j < source.GetLength(0); j++)
				{
					output[i, j] = source[j, i];
				}
			}

			return output;
		}

		public static TResult[,] Select<TSource, TResult>(this TSource[,] source, Func<TSource, TResult> selector)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			else if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}

			TResult[,] result = new TResult[source.GetLength(0), source.GetLength(1)];

			for (int i = 0; i < source.GetLength(0); i++)
			{
				for (int j = 0; j < source.GetLength(1); j++)
				{
					result[i, j] = selector(source[i, j]);
				}
			}

			return result;
		}
	}
}