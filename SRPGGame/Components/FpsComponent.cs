using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyFirstSRPG.SRPGGame.Components
{
	public class FpsComponent : DrawableGameComponent
	{
		private new GameMain Game;
		private Vector2 destLocation;
		private TimeSpan elapsedTime;
		private int frameRate;
		private int frameCounter;

		public FpsComponent(GameMain game)
			: base(game)
		{
			this.Game = game;
		}

		public override void Update(GameTime gameTime)
		{
			if (this.destLocation == Vector2.Zero)
			{
				this.destLocation = new Vector2(GameMain.ScreenBounds.Right - 105, 11);
			}

			this.elapsedTime += gameTime.ElapsedGameTime;

			if (this.elapsedTime.TotalSeconds > 1d)
			{
				this.elapsedTime -= TimeSpan.FromSeconds(1d);
				this.frameRate = frameCounter;
				this.frameCounter = 0;
			}
		}

		public override void Draw(GameTime gameTime)
		{
			string text;

			if (this.Game.IsGamePaused)
			{
				text = "paused";
			}
			else
			{
				this.frameCounter++;
				text = "FPS: " + frameRate.ToString();
			}

			this.Game.SpriteBatch.Begin();

			this.Game.SpriteBatch.DrawString(GameMain.SmallFont, text, this.destLocation, Color.White);

			this.Game.SpriteBatch.End();
		}
	}
}