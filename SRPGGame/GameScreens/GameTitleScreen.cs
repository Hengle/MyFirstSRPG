using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyFirstSRPG.SRPGGame.GameScreens
{
	public class GameTitleScreen : GameScreen
	{
		private Texture2D texture;

		public GameTitleScreen()
		{
			this.TransitionOnTime = TimeSpan.FromMilliseconds(500d);
			this.TransitionOffTime = TimeSpan.FromMilliseconds(500d);
		}

		public override void LoadContent()
		{
			this.texture = this.Content.Load<Texture2D>("Texture/GameTitleScreen");
		}

		public override void UnloadContent()
		{
			this.texture = null;
		}

		public override void Update(GameTime gameTime, bool isCovered, bool isFocusLost)
		{
			base.Update(gameTime, isCovered, isFocusLost);

			this.Brush = Color.White * this.SmoothTransitionOffset;
		}

		public override void Draw(GameTime gameTime)
		{
			this.SpriteBatch.Begin();
			this.SpriteBatch.Draw(this.texture, Vector2.Zero, this.Brush);
			this.SpriteBatch.End();
		}
	}
}