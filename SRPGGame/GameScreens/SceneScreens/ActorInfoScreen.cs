using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGame.Animations;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGame.GameScreens.SceneScreens
{
	public class ActorInfoScreen : GameScreen
	{
		private SceneActor actor;

		private Texture2D frameTexture;
		private Texture2D iconTexture;
		private Rectangle[] srcRects;
		private Rectangle[] destRects;
		private Rectangle topSrcRect;
		private Rectangle bottomSrcRect;
		private Rectangle scrollArea;
		private FrameAnimation[] scrollMarkAnimations;

		private ActorWaitAnimation actorAnimation;
		private Rectangle faceDestRect;
		private Point iconLocation;
		private TimeSpan animationTime;
		private Vector2[] combatLocations;
		private string[] combatLabels;
		private string[] combatValues;
		private Vector2[] attrLocations;
		private string[] attrLabels;
		private string[] attrValues;

		private Rectangle[] itemDestRects;
		private Rectangle[] perkDestRects;
		private FrameAnimation boxAnimation;
		private int selectedItemIndex = -1;
		private int selectedPerkIndex = -1;

		private TimeSpan scrollTime;
		private bool isScrolling;
		private Direction scrollDirection;

		private RasterizerState rasterizerState;
		private Color blue = Helper.ConvertToColor(0xffc0e8f8);

		public ActorInfoScreen(SceneActor actor)
		{
			this.actor = actor;
		}

		public override void LoadContent()
		{
			this.frameTexture = this.Content.Load<Texture2D>("Texture/Frame01");
			this.iconTexture = this.Content.Load<Texture2D>("Texture/Icon01");

			#region main frame

			this.srcRects = new Rectangle[4];
			this.srcRects[0] = new Rectangle(0, 0, 256, 224);
			this.srcRects[1] =
				this.topSrcRect = new Rectangle(0, 224, 256, 139);
			this.srcRects[2] =
				this.bottomSrcRect = new Rectangle(256, 224, 256, 139);
			this.srcRects[3] = new Rectangle(0, 363, 256, 6);

			this.destRects = new Rectangle[4];
			this.destRects[0].Width = this.srcRects[0].Width * GameMain.TextureScale;
			this.destRects[0].Height = this.srcRects[0].Height * GameMain.TextureScale;

			this.destRects[1].Width = this.srcRects[1].Width * GameMain.TextureScale;
			this.destRects[1].Height = this.srcRects[1].Height * GameMain.TextureScale;

			this.destRects[2].Width = this.srcRects[2].Width * GameMain.TextureScale;
			this.destRects[2].Height = this.srcRects[2].Height * GameMain.TextureScale;

			this.destRects[3].Width = this.srcRects[3].Width * GameMain.TextureScale;
			this.destRects[3].Height = this.srcRects[3].Height * GameMain.TextureScale;

			this.destRects[0].X =
				this.destRects[1].X =
				this.destRects[2].X =
				this.destRects[3].X = (GameMain.ScreenBounds.Width - this.destRects[0].Width) / 2;
			this.destRects[0].Y = (GameMain.ScreenBounds.Height - this.destRects[0].Height) / 2;
			this.destRects[1].Y = this.destRects[0].Y + 79 * GameMain.TextureScale;

			this.scrollArea = new Rectangle(this.destRects[1].X, this.destRects[1].Y, this.destRects[1].Width, this.destRects[1].Height);
			this.rasterizerState = new RasterizerState() { ScissorTestEnable = true };
			this.SpriteBatch.GraphicsDevice.ScissorRectangle = this.scrollArea;

			this.destRects[2].Y = this.destRects[1].Bottom;
			this.destRects[3].Y = this.destRects[1].Y + this.destRects[1].Height;

			this.faceDestRect.Width = this.actor.FaceRect.Width * GameMain.TextureScale;
			this.faceDestRect.Height = this.actor.FaceRect.Height * GameMain.TextureScale;
			this.faceDestRect.X = this.destRects[0].X + 96 * GameMain.TextureScale;
			this.faceDestRect.Y = this.destRects[0].Y + 8 * GameMain.TextureScale;

			this.iconLocation.X = this.destRects[0].X + 16 * GameMain.TextureScale;
			this.iconLocation.Y = this.destRects[0].Y + 10 * GameMain.TextureScale;

			this.actorAnimation = this.actor.AnimationWait.Clone() as ActorWaitAnimation;
			this.actorAnimation.SetDestLocation(this.iconLocation);

			this.combatLocations = new Vector2[3];
			this.combatLocations[0] = new Vector2(this.destRects[0].X + 40 * GameMain.TextureScale, this.destRects[0].Y + 44 * GameMain.TextureScale);
			this.combatLocations[1] = new Vector2(this.destRects[0].X + 158 * GameMain.TextureScale, this.destRects[0].Y + 27 * GameMain.TextureScale);
			this.combatLocations[2] = new Vector2(this.destRects[0].X + 202 * GameMain.TextureScale, this.destRects[0].Y + 27 * GameMain.TextureScale);

			this.combatLabels = new string[5];
			this.combatLabels[0] = GameMain.SysText["LabelAttack"];
			this.combatLabels[1] = GameMain.SysText["LabelCritical"];
			this.combatLabels[2] = GameMain.SysText["LabelHit"];
			this.combatLabels[3] = GameMain.SysText["LabelAvoid"];
			this.combatLabels[4] = GameMain.SysText["LabelRange"];

			this.combatValues = new string[7];
			this.combatValues[0] = string.Format("{0:2} {1:2}", this.actor.LV.ToString().PadLeft(2, ' '), this.actor.EXP.ToString().PadLeft(2, ' '));
			this.combatValues[1] = string.Format("{0:2} {1:2}", this.actor.CurrentHP.ToString().PadLeft(2, ' '), this.actor.ActualHP.ToString().PadLeft(2, ' '));
			this.combatValues[2] = this.actor.ActualAttackPower.ToString().PadLeft(3, ' ');
			this.combatValues[3] = this.actor.ActualCritical.ToString().PadLeft(3, ' ');
			this.combatValues[4] = this.actor.ActualAccuracy.ToString().PadLeft(3, ' ');
			this.combatValues[5] = this.actor.ActualAvoid.ToString().PadLeft(3, ' ');
			this.combatValues[6] = this.actor.CurrentWeapon.Range.ToString();

			this.scrollMarkAnimations = new FrameAnimation[2];
			this.scrollMarkAnimations[0] = new FrameAnimation()
			{
				Texture = GameMain.SysTexture,
				TextureRectangle = new Rectangle(128, 0, 8, 8),
				FrameSkip = 8,
				FrameCount = 6
			};
			this.scrollMarkAnimations[1] = new FrameAnimation()
			{
				Texture = GameMain.SysTexture,
				TextureRectangle = new Rectangle(128, 0, 8, 8),
				FrameSkip = 8,
				FrameCount = 6,
				Flip = SpriteEffects.FlipVertically
			};

			this.scrollMarkAnimations[0].SetDestLocation(
				this.destRects[1].Left + (this.destRects[1].Width - this.scrollMarkAnimations[0].TextureRectangle.Width * GameMain.TextureScale) / 2,
				this.destRects[1].Bottom - (1 + this.scrollMarkAnimations[0].TextureRectangle.Height) * GameMain.TextureScale
			);

			#endregion main frame

			#region attributes

			this.attrLocations = new Vector2[14];

			for (int i = 0; i < this.attrLocations.Length; i++)
			{
				if (i < 7)
				{
					this.attrLocations[i].X = this.destRects[1].Left + 64 * GameMain.TextureScale;

					if (i == 0)
					{
						this.attrLocations[i].Y = this.destRects[1].Y + 21 * GameMain.TextureScale;
					}
					else
					{
						this.attrLocations[i].Y = this.attrLocations[i - 1].Y + 16 * GameMain.TextureScale;
					}
				}
				else
				{
					this.attrLocations[i].X = this.destRects[1].Left + 16 * GameMain.TextureScale;

					if (i == 7)
					{
						this.attrLocations[i].Y = this.destRects[2].Y + 28 * GameMain.TextureScale;
					}
					else
					{
						this.attrLocations[i].Y = this.attrLocations[i - 1].Y + 16 * GameMain.TextureScale;
					}
				}
			}

			this.attrLabels = new string[14];
			this.attrLabels[0] = GameMain.SysText["LabelSTR"];
			this.attrLabels[1] = GameMain.SysText["LabelMAG"];
			this.attrLabels[2] = GameMain.SysText["LabelSKL"];
			this.attrLabels[3] = GameMain.SysText["LabelSPD"];
			this.attrLabels[4] = GameMain.SysText["LabelLUK"];
			this.attrLabels[5] = GameMain.SysText["LabelDEF"];
			this.attrLabels[6] = GameMain.SysText["LabelBLD"];

			this.attrLabels[7] = GameMain.SysText["LabelArmy"];
			this.attrLabels[8] = GameMain.SysText["LabelLead"];
			this.attrLabels[9] = GameMain.SysText["LabelReact"];
			this.attrLabels[10] = GameMain.SysText["LabelCarry"];
			this.attrLabels[11] = GameMain.SysText["LabelMove"];
			this.attrLabels[12] = GameMain.SysText["LabelTire"];
			this.attrLabels[13] = GameMain.SysText["LabelStat"];

			for (int i = 0; i < this.actor.Items.Count; i++)
			{
				this.attrLabels[i] += new string(' ', 6) + this.actor.Items[i].Name;

				if (i == 0 && this.actor.Items[i] is Weapon)
				{
					this.attrLabels[i] += "(E)";
				}
			}

			this.attrValues = new string[14];
			this.attrValues[0] = this.actor.ActualSTR.ToString().PadLeft(2, ' ');
			this.attrValues[1] = this.actor.ActualMAG.ToString().PadLeft(2, ' ');
			this.attrValues[2] = this.actor.ActualSKL.ToString().PadLeft(2, ' ');
			this.attrValues[3] = this.actor.ActualSPD.ToString().PadLeft(2, ' ');
			this.attrValues[4] = this.actor.ActualLUK.ToString().PadLeft(2, ' ');
			this.attrValues[5] = this.actor.ActualDEF.ToString().PadLeft(2, ' ');
			this.attrValues[6] = this.actor.ActualBLD.ToString().PadLeft(2, ' ');

			this.attrValues[7] = string.Format("             {0}       {1}", this.actor.Sword.ToWLV(), this.actor.Fire.ToWLV());
			this.attrValues[8] = string.Format("             {0}       {1}", this.actor.Lance.ToWLV(), this.actor.Thunder.ToWLV());
			this.attrValues[9] = string.Format("             {0}       {1}", this.actor.Axe.ToWLV(), this.actor.Wind.ToWLV());
			this.attrValues[10] = string.Format("----         {0}       {1}", this.actor.Bow.ToWLV(), this.actor.Light.ToWLV());
			this.attrValues[11] = this.actor.Class.MOV.ToString().PadLeft(3, ' ') + string.Format("          {0}       {1}", this.actor.Staff.ToWLV(), this.actor.Dark.ToWLV());
			this.attrValues[12] = 0.ToString().PadLeft(3, ' ');
			this.attrValues[13] = "----";

			#endregion attributes

			#region items & perks

			this.itemDestRects = new Rectangle[this.actor.Items.Count];

			for (int i = 0; i < this.itemDestRects.Length; i++)
			{
				this.itemDestRects[i].Width = this.actor.Items[i].SrcRegion.Width * GameMain.TextureScale;
				this.itemDestRects[i].Height = this.actor.Items[i].SrcRegion.Height * GameMain.TextureScale;
				this.itemDestRects[i].X = this.destRects[1].X + 120 * GameMain.TextureScale;
				this.itemDestRects[i].Y = this.destRects[1].Y + 16 * GameMain.TextureScale + i * this.itemDestRects[i].Height;
			}

			this.perkDestRects = new Rectangle[this.actor.Perks.Length];

			for (int i = 0; i < this.perkDestRects.Length; i++)
			{
				this.perkDestRects[i].Width = this.actor.Perks[i].SrcRegion.Width * GameMain.TextureScale;
				this.perkDestRects[i].Height = this.actor.Perks[i].SrcRegion.Height * GameMain.TextureScale;
				this.perkDestRects[i].X = this.destRects[2].Left + 120 * GameMain.TextureScale + i * this.perkDestRects[i].Width;
				this.perkDestRects[i].Y = this.destRects[2].Bottom - 18 * GameMain.TextureScale;
			}

			#endregion items & perks

			#region icon actorAnimation

			this.boxAnimation = new FrameAnimation()
			{
				Texture = this.iconTexture,
				TextureRectangle = new Rectangle(448, 0, 16, 16),
				FrameSkip = 4,
				FrameCount = 4
			};

			#endregion icon actorAnimation
		}

		public override void UnloadContent()
		{
			this.frameTexture = null;
		}

		public override void Update(GameTime gameTime, bool isCovered, bool isFocusLost)
		{
			base.Update(gameTime, isCovered, isFocusLost);

			this.animationTime += gameTime.ElapsedGameTime;

			if (this.animationTime.TotalMilliseconds > GameMain.MSPF * 60)
			{
				this.animationTime = TimeSpan.Zero;
			}

			this.actorAnimation.Update(gameTime, this.animationTime);

			if (this.isScrolling)
			{
				this.scrollTime += gameTime.ElapsedGameTime;
				double percent = this.scrollTime.TotalMilliseconds / 333d;
				int offset = (int)(MathHelper.SmoothStep(0f, 1f, (float)percent) * this.scrollArea.Height);

				if (percent < 1.0d)
				{
					if (this.scrollDirection == Direction.Up)
					{
						this.destRects[1].Y = this.scrollArea.Top - offset;
						this.destRects[2].Y = this.scrollArea.Bottom - offset;
					}
					else
					{
						this.destRects[1].Y = this.scrollArea.Top - this.scrollArea.Height + offset;
						this.destRects[2].Y = this.scrollArea.Top + offset;
					}
				}
				else
				{
					this.isScrolling = false;
					this.scrollTime = TimeSpan.Zero;

					if (this.scrollDirection == Direction.Up)
					{
						this.destRects[1].Y = this.scrollArea.Top - this.scrollArea.Height;
						this.destRects[2].Y = this.scrollArea.Top;
					}
					else
					{
						this.destRects[1].Y = this.scrollArea.Top;
						this.destRects[2].Y = this.scrollArea.Bottom;
					}

					int x = this.scrollArea.Left + (this.scrollArea.Width - this.scrollMarkAnimations[0].TextureRectangle.Width * GameMain.TextureScale) / 2;
					this.scrollMarkAnimations[0].SetDestLocation(x, this.destRects[1].Bottom - (1 + this.scrollMarkAnimations[0].TextureRectangle.Height) * GameMain.TextureScale);
					this.scrollMarkAnimations[1].SetDestLocation(x, this.destRects[2].Y + 1 * GameMain.TextureScale);
				}

				for (int i = 0; i < this.attrLocations.Length; i++)
				{
					switch (i)
					{
						case 0:
							this.attrLocations[i].Y = this.destRects[1].Y + 21 * GameMain.TextureScale;
							break;
						case 7:
							this.attrLocations[i].Y = this.destRects[2].Y + 28 * GameMain.TextureScale;
							break;
						default:
							this.attrLocations[i].Y = this.attrLocations[i - 1].Y + 16 * GameMain.TextureScale;
							break;
					}
				}

				for (int i = 0; i < this.itemDestRects.Length; i++)
				{
					this.itemDestRects[i].Y = this.destRects[1].Y + 16 * GameMain.TextureScale + i * this.itemDestRects[i].Height;
				}

				for (int i = 0; i < this.perkDestRects.Length; i++)
				{
					this.perkDestRects[i].Y = this.destRects[2].Bottom - 18 * GameMain.TextureScale;
				}
			}
			else
			{
				if (this.scrollDirection == Direction.Up)
				{
					this.scrollMarkAnimations[1].Update(gameTime);
				}
				else
				{
					this.scrollMarkAnimations[0].Update(gameTime);
				}
			}

			if (this.selectedItemIndex > -1)
			{
				this.boxAnimation.Update(gameTime, this.itemDestRects[this.selectedItemIndex].Location);
			}

			if (this.selectedPerkIndex > -1)
			{
				this.boxAnimation.Update(gameTime, this.perkDestRects[this.selectedPerkIndex].Location);
			}
		}

		public override void UpdateInput(GameTime gameTime)
		{
			if (Input.Mouse.IsMouseOut(this.destRects[0]) && Input.Mouse.IsLeftButtonClicked())
			{
				this.Exit();
			}
			else if (this.isScrolling || Input.Mouse.IsMouseOut(this.scrollArea))
			{
				return;
			}

			if (Input.Mouse.IsLeftButtonClicked())
			{
				if (this.scrollDirection != Direction.Up && Input.Mouse.State.Y > this.scrollArea.Bottom - GameMain.CellSize.Height)
				{
					this.isScrolling = true;
					this.scrollDirection = Direction.Up;
				}
				else if (this.scrollDirection == Direction.Up && Input.Mouse.State.Y < this.scrollArea.Top + GameMain.CellSize.Height)
				{
					this.isScrolling = true;
					this.scrollDirection = Direction.Down;
				}
				else
				{
					bool isItemSelected = false;
					bool isPerkSelected = false;

					if (this.scrollDirection == Direction.Up)
					{
						for (int i = 0; i < this.perkDestRects.Length; i++)
						{
							if (Input.Mouse.IsMouseOver(this.perkDestRects[i]))
							{
								this.boxAnimation.SetDestLocation(this.perkDestRects[i].Location);
								this.selectedPerkIndex = i;
								this.selectedItemIndex = -1;
								isPerkSelected = true;
								break;
							}
						}
					}
					else
					{
						for (int i = 0; i < this.itemDestRects.Length; i++)
						{
							if (Input.Mouse.IsMouseOver(this.itemDestRects[i]))
							{
								this.boxAnimation.SetDestLocation(this.itemDestRects[i].Location);
								this.selectedItemIndex = i;
								this.selectedPerkIndex = -1;
								isItemSelected = true;
								break;
							}
						}
					}

					if (!isItemSelected && !isPerkSelected)
					{
						this.selectedItemIndex = -1;
						this.selectedPerkIndex = -1;
					}
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			#region main area

			this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			this.SpriteBatch.Draw(this.frameTexture, this.destRects[0], this.srcRects[0], Color.White);
			this.SpriteBatch.Draw(GameMain.ActorFaceTexture, this.faceDestRect, this.actor.FaceRect, Color.White);
			this.actorAnimation.Draw(this.SpriteBatch, gameTime);

			this.SpriteBatch.DrawString(GameMain.NormalFont, this.actor.Name, this.iconLocation.Expand(16 * GameMain.TextureScale, 0).ToVector2(), Color.White);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.actor.Class.Name, this.iconLocation.Expand(0, 16 * GameMain.TextureScale).ToVector2(), Color.White);

			this.SpriteBatch.DrawString(GameMain.NormalFont, this.combatLabels[0], this.combatLocations[1], Color.White);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.combatLabels[1], this.combatLocations[2], Color.White);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.combatLabels[2], this.combatLocations[1].Expand(0, 16 * GameMain.TextureScale), Color.White);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.combatLabels[3], this.combatLocations[2].Expand(0, 16 * GameMain.TextureScale), Color.White);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.combatLabels[4], this.combatLocations[1].Expand(0, 32 * GameMain.TextureScale), Color.White);

			this.SpriteBatch.DrawString(GameMain.DigitFont, this.combatValues[0], this.combatLocations[0], Color.White, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.combatValues[1], this.combatLocations[0].Expand(0, 16 * GameMain.TextureScale), Color.White, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.combatValues[2], this.combatLocations[1].Expand(16 * GameMain.TextureScale, 2 * GameMain.TextureScale), blue, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.combatValues[3], this.combatLocations[2].Expand(16 * GameMain.TextureScale, 2 * GameMain.TextureScale), blue, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.combatValues[4], this.combatLocations[1].Expand(16 * GameMain.TextureScale, 18 * GameMain.TextureScale), blue, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.combatValues[5], this.combatLocations[2].Expand(16 * GameMain.TextureScale, 18 * GameMain.TextureScale), blue, 2f);
			this.SpriteBatch.DrawString(GameMain.DigitFont, this.combatValues[6], this.combatLocations[1].Expand(35 * GameMain.TextureScale, 34 * GameMain.TextureScale), blue, 2f);

			this.SpriteBatch.Draw(this.frameTexture, this.destRects[3], this.srcRects[3], Color.White);

			this.SpriteBatch.End();

			#endregion main area

			this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, rasterizerState);

			#region attributes

			if (this.isScrolling)
			{
				this.DrawFrame1(this.SpriteBatch, gameTime);
				this.DrawFrame2(this.SpriteBatch, gameTime);
			}
			else
			{
				if (this.scrollDirection == Direction.Up)
				{
					this.DrawFrame2(this.SpriteBatch, gameTime);
					this.scrollMarkAnimations[1].Draw(this.SpriteBatch, gameTime);
				}
				else
				{
					this.DrawFrame1(this.SpriteBatch, gameTime);
					this.scrollMarkAnimations[0].Draw(this.SpriteBatch, gameTime);
				}
			}

			#endregion attributes

			this.SpriteBatch.End();
		}

		private void DrawFrame1(SpriteBatch sb, GameTime gameTime)
		{
			sb.Draw(this.frameTexture, this.destRects[1], this.topSrcRect, Color.White);

			sb.DrawString(GameMain.NormalFont, this.attrLabels[0], this.attrLocations[0].Expand(-22 * GameMain.TextureScale, -3), Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[1], this.attrLocations[1].Expand(-22 * GameMain.TextureScale, -3), Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[2], this.attrLocations[2].Expand(-22 * GameMain.TextureScale, -3), Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[3], this.attrLocations[3].Expand(-22 * GameMain.TextureScale, -3), Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[4], this.attrLocations[4].Expand(-22 * GameMain.TextureScale, -3), Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[5], this.attrLocations[5].Expand(-22 * GameMain.TextureScale, -3), Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[6], this.attrLocations[6].Expand(-22 * GameMain.TextureScale, -3), Color.White);

			sb.DrawString(GameMain.DigitFont, this.attrValues[0], this.attrLocations[0], this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[1], this.attrLocations[1], this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[2], this.attrLocations[2], this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[3], this.attrLocations[3], this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[4], this.attrLocations[4], this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[5], this.attrLocations[5], this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[6], this.attrLocations[6], this.blue, 2f);

			for (int i = 0; i < this.itemDestRects.Length; i++)
			{
				sb.Draw(this.iconTexture, this.itemDestRects[i], this.actor.Items[i].SrcRegion, Color.White);
			}

			if (this.selectedItemIndex > -1)
			{
				this.boxAnimation.Draw(this.SpriteBatch, gameTime);
			}
		}

		private void DrawFrame2(SpriteBatch sb, GameTime gameTime)
		{
			sb.Draw(this.frameTexture, this.destRects[2], this.bottomSrcRect, Color.White);

			sb.DrawString(GameMain.NormalFont, this.attrLabels[07], this.attrLocations[07], Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[08], this.attrLocations[08], Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[09], this.attrLocations[09], Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[10], this.attrLocations[10], Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[11], this.attrLocations[11], Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[12], this.attrLocations[12], Color.White);
			sb.DrawString(GameMain.NormalFont, this.attrLabels[13], this.attrLocations[13], Color.White);

			sb.DrawString(GameMain.DigitFont, this.attrValues[07], this.attrLocations[07].Expand(32 * GameMain.TextureScale, 4), this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[08], this.attrLocations[08].Expand(32 * GameMain.TextureScale, 4), this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[09], this.attrLocations[09].Expand(32 * GameMain.TextureScale, 4), this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[10], this.attrLocations[10].Expand(32 * GameMain.TextureScale, 4), this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[11], this.attrLocations[11].Expand(32 * GameMain.TextureScale, 4), this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[12], this.attrLocations[12].Expand(32 * GameMain.TextureScale, 4), this.blue, 2f);
			sb.DrawString(GameMain.DigitFont, this.attrValues[13], this.attrLocations[13].Expand(32 * GameMain.TextureScale, 4), this.blue, 2f);

			for (int i = 0; i < this.perkDestRects.Length; i++)
			{
				sb.Draw(this.iconTexture, this.perkDestRects[i], this.actor.Perks[i].SrcRegion, Color.White);
			}

			if (this.selectedPerkIndex > -1)
			{
				this.boxAnimation.Draw(sb, gameTime);
			}
		}
	}
}