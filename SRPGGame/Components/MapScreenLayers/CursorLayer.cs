using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGame.Animations;
using MyFirstSRPG.SRPGGame.GameScreens.SceneScreens;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGame.Components.MapScreenLayers
{
	public class CursorLayer : MapScreenLayer
	{
		private FrameAnimation cursorAnimation;
		private Point mapPoint;
		private Point lastMapCell;

		private Rectangle terrainTipSrcRect;
		private Rectangle terrainTipDestRect;
		private Terrain currentTerrain;
		private Rectangle[] actorTipSrcRects;
		private Rectangle actorTipDestRect;
		private int actorTipIndex;
		private SceneActor actor;

		public CursorLayer(MapScreen screen)
			: base(screen)
		{
			this.cursorAnimation = new FrameAnimation()
			{
				Texture = GameMain.SysTexture,
				TextureRectangle = new Rectangle(0, 0, 21, 21),
				FrameSkip = 20,
				FrameCount = 2,
				DestOffest = new Point(-2, -2),
				CameraFollowing = true
			};

			this.terrainTipSrcRect = new Rectangle(37, 236, 72, 35);
			this.terrainTipDestRect.Width = this.terrainTipSrcRect.Width * GameMain.TextureScale;
			this.terrainTipDestRect.Height = this.terrainTipSrcRect.Height * GameMain.TextureScale;
			this.terrainTipDestRect.Y = 4 * GameMain.TextureScale;

			this.actorTipIndex = -1;
			this.actorTipSrcRects = new Rectangle[3];
			this.actorTipSrcRects[0] =
				this.actorTipSrcRects[1] =
				this.actorTipSrcRects[2] = new Rectangle(128, 0, 72, 32);
			this.actorTipSrcRects[0].Y = 204;
			this.actorTipSrcRects[1].Y = 236;
			this.actorTipSrcRects[2].Y = 268;
			this.actorTipDestRect.Width = this.actorTipSrcRects[0].Width * GameMain.TextureScale;
			this.actorTipDestRect.Height = this.actorTipSrcRects[0].Height * GameMain.TextureScale;
		}

		public override void UpdateInput(GameTime gameTime)
		{
			if (this.Map.IsActive)
			{
				if (Input.Mouse.IsMouseOver(GameMain.SafeArea))
				{
					this.UpdateCursorLocation(gameTime, Input.Mouse.State.X, Input.Mouse.State.Y);
					this.cursorAnimation.Update(gameTime);
				}
				else
				{
					this.MoveCamera(gameTime);
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			if (this.Map.IsActive)
			{
				this.Scene.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

				this.cursorAnimation.Draw(this.Scene.SpriteBatch, gameTime);
				this.DrawTerrainTip(this.Scene.SpriteBatch);
				this.DrawActorTip(this.Scene.SpriteBatch);

				this.Scene.SpriteBatch.End();
			}
		}

		private void UpdateCursorLocation(GameTime gameTime, int x, int y)
		{
			this.mapPoint = GameMain.GetMapPoint(x, y);

			if (this.mapPoint != this.lastMapCell)
			{
				GameMain.PlaySfx(this.Map.CusorSfx);
				this.lastMapCell = this.mapPoint;
				this.UpdateTipLocation();
			}

			this.cursorAnimation.DestRectangle.X = this.mapPoint.X * GameMain.CellSize.Width;
			this.cursorAnimation.DestRectangle.Y = this.mapPoint.Y * GameMain.CellSize.Height;
			this.cursorAnimation.LastDestRectangle = this.cursorAnimation.DestRectangle;
		}

		private void UpdateTipLocation()
		{
			bool isToLeft = (mapPoint.X + GameMain.Camera.Location.X / GameMain.CellSize.Width) > this.Scene.MapSize.Width / 2;

			if (isToLeft)
			{
				this.terrainTipDestRect.X = 4 * GameMain.TextureScale;
			}
			else
			{
				this.terrainTipDestRect.X = GameMain.ScreenBounds.Width - this.terrainTipDestRect.Width - 4 * GameMain.TextureScale;
			}

			this.currentTerrain = this.Scene.Terrains[this.mapPoint.X, this.mapPoint.Y];

			this.actorTipDestRect.Location = MapScreen.GetScreenLocation(this.mapPoint).Expand(-28 * GameMain.TextureScale, -42 * GameMain.TextureScale);
			this.actorTipIndex = -1;
			this.actor = this.Scene.FindActor(this.mapPoint);

			if (this.actor != null)
			{
				switch (this.actor.Faction)
				{
					case Faction.Player:
						this.actorTipIndex = 0;
						break;
					case Faction.Enemy:
						this.actorTipIndex = 1;
						break;
					case Faction.Ally:
						this.actorTipIndex = 2;
						break;
				}
			}
		}

		private void MoveCamera(GameTime gameTime)
		{
			if (GameMain.Camera.IsMoving)
			{
				GameMain.Camera.Move(gameTime);
			}
			else
			{
				Direction direction = Direction.Unknown;

				if (Input.Mouse.State.X < GameMain.SafeArea.Left + GameMain.CellSize.Width)
				{
					direction = direction | Direction.Left;
				}
				else if (Input.Mouse.State.X > GameMain.SafeArea.Right - GameMain.CellSize.Width)
				{
					direction = direction | Direction.Right;
				}

				if (Input.Mouse.State.Y < GameMain.SafeArea.Top + GameMain.CellSize.Height)
				{
					direction = direction | Direction.Up;
				}
				else if (Input.Mouse.State.Y > GameMain.SafeArea.Bottom - GameMain.CellSize.Height)
				{
					direction = direction | Direction.Down;
				}

				GameMain.Camera.Move(gameTime, direction);
			}

			GameMain.Camera.Lock();
		}

		private void DrawTerrainTip(SpriteBatch sb)
		{
			if (string.IsNullOrEmpty(this.currentTerrain.Name))
			{
				return;
			}

			Vector2 location = this.terrainTipDestRect.Location.Expand(20, 8).ToVector2();
			sb.Draw(GameMain.SysTexture, this.terrainTipDestRect, this.terrainTipSrcRect, Color.White);
			sb.DrawString(GameMain.NormalFont, this.currentTerrain.Name + string.Format(" {0},{1}", this.mapPoint.X, this.mapPoint.Y), location, Color.White);
			sb.DrawString(GameMain.SmallFont, this.currentTerrain.DEF.ToString(), location.Expand(21f, 30f), Color.Black);
			sb.DrawString(GameMain.SmallFont, this.currentTerrain.Avoid.ToString(), location.Expand(72f, 30f), Color.Black);
		}

		private void DrawActorTip(SpriteBatch sb)
		{
			if (this.actorTipIndex > -1)
			{
				Vector2 location = this.actorTipDestRect.Location.Expand(12, 6).ToVector2();
				sb.Draw(GameMain.SysTexture, this.actorTipDestRect, this.actorTipSrcRects[this.actorTipIndex], Color.White);
				sb.DrawString(GameMain.NormalFont, this.actor.Name, location, Color.White);
				sb.DrawString(GameMain.SmallFont, string.Format("HP {0:d2}/{1:d2}", this.actor.CurrentHP, this.actor.ActualHP), location.Expand(17f, 26f), Color.Black);
			}
		}
	}
}