using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGame
{
	public static class Extensions
	{
		private static StringBuilder sbCharContainer = new StringBuilder(" ");
		private static Dictionary<SpriteFont, Dictionary<char, float>> CharSizes = new Dictionary<SpriteFont, Dictionary<char, float>>();

		public static Vector2 MeasureCharacter(this SpriteFont source, char character)
		{
			sbCharContainer[0] = character;
			return source.MeasureString(sbCharContainer);
		}

		public static Direction GetFaceDirection(this Point source, Point target)
		{
			Direction direction = Direction.Unknown;

			if (source != target)
			{
				if (source.X == target.X)
				{
					if (source.Y > target.Y)
					{
						direction = direction | Direction.Up;
					}
					else
					{
						direction = direction | Direction.Down;
					}
				}
				else
				{
					if (source.X > target.X)
					{
						direction = direction | Direction.Left;
					}
					else
					{
						direction = direction | Direction.Right;
					}
				}
			}

			return direction;
		}

		public static Rectangle ToRectangle(this Point source)
		{
			return new Rectangle(
				source.X * GameMain.CellSize.Width - GameMain.Camera.Location.X,
				source.Y * GameMain.CellSize.Height - GameMain.Camera.Location.Y,
				GameMain.CellSize.Width,
				GameMain.CellSize.Height
			);
		}

		public static string SplitString(this SpriteFont source, string fullString, float maxWidth)
		{
			Dictionary<char, float> sizes;

			if (CharSizes.ContainsKey(source))
			{
				sizes = CharSizes[source];
			}
			else
			{
				sizes = new Dictionary<char, float>();
			}

			StringBuilder output = new StringBuilder();
			float width = 0f;
			float charWidth;
			char c;

			foreach (char character in fullString)
			{
				c = character;

				if (c == '\r')
				{
					continue;
				}
				else if (c == '\n')
				{
					output.Append(c);
					width = 0f;
					continue;
				}
				else if (char.IsWhiteSpace(character))
				{
					c = ' ';
				}

				if (sizes.ContainsKey(c))
				{
					charWidth = sizes[c];
				}
				else
				{
					charWidth = source.MeasureCharacter(c).X;
					sizes.Add(c, charWidth);
				}

				if (width + charWidth > maxWidth)
				{
					output.Append('\n');
					width = 0f;
				}

				output.Append(c);
				width += charWidth;
			}

			return output.ToString();
		}

		public static void DrawString(this SpriteBatch source, SpriteFont font, string str, Vector2 location, Color color, float scale)
		{
			source.DrawString(font, str, location, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
		}

		public static string ToWLV(this int source)
		{
			string wlv;

			if (source > 250)
			{
				wlv = "*";
			}
			else if (source == 250)
			{
				wlv = "A";
			}
			else if (source >= 200)
			{
				wlv = "B";
			}
			else if (source >= 150)
			{
				wlv = "C";
			}
			else if (source >= 100)
			{
				wlv = "D";
			}
			else if (source >= 50)
			{
				wlv = "E";
			}
			else
			{
				wlv = "-";
			}

			return wlv;
		}
	}

	public class PointComparer : IComparer<Point>
	{
		public int Compare(Point x, Point y)
		{
			if (x == y)
			{
				return 0;
			}

			if (x.Y == y.Y)
			{
				return x.X < y.X ? -1 : 1;
			}

			return x.Y < y.Y ? -1 : 1;
		}
	}
}