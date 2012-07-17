using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGame.GameScreens.SceneScreens;

namespace MyFirstSRPG.SRPGGame.GameScreens
{
	public class MainMenuScreen : MenuScreen
	{
		private Rectangle frameSrcRect;
		private Rectangle frameDestRect;

		public MainMenuScreen()
		{
		}

		public override void LoadContent()
		{
			base.LoadContent();

			MenuOption option;

			if (this.IsGameSaved())
			{
				option = new MenuOption(GameMain.SysText["ContinueGameMenuOption"]);

				option.Action = () =>
				{
					this.ScreenManager.AddScreen(new SLMenuScreen(option.Title));
					this.Exit();
				};

				this.Options.Add(option);
			}

			option = new MenuOption(GameMain.SysText["NewGameMenuOption"]);

			option.Action = () =>
			{
				this.ScreenManager.AddScreen(new SceneScreen(new Scene01()));
				this.Exit();
			};

			this.Options.Add(option);

			this.frameSrcRect = new Rectangle(0, 204, 128, 32);
			this.frameDestRect.Width = this.frameSrcRect.Width * GameMain.TextureScale;
			this.frameDestRect.Height = this.frameSrcRect.Height * GameMain.TextureScale;
			this.frameDestRect.X = (GameMain.ScreenBounds.Width - this.frameDestRect.Width) / 2;
			this.frameDestRect.Y = (GameMain.ScreenBounds.Height - this.frameDestRect.Height * this.Options.Count) / 2;

			this.CursorSrcRect = new Rectangle(116, 0, 11, 10);
			this.CursorDestRect.Width = this.CursorSrcRect.Width * GameMain.TextureScale;
			this.CursorDestRect.Height = this.CursorSrcRect.Height * GameMain.TextureScale;
		}

		public override void UpdateInput(GameTime gameTime)
		{
			MenuOption selectedOption = null;

			foreach (var option in this.Options)
			{
				if (Input.Mouse.IsMouseOver(option.DestRect))
				{
					if (Input.Mouse.IsLeftButtonClicked())
					{
						if (option.Action != null)
						{
							GameMain.PlaySfx(GameMain.SysSfx[1]);
							this.SelectedOption = null;
							option.Action();
						}

						return;
					}

					selectedOption = option;
				}
			}

			if (this.SelectedOption != selectedOption)
			{
				this.SelectedOption = selectedOption;
				GameMain.PlaySfx(GameMain.SysSfx[0]);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			if (this.SelectedOption != null)
			{
				this.CursorDestRect.Y = this.SelectedOption.DestRect.Y + 13 * GameMain.TextureScale;
				this.SpriteBatch.Draw(GameMain.SysTexture, this.CursorDestRect, this.CursorSrcRect, Color.White);
			}

			foreach (var option in this.Options)
			{
				this.SpriteBatch.Draw(GameMain.SysTexture, option.DestRect, this.frameSrcRect, Color.White);
				this.SpriteBatch.DrawString(GameMain.NormalFont, option.Title, option.GetStringLocation(), Color.White);
			}

			this.SpriteBatch.End();
		}

		protected override void UpdateOptionsLocation()
		{
			//float transitionOffset = (float)Math.Sqrt(this.TransitionOffset);
			float transitionOffset = this.SmoothTransitionOffset;
			Rectangle destRect = this.frameDestRect;

			foreach (var option in this.Options)
			{
				switch (this.Status)
				{
					case ScreenStatus.TransitionOn:
						destRect.X = this.frameDestRect.X + (int)(transitionOffset * GameMain.ScreenBounds.Width) - GameMain.ScreenBounds.Width;
						break;
					case ScreenStatus.TransitionOff:
						destRect.X = this.frameDestRect.X - (int)((1 - transitionOffset) * GameMain.ScreenBounds.Width);
						break;
					case ScreenStatus.Active:
						destRect.X = this.frameDestRect.X;
						break;
					case ScreenStatus.Hidden:
						destRect.X = this.frameDestRect.X - GameMain.ScreenBounds.Width;
						break;
				}

				option.DestRect = destRect;
				destRect.Y += this.frameDestRect.Height;
			}
		}

		protected override void UpdateCursorLocation(GameTime gameTime)
		{
			this.CursorTime += gameTime.ElapsedGameTime;
			int x;

			if (this.CursorTime.TotalMilliseconds < GameMain.MSPF * 15)
			{
				x = (int)(this.CursorTime.TotalMilliseconds / (GameMain.MSPF * 5));
				this.CursorDestRect.X = this.frameDestRect.X - this.CursorDestRect.Width - x - 1;
			}
			else if (this.CursorTime.TotalMilliseconds < GameMain.MSPF * 30)
			{
				x = (int)((this.CursorTime.TotalMilliseconds - GameMain.MSPF * 15) / (GameMain.MSPF * 5));
				this.CursorDestRect.X = this.frameDestRect.X - this.CursorDestRect.Width + x - 4;
			}
			else
			{
				this.CursorTime = TimeSpan.Zero;
			}
		}

		private bool IsGameSaved()
		{
			// checks save state;
			return true;
		}
	}
}