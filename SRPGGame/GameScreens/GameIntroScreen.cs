using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyFirstSRPG.SRPGGame.GameScreens
{
	public class GameIntroScreen : GameScreen
	{
		private Texture2D texture;

		public GameIntroScreen()
		{
			this.TransitionOnTime = TimeSpan.FromMilliseconds(500d);
			this.TransitionOffTime = TimeSpan.FromMilliseconds(500d);
		}

		public override void LoadContent()
		{
			this.texture = this.Content.Load<Texture2D>("Texture/GameIntroScreen");
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

		public override void UpdateInput(GameTime gameTime)
		{
			if (Input.Mouse.IsLeftButtonClicked() || Input.Keyboard.IsAnyKeyPressed(Keys.Escape, Keys.Space, Keys.Enter))
			{
				this.ScreenManager.AddScreen(new GameTitleScreen());
				this.ScreenManager.AddScreen(new MainMenuScreen());
				GameMain.PlaySfx(GameMain.SysSfx[1]);
				this.Exit();
			}
		}

		public override void Draw(GameTime gameTime)
		{
			this.SpriteBatch.Begin();
			this.SpriteBatch.Draw(this.texture, Vector2.Zero, this.Brush);
			this.SpriteBatch.End();
		}
	}
}