using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGame.Components.MapScreenLayers;

namespace MyFirstSRPG.SRPGGame.GameScreens.SceneScreens
{
	public class PopupMenuScreen : MenuScreen
	{
		public static readonly MenuOption MenuOptionCancel;
		public static readonly MenuOption MenuOptionAttack;
		public static readonly MenuOption MenuOptionItem;
		public static readonly MenuOption MenuOptionRest;
		public static readonly MenuOption MenuOptionTroop;
		public static readonly MenuOption MenuOptionBrief;
		public static readonly MenuOption MenuOptionOptions;
		public static readonly MenuOption MenuOptionSave;
		public static readonly MenuOption MenuOptionEndPhase;

		static PopupMenuScreen()
		{
			MenuOptionCancel = new MenuOption(GameMain.SysText["MenuOptionCancel"]);
			MenuOptionAttack = new MenuOption(GameMain.SysText["MenuOptionAttack"]);
			MenuOptionItem = new MenuOption(GameMain.SysText["MenuOptionItem"]);
			MenuOptionRest = new MenuOption(GameMain.SysText["MenuOptionRest"]);
			MenuOptionTroop = new MenuOption(GameMain.SysText["MenuOptionTroop"]);
			MenuOptionBrief = new MenuOption(GameMain.SysText["MenuOptionBrief"]);
			MenuOptionOptions = new MenuOption(GameMain.SysText["MenuOptionOptions"]);
			MenuOptionSave = new MenuOption(GameMain.SysText["MenuOptionSave"]);
			MenuOptionEndPhase = new MenuOption(GameMain.SysText["MenuOptionEndPhase"]);
		}

		protected ActorLayer Layer;
		protected Rectangle SrcRectHead;
		protected Rectangle DestRectHead;
		protected Rectangle SrcRectFoot;
		protected Rectangle DestRectFoot;
		protected Rectangle SrcRectBody;
		protected bool IsToLeft;

		public SceneActor CurrentActor;

		public PopupMenuScreen(ActorLayer layer, bool isToLeft)
		{
			this.Layer = layer;
			this.IsToLeft = isToLeft;
			this.TransitionOnTime =
				this.TransitionOffTime = TimeSpan.Zero;

			MenuOptionCancel.Action = this.CancelCommand;
			MenuOptionAttack.Action = this.AttackCommand;
			MenuOptionItem.Action = this.ItemCommand;
			MenuOptionRest.Action = this.RestCommand;
			MenuOptionTroop.Action = this.TroopCommand;
			MenuOptionBrief.Action = this.BriefCommand;
			MenuOptionOptions.Action = this.OptionsCommand;
			MenuOptionSave.Action = this.SaveCommand;
			MenuOptionEndPhase.Action = this.EndPhaseCommand;
		}

		public override void LoadContent()
		{
			base.LoadContent();

			this.SrcRectHead = new Rectangle(0, 81, 50, 11);
			this.DestRectHead.Width = this.SrcRectHead.Width * GameMain.TextureScale;
			this.DestRectHead.Height = this.SrcRectHead.Height * GameMain.TextureScale;

			this.SrcRectFoot = new Rectangle(50, 81, 50, 9);
			this.DestRectFoot.Width = this.SrcRectFoot.Width * GameMain.TextureScale;
			this.DestRectFoot.Height = this.SrcRectFoot.Height * GameMain.TextureScale;

			this.SrcRectBody = new Rectangle(50, 91, 50, 1);

			if (this.IsToLeft)
			{
				this.DestRectHead.X =
					this.DestRectFoot.X = GameMain.ScreenBounds.Left + 8 * GameMain.TextureScale;
			}
			else
			{
				this.DestRectHead.X =
					this.DestRectFoot.X = GameMain.ScreenBounds.Right - this.DestRectHead.Width - 8 * GameMain.TextureScale;
			}

			this.CursorSrcRect = new Rectangle(116, 0, 11, 10);
			this.CursorDestRect.Width = this.CursorSrcRect.Width * GameMain.TextureScale;
			this.CursorDestRect.Height = this.CursorSrcRect.Height * GameMain.TextureScale;

			GameMain.PlaySfx(GameMain.SysSfx[3]);
		}

		public override void UpdateInput(GameTime gameTime)
		{
			if (this.Options.Count == 0)
			{
				return;
			}

			MenuOption selectedOption = null;

			foreach (var option in this.Options)
			{
				if (Input.Mouse.IsMouseOver(option.DestRect))
				{
					if (Input.Mouse.IsLeftButtonClicked())
					{
						if (option.Action != null)
						{
							option.Action();
							GameMain.PlaySfx(GameMain.SysSfx[1]);
							this.SelectedOption = null;
						}

						return;
					}

					selectedOption = option;
				}
			}

			if (selectedOption == null && Input.Mouse.IsLeftButtonClicked())
			{
				this.CancelCommand();
			}

			if (this.SelectedOption != selectedOption)
			{
				this.SelectedOption = selectedOption;
				GameMain.PlaySfx(GameMain.SysSfx[0]);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			if (this.Options.Count == 0)
			{
				return;
			}

			this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			this.SpriteBatch.Draw(GameMain.SysTexture, this.DestRectHead, this.SrcRectHead, Color.White);

			foreach (var option in this.Options.Where(o => o.Enabled))
			{
				this.SpriteBatch.Draw(GameMain.SysTexture, option.DestRect, this.SrcRectBody, Color.White);
				this.SpriteBatch.DrawString(GameMain.NormalFont, option.Title, option.GetStringLocation(), Color.Black);

				if (this.SelectedOption == option)
				{
					this.SpriteBatch.DrawString(GameMain.NormalFont, option.Title, option.GetStringLocation(), Color.Orange);
				}
				else
				{
					this.SpriteBatch.DrawString(GameMain.NormalFont, option.Title, option.GetStringLocation(), Color.White);
				}
			}

			this.SpriteBatch.Draw(GameMain.SysTexture, this.DestRectFoot, this.SrcRectFoot, Color.White);

			if (this.SelectedOption != null)
			{
				this.CursorDestRect.Y = this.SelectedOption.DestRect.Y + 2 * GameMain.TextureScale;
				this.SpriteBatch.Draw(GameMain.SysTexture, this.CursorDestRect, this.CursorSrcRect, Color.White);
			}

			this.SpriteBatch.End();
		}

		protected override void UpdateOptionsLocation()
		{
			Rectangle rect = new Rectangle(this.DestRectHead.X, 0, this.SrcRectBody.Width * GameMain.TextureScale, GameMain.NormalFont.LineSpacing);
			int optionHeight = this.Options.Count(o => o.Enabled) * rect.Height;
			this.DestRectHead.Y = (GameMain.ScreenBounds.Height - (this.DestRectHead.Height + optionHeight + this.DestRectFoot.Height)) / 3;
			rect.Y = this.DestRectHead.Bottom;

			foreach (var option in this.Options.Where(o => o.Enabled))
			{
				option.DestRect = rect;
				rect.Y += rect.Height;
			}

			this.DestRectFoot.Y = rect.Y;
		}

		protected override void UpdateCursorLocation(GameTime gameTime)
		{
			if (this.SelectedOption == null)
			{
				return;
			}

			this.CursorTime += gameTime.ElapsedGameTime;
			int optionLeft = (int)(this.SelectedOption.GetStringLocation().X);
			int x;

			if (this.CursorTime.TotalMilliseconds < GameMain.MSPF * 15)
			{
				x = (int)(this.CursorTime.TotalMilliseconds / (GameMain.MSPF * 5));
				this.CursorDestRect.X = optionLeft - this.CursorDestRect.Width - x - 1;
			}
			else if (this.CursorTime.TotalMilliseconds < GameMain.MSPF * 30)
			{
				x = (int)((this.CursorTime.TotalMilliseconds - GameMain.MSPF * 15) / (GameMain.MSPF * 5));
				this.CursorDestRect.X = optionLeft - this.CursorDestRect.Width + x - 4;
			}
			else
			{
				this.CursorTime = TimeSpan.Zero;
			}
		}

		public void AddOptions(params MenuOption[] options)
		{
			foreach (var option in options)
			{
				option.Enabled = true;

				if (!this.Options.Contains(option))
				{
					this.Options.Add(option);
				}
			}
		}

		#region menuScreen opion actions

		private void CancelCommand()
		{
			GameMain.PlaySfx(GameMain.SysSfx[3]);

			if ((PlayStatus)this.Layer.StateManager.CurrentState.Status == PlayStatus.PlayerInteract)
			{
				this.CurrentActor.MapPoint = this.CurrentActor.LastMapPoint;
				this.Layer.StateManager.ChangeState(PlayStatus.PlayerSelected);
			}

			this.Exit();
		}

		private void AttackCommand()
		{
			this.Layer.StateManager.ChangeState(PlayStatus.PlayerAim);
			this.Exit();
		}

		private void ItemCommand()
		{
		}

		private void RestCommand()
		{
			if (this.CurrentActor != null)
			{
				this.CurrentActor.StateManager.ChangeState(ActorStatus.Rest);
				this.CurrentActor = null;
			}

			this.Layer.StateManager.ChangeState(PlayStatus.Explore);
			this.Exit();
		}

		private void TroopCommand()
		{
		}

		private void BriefCommand()
		{
		}

		private void OptionsCommand()
		{
		}

		private void SaveCommand()
		{
		}

		private void EndPhaseCommand()
		{
			this.Layer.Scene.EndPhase();
			this.Exit();
		}

		#endregion menuScreen opion actions
	}
}