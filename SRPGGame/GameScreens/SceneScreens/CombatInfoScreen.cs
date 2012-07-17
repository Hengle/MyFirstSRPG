using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGame.Animations;
using MyFirstSRPG.SRPGGameLibrary;
using MyFirstSRPG.SRPGGame.Components.MapScreenLayers;

namespace MyFirstSRPG.SRPGGame.GameScreens.SceneScreens
{
	public class CombatInfoScreen : GameScreen
	{
		private ActorLayer layer;
		private SceneActor player;
		private SceneActor enemy;
		private Rectangle frameSrcRect;
		private Rectangle frameDestRect;
		private FrameAnimation cursorAnimation;
		private Vector2[] locations;
		private string[] labels;
		private string[] values;
		private bool isToLeft;
		private Color blue = Helper.ConvertToColor(0xffc0e8f8);

		public CombatInfoScreen(ActorLayer layer, SceneActor player, SceneActor enemy, bool isToLeft)
		{
			this.layer = layer;
			this.player = player;
			this.enemy = enemy;
			this.isToLeft = isToLeft;
		}

		public override void LoadContent()
		{
			this.frameSrcRect = new Rectangle(208, 0, 82, 210);
			this.frameDestRect.Width = this.frameSrcRect.Width * GameMain.TextureScale;
			this.frameDestRect.Height = this.frameSrcRect.Height * GameMain.TextureScale;
			this.frameDestRect.Y = (GameMain.ScreenBounds.Height - this.frameDestRect.Height) / 2 + 2 * GameMain.TextureScale;

			if (this.isToLeft)
			{
				this.frameDestRect.X = 6 * GameMain.TextureScale;
			}
			else
			{
				this.frameDestRect.X = GameMain.ScreenBounds.Right - this.frameDestRect.Width - 6 * GameMain.TextureScale;
			}

			this.cursorAnimation = new FrameAnimation()
			{
				Texture = GameMain.SysTexture,
				TextureRectangle = new Rectangle(42, 0, 21, 21),
				FrameSkip = 20,
				FrameCount = 2,
				DestOffest = new Point(-2, -2),
				CameraFollowing = true
			};
			this.cursorAnimation.SetDestLocation(this.enemy.MapPoint.X * GameMain.CellSize.Width, this.enemy.MapPoint.Y * GameMain.CellSize.Height);

			this.locations = new Vector2[3];
			this.locations[0] = new Vector2(this.frameDestRect.X + 8 * GameMain.TextureScale, this.frameDestRect.Y + 8 * GameMain.TextureScale);
			this.locations[1] = new Vector2(this.locations[0].X, this.frameDestRect.Y + 64 * GameMain.TextureScale);
			this.locations[2] = new Vector2(this.locations[0].X, this.frameDestRect.Y + 184 * GameMain.TextureScale);
			this.labels = new string[7];
			this.labels[0] = "Lv";
			this.labels[1] = "Hp";
			this.labels[2] = GameMain.SysText["LabelAttack"];
			this.labels[3] = GameMain.SysText["LabelDEF"];
			this.labels[4] = GameMain.SysText["LabelHit"];
			this.labels[5] = GameMain.SysText["LabelCritical"];
			this.labels[6] = GameMain.SysText["LabelAtkSpeed"];

			string enemyAccuracy = (this.enemy.ActualAccuracy - this.player.ActualAvoid).Clamp(1, 99).ToString().PadLeft(2);
			string playerAccuracy = (this.player.ActualAccuracy - this.enemy.ActualAvoid).Clamp(1, 99).ToString().PadLeft(6);
			this.values = new string[11];
			this.values[0] = this.enemy.Name;
			this.values[1] = this.enemy.Class.Name;
			this.values[2] = this.enemy.CurrentWeapon.Name;
			this.values[3] = this.enemy.LV.ToString().PadLeft(2) + this.player.LV.ToString().PadLeft(6);
			this.values[4] = this.enemy.CurrentHP.ToString().PadLeft(2) + this.player.CurrentHP.ToString().PadLeft(6);
			this.values[5] = this.enemy.ActualAttackPower.ToString().PadLeft(2) + this.player.ActualAttackPower.ToString().PadLeft(6);
			this.values[6] = this.enemy.ActualDEF.ToString().PadLeft(2) + this.player.ActualDEF.ToString().PadLeft(6);
			this.values[7] = enemyAccuracy + playerAccuracy;
			this.values[8] = this.enemy.ActualCritical.ToString().PadLeft(2) + this.player.ActualCritical.ToString().PadLeft(6);
			this.values[9] = this.enemy.ActualAttackSpeed.ToString().PadLeft(2) + this.player.ActualAttackSpeed.ToString().PadLeft(6);
			this.values[10] = this.player.Name.PadLeft(8, '　');
		}

		public override void Update(GameTime gameTime, bool isCovered, bool isFocusLost)
		{
			base.Update(gameTime, isCovered, isFocusLost);

			this.cursorAnimation.Update(gameTime);
		}

		public override void UpdateInput(GameTime gameTime)
		{
			if (Input.Mouse.IsLeftButtonClicked())
			{
				if (Input.Mouse.IsMouseOver(this.enemy.MapPoint.ToRectangle()))
				{
					this.layer.StateManager.ChangeState(PlayStatus.Combat);
					this.Exit();
				}
				else
				{
					this.layer.StateManager.ChangeState(PlayStatus.PlayerAim);
					this.Exit();
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			this.cursorAnimation.Draw(this.SpriteBatch, gameTime);
			this.SpriteBatch.Draw(GameMain.SysTexture, this.frameDestRect, this.frameSrcRect, Color.White);

			this.SpriteBatch.DrawString(GameMain.DigitFont, this.labels[0], this.locations[1].Expand(24 * GameMain.TextureScale, 0), Color.White, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.labels[1], this.locations[1].Expand(24 * GameMain.TextureScale, 16 * GameMain.TextureScale), Color.White, 2f);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.labels[2], this.locations[1].Expand(24 * GameMain.TextureScale, 32 * GameMain.TextureScale), Color.White);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.labels[3], this.locations[1].Expand(24 * GameMain.TextureScale, 48 * GameMain.TextureScale), Color.White);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.labels[4], this.locations[1].Expand(24 * GameMain.TextureScale, 64 * GameMain.TextureScale), Color.White);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.labels[5], this.locations[1].Expand(24 * GameMain.TextureScale, 80 * GameMain.TextureScale), Color.White);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.labels[6], this.locations[1].Expand(24 * GameMain.TextureScale, 96 * GameMain.TextureScale), Color.White);


			this.SpriteBatch.DrawString(GameMain.NormalFont, this.values[0], this.locations[0], Color.White);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.values[1], this.locations[0].Expand(0, 16 * GameMain.TextureScale), Color.White);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.values[2], this.locations[0].Expand(0, 32 * GameMain.TextureScale), Color.White);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.values[3], this.locations[1], this.blue, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.values[4], this.locations[1].Expand(0, 16 * GameMain.TextureScale), this.blue, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.values[5], this.locations[1].Expand(0, 32 * GameMain.TextureScale), this.blue, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.values[6], this.locations[1].Expand(0, 48 * GameMain.TextureScale), this.blue, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.values[7], this.locations[1].Expand(0, 64 * GameMain.TextureScale), this.blue, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.values[8], this.locations[1].Expand(0, 80 * GameMain.TextureScale), this.blue, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.values[9], this.locations[1].Expand(0, 96 * GameMain.TextureScale), this.blue, 2f);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.values[10], this.locations[2], Color.White);

			this.SpriteBatch.End();
		}
	}
}
