using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGameLibrary;
using System.Collections.Generic;

namespace MyFirstSRPG.SRPGGame.GameScreens.SceneScreens
{
	public class PhaseScreen : GameScreen
	{
		private SceneScreen scene;
		private TurnPhase phase;
		private Rectangle srcRect;
		private List<Rectangle> destRects;
		private Vector2 location;
		private TimeSpan elapsedTime;
		private string text;

		public PhaseScreen(SceneScreen scene, TurnPhase phase)
		{
			this.scene = scene;
			this.phase = phase;
			this.TransitionOnTime = TimeSpan.FromSeconds(1d);
			this.TransitionOffTime = TimeSpan.FromSeconds(1d);
			this.IsPopup = true;
		}

		public override void LoadContent()
		{
			switch (this.phase)
			{
				case TurnPhase.PlayerPhase:
					this.srcRect = new Rectangle(72, 369, 56, 24);
					this.text = GameMain.SysText["PlayerTurn"];
					break;
				case TurnPhase.EnemyPhase:
					this.srcRect = new Rectangle(128, 369, 56, 24);
					this.text = GameMain.SysText["EnemyTurn"];
					break;
				case TurnPhase.AllyPhase:
					this.srcRect = new Rectangle(72, 369, 56, 24);
					this.text = GameMain.SysText["AllyTurn"];
					break;
			}

			int x = 0;
			int y = (GameMain.ScreenBounds.Height - 2 * this.srcRect.Height * GameMain.TextureScale) / 2;
			int width = this.srcRect.Width * GameMain.TextureScale;
			int height = this.srcRect.Height * GameMain.TextureScale;
			this.destRects = new List<Rectangle>();

			while (x < GameMain.ScreenBounds.Width)
			{
				this.destRects.Add(new Rectangle(x, y, width, height));
				this.destRects.Add(new Rectangle(x, y + height, width, height));
				x += width;
			}

			Vector2 size = GameMain.NormalFont.MeasureString(this.text);
			this.location.X = (GameMain.ScreenBounds.Width - size.X) / 2;
			this.location.Y = (GameMain.ScreenBounds.Height - size.Y) / 2;
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

		public override void UpdateInput(GameTime gameTime)
		{
			if (this.Status.In(ScreenStatus.TransitionOn, ScreenStatus.Active))
			{
				if (Input.Mouse.IsMouseOver(GameMain.SafeArea) && Input.Mouse.IsLeftButtonClicked())
				{
					this.Exit();
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			foreach (var rect in this.destRects)
			{
				this.SpriteBatch.Draw(GameMain.FrameTexture, rect, this.srcRect, this.Brush * 0.5f);
			}

			this.SpriteBatch.End();

			this.SpriteBatch.Begin();

			this.SpriteBatch.DrawString(GameMain.NormalFont, this.text, this.location, this.Brush);

			this.SpriteBatch.End();
		}
	}
}