using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyFirstSRPG.SRPGGame.GameScreens
{
	public abstract class MenuScreen : GameScreen
	{
		protected List<MenuOption> Options;
		protected MenuOption SelectedOption;
		protected Rectangle CursorSrcRect;
		protected Rectangle CursorDestRect;
		protected TimeSpan CursorTime;

		public MenuScreen()
		{
			this.TransitionOnTime = TimeSpan.FromMilliseconds(500d);
			this.TransitionOffTime = TimeSpan.FromMilliseconds(500d);
			this.Options = new List<MenuOption>();
		}

		public override void LoadContent()
		{
		}

		public override void Update(GameTime gameTime, bool isCovered, bool isFocusLost)
		{
			base.Update(gameTime, isCovered, isFocusLost);

			this.UpdateOptionsLocation();
			this.UpdateCursorLocation(gameTime);
		}

		protected abstract void UpdateOptionsLocation();

		protected abstract void UpdateCursorLocation(GameTime gameTime);
	}

	public class MenuOption
	{
		public string Title;
		public Rectangle DestRect;
		public Action Action;
		public bool Enabled;

		private Vector2 titleSize;

		public MenuOption(string title)
		{
			this.Title = title;
		}

		public Vector2 GetStringLocation()
		{
			if (this.titleSize == Vector2.Zero)
			{
				this.titleSize = GameMain.NormalFont.MeasureString(this.Title);
			}

			return new Vector2(this.DestRect.Location.X + (this.DestRect.Width - this.titleSize.X) / 2, this.DestRect.Location.Y + (this.DestRect.Height - this.titleSize.Y) / 2);
		}
	}
}