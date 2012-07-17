using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyFirstSRPG.SRPGGameLibrary;
using Microsoft.Xna.Framework.Graphics;

namespace MyFirstSRPG.SRPGGame.GameScreens.SceneScreens
{
	public class LevelUpScreen : GameScreen
	{
		private SceneActor actor;
		private Rectangle frameSrcRect;
		private Rectangle frameDestRect;
		private Rectangle arrowSrcRect;
		private Rectangle[] arrowDestRects;
		private bool[] enableArrows;
		private int arrowOffset;
		private Vector2[] attrLocations;
		private string[] attrs;
		private int[] attrUps;
		private string[] attrUpStrs;
		private List<int> attrUpIndexes;
		private bool isAttrAdded;
		private Color blue;
		private TimeSpan elapsedTime;

		public LevelUpScreen(SceneActor actor)
		{
			this.actor = actor;
			this.IsPopup = true;
			this.attrUps = actor.GetGrowth();
			this.attrUpStrs = this.attrUps.Select(i => i == 0 ? null : i.ToString()).ToArray();
			this.attrUpIndexes = this.attrUps.Select((a, i) => a > 0 ? i : -1).Where(i => i > -1).ToList();
		}

		public override void LoadContent()
		{
			this.frameSrcRect = new Rectangle(0, 271, 112, 48);
			this.frameDestRect = this.frameSrcRect.Zoom(GameMain.TextureScale);
			this.frameDestRect.X = (GameMain.ScreenBounds.Width - this.frameDestRect.Width) / 2;
			this.frameDestRect.Y = (GameMain.ScreenBounds.Height - this.frameDestRect.Height) / 2;

			this.arrowSrcRect = new Rectangle(176, 0, 7, 8);
			this.arrowDestRects = new Rectangle[8];
			this.enableArrows = new bool[8];
			this.attrLocations = new Vector2[8];

			for (int i = 0; i < 8; i++)
			{
				this.arrowDestRects[i] = this.arrowSrcRect.Zoom(GameMain.TextureScale);
			}

			for (int i = 0; i < 4; i++)
			{
				this.arrowDestRects[i].X = this.frameDestRect.X + 39 * GameMain.TextureScale;
				this.arrowDestRects[i + 4].X = this.frameDestRect.X + 84 * GameMain.TextureScale;

				this.attrLocations[i].X = this.frameDestRect.X + 27 * GameMain.TextureScale;
				this.attrLocations[i + 4].X = this.frameDestRect.X + 72 * GameMain.TextureScale;

				this.arrowDestRects[i].Y =
					this.arrowDestRects[i + 4].Y = this.frameDestRect.Y + (i * arrowSrcRect.Height + 8) * GameMain.TextureScale;

				this.attrLocations[i].Y =
					this.attrLocations[i + 4].Y = this.frameDestRect.Y + (i * 8 + 5) * GameMain.TextureScale;
			}

			this.attrs = new string[8];
			this.attrs[0] = this.actor.ActualHP.ToString().PadLeft(2);
			this.attrs[1] = this.actor.ActualSTR.ToString().PadLeft(2);
			this.attrs[2] = this.actor.ActualMAG.ToString().PadLeft(2);
			this.attrs[3] = this.actor.ActualSKL.ToString().PadLeft(2);
			this.attrs[4] = this.actor.ActualSPD.ToString().PadLeft(2);
			this.attrs[5] = this.actor.ActualLUK.ToString().PadLeft(2);
			this.attrs[6] = this.actor.ActualDEF.ToString().PadLeft(2);
			this.attrs[7] = this.actor.ActualBLD.ToString().PadLeft(2);
		}

		public override void Update(GameTime gameTime, bool isCovered, bool isFocusLost)
		{
			base.Update(gameTime, isCovered, isFocusLost);

			this.elapsedTime += gameTime.ElapsedGameTime;

			if (this.elapsedTime.TotalMilliseconds >= 2000d + this.attrUpIndexes.Count * 500d)
			{
				this.Exit();
				return;
			}

			this.Brush = Color.White * this.SmoothTransitionOffset;
			this.blue = Helper.ConvertToColor(0xc0e8f8) * this.SmoothTransitionOffset;
			double cursorMS = this.elapsedTime.TotalMilliseconds % 1333d;

			if (cursorMS < 666d)
			{
				this.arrowOffset = (int)((0 - cursorMS) / 222d);
			}
			else if (cursorMS < 1333d)
			{
				this.arrowOffset = (int)((666d - cursorMS) / 222d);
			}
		}

		public override void UpdateInput(GameTime gameTime)
		{
			if (Input.Mouse.IsLeftButtonClicked())
			{
				this.Exit();
			}
		}

		public override void Draw(GameTime gameTime)
		{
			this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			this.SpriteBatch.Draw(GameMain.SysTexture, this.frameDestRect, this.frameSrcRect, this.Brush);

			for (int i = 0; i < 8; i++)
			{
				this.SpriteBatch.DrawString(GameMain.SmallFont, this.attrs[i], this.attrLocations[i], this.Brush);

				if (this.elapsedTime.TotalMilliseconds > 100d)
				{
					int n = (int)((this.elapsedTime.TotalMilliseconds - 1000d) / 500d);

					if (n < this.attrUpIndexes.Count)
					{
						for (int j = 0; j < n + 1; j++)
						{
							Rectangle rect = this.arrowDestRects[this.attrUpIndexes[j]];
							rect.Y += this.arrowOffset;
							this.SpriteBatch.Draw(GameMain.SysTexture, rect, this.arrowSrcRect, this.Brush);
							this.SpriteBatch.DrawString(GameMain.SmallFont, this.attrUpStrs[this.attrUpIndexes[j]], this.attrLocations[this.attrUpIndexes[j]].Expand(21 * GameMain.TextureScale, 0), this.blue);
						}
					}
				}
			}

			this.SpriteBatch.End();
		}

		public override void Exit()
		{
			base.Exit();

			if (!this.isAttrAdded)
			{
				this.actor.LV += 1;
				this.actor.Basics.HP += this.attrUps[0];
				this.actor.Basics.STR += this.attrUps[1];
				this.actor.Basics.MAG += this.attrUps[2];
				this.actor.Basics.SKL += this.attrUps[3];
				this.actor.Basics.SPD += this.attrUps[4];
				this.actor.Basics.LUK += this.attrUps[5];
				this.actor.Basics.DEF += this.attrUps[6];
				this.actor.Basics.BLD += this.attrUps[7];
				this.isAttrAdded = true;
			}
		}
	}
}
