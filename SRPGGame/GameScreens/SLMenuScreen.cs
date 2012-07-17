using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyFirstSRPG.SRPGGame.GameScreens
{
	public class SLMenuScreen : MenuScreen
	{
		private GameSaveSlot[] saveSlots;
		private Rectangle frameSrcRect;
		private Rectangle frameDestRect;
		private Rectangle confirmDestRect;
		private Rectangle emptySlotSrcRect;
		private Rectangle slotSrcRect;
		private Rectangle slotDestRect;
		private bool showConfirm;
		private MenuOption[] confirmOptions;

		public SLMenuScreen(string title)
		{
			this.Options.Add(new MenuOption(title));
		}

		public override void LoadContent()
		{
			base.LoadContent();

			this.saveSlots = this.GetSaveSlots();

			foreach (var saveSlot in this.saveSlots)
			{
				this.Options.Add(new MenuOption(saveSlot.Title)
				{
					Action = () =>
					{
						this.showConfirm = true;
					}
				});
			}

			this.confirmOptions = new MenuOption[2];
			this.confirmOptions[0] = new MenuOption(GameMain.SysText["OK"]);
			this.confirmOptions[1] = new MenuOption(GameMain.SysText["Cancel"])
			{
				Action = () =>
				{
					this.showConfirm = false;
				}
			};

			this.frameSrcRect = new Rectangle(0, 204, 128, 32);
			this.frameDestRect.Width = this.frameSrcRect.Width * GameMain.TextureScale;
			this.frameDestRect.Height = this.frameSrcRect.Height * GameMain.TextureScale;
			this.frameDestRect.X = (GameMain.ScreenBounds.Width - this.frameDestRect.Width) / 2;
			this.frameDestRect.Y = GameMain.ScreenBounds.Height / 2 - 176;
			this.confirmDestRect = this.frameDestRect;
			this.confirmDestRect.Y = GameMain.ScreenBounds.Height / 2 + 128;
			this.confirmOptions[0].DestRect =
				this.confirmOptions[1].DestRect = this.confirmDestRect;
			this.confirmOptions[0].DestRect.Width = (int)GameMain.NormalFont.MeasureString(this.confirmOptions[0].Title).X;
			this.confirmOptions[1].DestRect.Width = (int)GameMain.NormalFont.MeasureString(this.confirmOptions[1].Title).X;
			this.confirmOptions[0].DestRect.X = this.confirmDestRect.X + (this.confirmDestRect.Width / 2 - this.confirmOptions[0].DestRect.Width) / 2;
			this.confirmOptions[1].DestRect.X = GameMain.ScreenBounds.Width / 2 + (this.confirmDestRect.Width / 2 - this.confirmOptions[1].DestRect.Width) / 2;

			this.emptySlotSrcRect = new Rectangle(0, 92, 208, 32);
			this.slotSrcRect = new Rectangle(0, 124, 208, 32);
			this.slotDestRect.Width = this.slotSrcRect.Width * GameMain.TextureScale;
			this.slotDestRect.Height = this.slotSrcRect.Height * GameMain.TextureScale;
			this.slotDestRect.X = (GameMain.ScreenBounds.Width - this.slotDestRect.Width) / 2;
			this.slotDestRect.Y = GameMain.ScreenBounds.Height / 2 - 80;

			this.CursorSrcRect = new Rectangle(116, 0, 11, 10);
			this.CursorDestRect.Width = this.CursorSrcRect.Width * GameMain.TextureScale;
			this.CursorDestRect.Height = this.CursorSrcRect.Height * GameMain.TextureScale;
		}

		public override void UpdateInput(GameTime gameTime)
		{
			MenuOption selectedOption = null;
			IEnumerable<MenuOption> options;

			if (this.showConfirm)
			{
				options = this.confirmOptions;
			}
			else
			{
				options = this.Options.Skip(1);
			}

			foreach (var option in options)
			{
				if (Input.Mouse.IsMouseOver(option.DestRect))
				{
					if (Input.Mouse.IsLeftButtonClicked())
					{
						if (option.Action != null)
						{
							option.Action();
							GameMain.PlaySfx(GameMain.SysSfx[1]);
							//this.SelectedOption = null;
						}

						return;
					}

					selectedOption = option;
				}
			}

			if (selectedOption == null && Input.Mouse.IsLeftButtonClicked())
			{
				this.ScreenManager.AddScreen(new MainMenuScreen());
				this.Exit();
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

			var titleOption = this.Options.First();
			this.SpriteBatch.Draw(GameMain.SysTexture, titleOption.DestRect, this.frameSrcRect, Color.White);

			foreach (var option in this.Options.Skip(1))
			{
				if (option.Title == GameSaveSlot.Empty.Title)
				{
					this.SpriteBatch.Draw(GameMain.SysTexture, option.DestRect, this.emptySlotSrcRect, Color.White);
				}
				else
				{
					this.SpriteBatch.Draw(GameMain.SysTexture, option.DestRect, this.slotSrcRect, Color.White);
				}
			}

			if (this.showConfirm)
			{
				this.SpriteBatch.Draw(GameMain.SysTexture, this.confirmDestRect, this.frameSrcRect, Color.White);
			}

			if (this.SelectedOption != null)
			{
				this.CursorDestRect.Y = this.SelectedOption.DestRect.Y + 13 * GameMain.TextureScale;
				this.SpriteBatch.Draw(GameMain.SysTexture, this.CursorDestRect, this.CursorSrcRect, Color.White);
			}

			this.SpriteBatch.End();

			this.SpriteBatch.Begin();

			Vector2 location;

			foreach (var option in this.Options)
			{
				this.SpriteBatch.DrawString(GameMain.NormalFont, option.Title, option.GetStringLocation(), Color.White);
			}

			if (this.showConfirm)
			{
				foreach (var option in this.confirmOptions)
				{
					location = option.GetStringLocation();
					this.SpriteBatch.DrawString(GameMain.NormalFont, option.Title, location, Color.White);
				}
			}

			this.SpriteBatch.End();
		}

		private GameSaveSlot[] GetSaveSlots()
		{
			GameSaveSlot[] saves = new GameSaveSlot[3]
			{
				new GameSaveSlot("Fake Save Slot"),
				GameSaveSlot.Empty,
				GameSaveSlot.Empty,
			};

			return saves;
		}

		protected override void UpdateOptionsLocation()
		{
			//float transitionOffset = (float)Math.Sqrt(this.TransitionOffset);
			float transitionOffset = this.SmoothTransitionOffset;
			Rectangle destRect = this.frameDestRect;

			switch (this.Status)
			{
				case ScreenStatus.TransitionOn:
					destRect.X = this.frameDestRect.X - (int)(transitionOffset * GameMain.ScreenBounds.Width) + GameMain.ScreenBounds.Width;
					break;
				case ScreenStatus.TransitionOff:
					destRect.X = this.frameDestRect.X + (int)((1 - transitionOffset) * GameMain.ScreenBounds.Width);
					break;
				case ScreenStatus.Active:
					destRect.X = this.frameDestRect.X;
					break;
				case ScreenStatus.Hidden:
					destRect.X = this.frameDestRect.X + GameMain.ScreenBounds.Width;
					break;
			}

			this.Options.First().DestRect = destRect;
			destRect = this.slotDestRect;

			foreach (var option in this.Options.Skip(1))
			{
				switch (this.Status)
				{
					case ScreenStatus.TransitionOn:
						destRect.X = this.slotDestRect.X - (int)(transitionOffset * GameMain.ScreenBounds.Width) + GameMain.ScreenBounds.Width;
						break;
					case ScreenStatus.TransitionOff:
						destRect.X = this.slotDestRect.X + (int)((1 - transitionOffset) * GameMain.ScreenBounds.Width);
						break;
					case ScreenStatus.Active:
						destRect.X = this.slotDestRect.X;
						break;
					case ScreenStatus.Hidden:
						destRect.X = this.slotDestRect.X + GameMain.ScreenBounds.Width;
						break;
				}

				option.DestRect = destRect;
				destRect.Y += this.slotDestRect.Height;
			}
		}

		protected override void UpdateCursorLocation(GameTime gameTime)
		{
			if (this.SelectedOption == null)
			{
				return;
			}

			this.CursorTime += gameTime.ElapsedGameTime;
			int x;

			if (this.CursorTime.TotalMilliseconds < GameMain.MSPF * 15)
			{
				x = (int)(this.CursorTime.TotalMilliseconds / (GameMain.MSPF * 5));
				this.CursorDestRect.X = this.SelectedOption.DestRect.X - this.CursorDestRect.Width - x * 2 - 2;
			}
			else if (this.CursorTime.TotalMilliseconds < GameMain.MSPF * 30)
			{
				x = (int)((this.CursorTime.TotalMilliseconds - GameMain.MSPF * 15) / (GameMain.MSPF * 5));
				this.CursorDestRect.X = this.SelectedOption.DestRect.X - this.CursorDestRect.Width + x * 2 - 8;
			}
			else
			{
				this.CursorTime = TimeSpan.Zero;
			}
		}
	}

	public class GameSaveSlot
	{
		public static readonly GameSaveSlot Empty = new GameSaveSlot("Empty Save Slot");

		public string Title;

		public GameSaveSlot(string title)
		{
			this.Title = title;
		}
	}
}