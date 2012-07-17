using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGame.GameScreens.SceneScreens
{
	public class SceneDialogueScreen : GameScreen
	{
		public Queue<ActorSpeech> SpeechQuene;

		#region private fields

		private SceneScreen scene;
		private Rectangle bgSrcRect;
		private Rectangle srcRect1;
		private Rectangle srcRect2;
		private Rectangle srcRect3;
		private Rectangle destRect1;
		private Rectangle destRect2;
		private Rectangle destRect3a;
		private Rectangle destRect3b;
		private SceneActor[] actors;
		private Rectangle[] faceDestRects;
		private Point[] locations;
		private Queue<char> speechBuffer;
		private float maxDialogWidth;
		private StringBuilder[] speechStrings;
		private Vector2[] speechLocations;
		private TimeSpan speechTime;
		private TimeSpan pauseTime;
		private TimeSpan currentPauseTime;
		private int currentSpeechIndex;

		#endregion private fields

		public SceneDialogueScreen(SceneScreen scene)
		{
			this.TransitionOnTime = TimeSpan.FromMilliseconds(1000d);
			this.TransitionOffTime = TimeSpan.FromMilliseconds(1000d);
			this.scene = scene;
			this.SpeechQuene = new Queue<ActorSpeech>();
			this.speechStrings = new StringBuilder[4];
			this.speechLocations = new Vector2[4];
			this.speechBuffer = new Queue<char>();
		}

		public override void LoadContent()
		{
			this.bgSrcRect = new Rectangle(84, 16, 1, 1);
			this.srcRect1 = new Rectangle(0, 236, 16, 32);
			this.srcRect2 = new Rectangle(16, 236, 8, 32);
			this.srcRect3 = new Rectangle(24, 236, 13, 18);
			this.destRect1 = new Rectangle(0, 0, this.srcRect1.Width * GameMain.TextureScale, this.srcRect1.Height * GameMain.TextureScale);
			this.destRect2 = new Rectangle(0, 0, this.srcRect2.Width * GameMain.TextureScale, this.srcRect2.Height * GameMain.TextureScale);
			this.destRect3a =
				this.destRect3b = new Rectangle(0, 0, this.srcRect3.Width * GameMain.TextureScale, this.srcRect3.Height * GameMain.TextureScale);
			this.locations = new Point[8];

			for (int i = 0; i < 4; i++)
			{
				this.locations[i * 2].X = GameMain.ScreenBounds.Width / 2 - this.destRect1.Width * 7 - this.destRect2.Width;
				this.locations[i * 2 + 1].X = GameMain.ScreenBounds.Width / 2;
			}

			this.locations[0].Y =
				this.locations[1].Y = GameMain.ScreenBounds.Height / 2 - 88 - this.destRect1.Height * 2;
			this.locations[2].Y =
				this.locations[3].Y = this.locations[0].Y + this.destRect1.Height;
			this.locations[4].Y =
				this.locations[5].Y = GameMain.ScreenBounds.Height / 2 + 88;
			this.locations[6].Y =
				this.locations[7].Y = this.locations[4].Y + this.destRect1.Height;

			this.actors = new SceneActor[2];
			this.faceDestRects = new Rectangle[2];
			this.faceDestRects[0] =
				this.faceDestRects[1] = new Rectangle(0, 0, 48 * GameMain.TextureScale, 64 * GameMain.TextureScale);
			this.faceDestRects[0].X = this.locations[4].X + 24 * GameMain.TextureScale;
			this.faceDestRects[1].X = this.locations[5].X + 48 * GameMain.TextureScale;
			this.faceDestRects[0].Y =
				this.faceDestRects[1].Y = this.locations[4].Y - 64 * GameMain.TextureScale;

			this.destRect3a.X = this.locations[2].X + this.destRect2.Width + this.destRect1.Width * 4 + 10 * GameMain.TextureScale;
			this.destRect3a.Y = this.locations[2].Y + this.destRect2.Height - 2 * GameMain.TextureScale;
			this.destRect3b.X = this.locations[5].X + this.destRect1.Width + 9 * GameMain.TextureScale;
			this.destRect3b.Y = this.locations[5].Y - this.destRect3b.Height + 2 * GameMain.TextureScale;

			this.maxDialogWidth = this.destRect1.Width * 14f;
			this.speechStrings[0] = new StringBuilder();
			this.speechStrings[1] = new StringBuilder();
			this.speechStrings[2] = new StringBuilder();
			this.speechStrings[3] = new StringBuilder();

			Vector2 offset = new Vector2(15, 10);
			this.speechLocations[0] = this.locations[0].Expand(10, 10).ToVector2() + offset;
			this.speechLocations[1] = this.locations[2].Expand(10, -5).ToVector2() + offset;
			this.speechLocations[2] = this.locations[4].Expand(10, 10).ToVector2() + offset;
			this.speechLocations[3] = this.locations[6].Expand(10, -5).ToVector2() + offset;
		}

		public override void Update(GameTime gameTime, bool isCovered, bool isFocusLost)
		{
			base.Update(gameTime, isCovered, isFocusLost);

			if (this.Status.In(ScreenStatus.TransitionOn, ScreenStatus.TransitionOff))
			{
				this.Brush = Color.White * this.SmoothTransitionOffset;
			}
			else if (this.Status == ScreenStatus.Active)
			{
				this.UpdateDialog(gameTime);
			}
		}

		public override void UpdateInput(GameTime gameTime)
		{
			if (Input.Mouse.IsMouseOver(GameMain.SafeArea) && Input.Mouse.IsLeftButtonClicked())
			{
				this.Exit();
			}
		}

		public override void Draw(GameTime gameTime)
		{
			this.DrawFrame();
			this.DrawText();
		}

		public override void Exit()
		{
			this.SpeechQuene.Clear();
			base.Exit();
		}

		private void UpdateDialog(GameTime gameTime)
		{
			if (this.SpeechQuene.Count == 0 && this.speechBuffer.Count == 0)
			{
				this.Exit();
				return;
			}

			if (this.pauseTime < this.currentPauseTime)
			{
				this.pauseTime += gameTime.ElapsedGameTime;
				return;
			}

			this.speechTime += gameTime.ElapsedGameTime;

			if (this.speechTime.TotalMilliseconds > GameMain.MSPF * 6)
			{
				if (this.speechBuffer.Count == 0)
				{
					if (this.SpeechQuene.Count > 0)
					{
						ActorSpeech speech = this.SpeechQuene.Peek();

						if (speech.IsLeft)
						{
							if (this.actors[0] != speech.Actor)
							{
								this.speechStrings[0].Clear();
								this.speechStrings[1].Clear();
								this.actors[0] = speech.Actor;
							}

							if (this.speechStrings[0].Length == 0)
							{
								this.currentSpeechIndex = 0;
							}
							else
							{
								this.currentSpeechIndex = 1;
							}
						}
						else
						{
							if (this.actors[1] != speech.Actor)
							{
								this.speechStrings[2].Clear();
								this.speechStrings[3].Clear();
								this.actors[1] = speech.Actor;
							}

							if (this.speechStrings[2].Length == 0)
							{
								this.currentSpeechIndex = 2;
							}
							else
							{
								this.currentSpeechIndex = 3;
							}
						}

						if (this.speechStrings[this.currentSpeechIndex].Length > 0)
						{
							this.speechBuffer.Enqueue('\n');
						}

						foreach (char c in GameMain.NormalFont.SplitString(speech.Text, this.maxDialogWidth))
						{
							this.speechBuffer.Enqueue(c);
						}

						this.currentPauseTime = speech.PauseTime;
						this.pauseTime = TimeSpan.Zero;
						this.SpeechQuene.Dequeue();
					}
				}
				else
				{
					char c = this.speechBuffer.Dequeue();

					if (this.currentSpeechIndex.In(1, 3))
					{
						if (c == '\n')
						{
							this.speechStrings[this.currentSpeechIndex - 1].Clear();
							this.speechStrings[this.currentSpeechIndex - 1] = this.speechStrings[this.currentSpeechIndex];
							this.speechStrings[this.currentSpeechIndex] = new StringBuilder();
							return;
						}
					}
					else if (c == '\n')
					{
						this.currentSpeechIndex++;
						return;
					}

					this.speechStrings[this.currentSpeechIndex].Append(c);
				}

				this.speechTime = TimeSpan.Zero;
			}
		}

		private void DrawFrame()
		{
			this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			this.SpriteBatch.Draw(GameMain.SysTexture, GameMain.ScreenBounds, this.bgSrcRect, this.Brush * 0.7f);

			this.DrawFramePart(this.locations[0], SpriteEffects.None);
			this.DrawFramePart(this.locations[1], SpriteEffects.FlipHorizontally);
			this.DrawFramePart(this.locations[2], SpriteEffects.FlipVertically);
			this.DrawFramePart(this.locations[3], SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally);
			this.SpriteBatch.Draw(GameMain.SysTexture, this.destRect3a, this.srcRect3, this.Brush, 0, Vector2.Zero, SpriteEffects.None, 0);

			if (this.actors[0] != null)
			{
				this.SpriteBatch.Draw(GameMain.ActorFaceTexture, this.faceDestRects[0], this.actors[0].FaceRect, this.Brush);
			}

			if (this.actors[1] != null)
			{
				this.SpriteBatch.Draw(GameMain.ActorFaceTexture, this.faceDestRects[1], this.actors[1].FaceRect, this.Brush, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
			}

			this.DrawFramePart(this.locations[4], SpriteEffects.None);
			this.DrawFramePart(this.locations[5], SpriteEffects.FlipHorizontally);
			this.DrawFramePart(this.locations[6], SpriteEffects.FlipVertically);
			this.DrawFramePart(this.locations[7], SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally);
			this.SpriteBatch.Draw(GameMain.SysTexture, this.destRect3b, this.srcRect3, this.Brush, 0, Vector2.Zero, SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0);

			this.SpriteBatch.End();
		}

		private void DrawFramePart(Point location, SpriteEffects flip2)
		{
			Rectangle rect1 = new Rectangle(0, location.Y, this.destRect1.Width, this.destRect1.Height);
			Rectangle rect2 = new Rectangle(0, location.Y, this.destRect2.Width, this.destRect2.Height);
			SpriteEffects flip1 = flip2 & SpriteEffects.FlipVertically;

			if ((flip2 & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally)
			{
				flip1 = flip1 | SpriteEffects.FlipHorizontally;
				rect1.X = location.X;
				rect2.X = location.X + rect1.Width * 7;
			}
			else
			{
				rect1.X = location.X + rect2.Width;
				rect2.X = location.X;
			}

			for (int i = 0; i < 7; i++)
			{
				this.SpriteBatch.Draw(GameMain.SysTexture, rect1, this.srcRect1, this.Brush, 0, Vector2.Zero, flip1, 0);

				rect1.X += rect1.Width;

				if ((flip1 & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally)
				{
					flip1 = flip2 & SpriteEffects.FlipVertically;
				}
				else
				{
					flip1 = (flip2 & SpriteEffects.FlipVertically) | SpriteEffects.FlipHorizontally;
				}
			}

			this.SpriteBatch.Draw(GameMain.SysTexture, rect2, this.srcRect2, this.Brush, 0, Vector2.Zero, flip2, 0);
		}

		private void DrawText()
		{
			this.SpriteBatch.Begin();

			this.SpriteBatch.DrawString(GameMain.NormalFont, this.speechStrings[0], this.speechLocations[0], this.Brush);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.speechStrings[1], this.speechLocations[1], this.Brush);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.speechStrings[2], this.speechLocations[2], this.Brush);
			this.SpriteBatch.DrawString(GameMain.NormalFont, this.speechStrings[3], this.speechLocations[3], this.Brush);

			this.SpriteBatch.End();
		}

		private void DrawLine(StringBuilder sb, Vector2 location, Color fontColor, Color lightColor, Color darkColor)
		{
			this.SpriteBatch.DrawString(GameMain.NormalFont, sb, location, fontColor);
		}

		public void AddSpeech(ActorSpeech speech)
		{
			this.SpeechQuene.Enqueue(speech);
		}
	}
}