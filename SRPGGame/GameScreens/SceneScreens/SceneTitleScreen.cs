using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyFirstSRPG.SRPGGame.GameScreens.SceneScreens
{
	public class SceneTitleScreen : GameScreen
	{
		private SceneScreen scene;
		private Rectangle bgSrcRect;
		private Rectangle srcRect;
		private Rectangle destRect;
		private Vector2 location;
		private TimeSpan elapsedTime;

		public SceneTitleScreen(SceneScreen scene)
		{
			this.scene = scene;
			this.TransitionOnTime = TimeSpan.FromSeconds(1d);
			this.TransitionOffTime = TimeSpan.FromSeconds(1d);
			this.IsPopup = true;
			this.OnScreenRemoved += this.SceneTitleScreen_OnScreenRemoved;
		}

		public override void LoadContent()
		{
			this.bgSrcRect = new Rectangle(84, 16, 1, 1);
			this.srcRect = new Rectangle(0, 156, 208, 48);
			this.destRect.Width = this.srcRect.Width * GameMain.TextureScale;
			this.destRect.Height = this.srcRect.Height * GameMain.TextureScale;
			this.destRect.X = (GameMain.ScreenBounds.Width - this.destRect.Width) / 2;
			this.destRect.Y = (GameMain.ScreenBounds.Height - this.destRect.Height) * 5 / 11;
			Vector2 titleSize = GameMain.NormalFont.MeasureString(this.scene.Title);
			this.location.X = (GameMain.ScreenBounds.Width - titleSize.X) / 2;
			this.location.Y = (GameMain.ScreenBounds.Height - titleSize.Y) * 5 / 11;
		}

		public override void Update(GameTime gameTime, bool isCovered, bool isFocusLost)
		{
			base.Update(gameTime, isCovered, isFocusLost);

			this.Brush = Color.White * this.SmoothTransitionOffset;

			if (this.Status == ScreenStatus.Active)
			{
				this.elapsedTime += gameTime.ElapsedGameTime;

				if (this.elapsedTime >= TimeSpan.FromSeconds(2))
				{
					this.Exit();
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			this.SpriteBatch.Draw(GameMain.SysTexture, GameMain.ScreenBounds, this.bgSrcRect, this.Brush * 0.7f);
			this.SpriteBatch.Draw(GameMain.SysTexture, this.destRect, this.srcRect, this.Brush);

			this.SpriteBatch.End();

			this.SpriteBatch.Begin();

			this.SpriteBatch.DrawString(GameMain.NormalFont, this.scene.Title, this.location, this.Brush);

			this.SpriteBatch.End();
		}

		private void SceneTitleScreen_OnScreenRemoved(object sender, EventArgs e)
		{
			this.OnScreenRemoved -= this.SceneTitleScreen_OnScreenRemoved;
			this.scene.StartScene();
		}
	}
}