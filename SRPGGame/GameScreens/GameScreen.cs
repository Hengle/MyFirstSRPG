using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGame.GameScreens
{
	public abstract class GameScreen
	{
		public event EventHandler OnScreenRemoved;

		public GameScreenManager ScreenManager;
		public ScreenStatus Status;

		public GameMain Game { get { return this.ScreenManager.Game; } }

		public ContentManager Content { get { return this.ScreenManager.Content; } }

		public SpriteBatch SpriteBatch { get { return this.ScreenManager.SpriteBatch; } }

		public float SmoothTransitionOffset { get { return MathHelper.SmoothStep(0, 1, this.TransitionOffset); } }

		public float TransitionOffset { get; private set; }

		public bool IsPopup { get; protected set; }

		public bool IsActive { get { return !isFocusLost && this.Status.In(ScreenStatus.TransitionOn, ScreenStatus.Active); } }

		protected Color Brush;
		protected TimeSpan TransitionOnTime;
		protected TimeSpan TransitionOffTime;

		private bool isExiting;
		private bool isFocusLost;

		public GameScreen()
		{
			this.TransitionOnTime = TimeSpan.Zero;
			this.TransitionOffTime = TimeSpan.Zero;
			this.TransitionOffset = 0;
			this.Status = ScreenStatus.Hidden;
			this.Brush = Color.Transparent;
		}

		public virtual void LoadContent() { }

		public virtual void UnloadContent() { }

		public virtual void Update(GameTime gameTime, bool isCovered, bool isFocusLost)
		{
			this.isFocusLost = isFocusLost;

			if (!this.isExiting)
			{
				if (this.UpdateTransition(gameTime, this.TransitionOnTime, 1))
				{
					this.Status = ScreenStatus.TransitionOn;
				}
				else
				{
					this.Status = ScreenStatus.Active;
				}
			}
			else
			{
				if (isCovered)
				{
					if (this.UpdateTransition(gameTime, this.TransitionOffTime, -1))
					{
						this.Status = ScreenStatus.TransitionOff;
					}
					else
					{
						this.Status = ScreenStatus.Hidden;
					}
				}
				else
				{
					this.Status = ScreenStatus.TransitionOff;

					if (!this.UpdateTransition(gameTime, this.TransitionOffTime, -1))
					{
						this.RemoveScreen();
					}
				}
			}
		}

		public virtual void Draw(GameTime gameTime) { }

		public virtual void UpdateInput(GameTime gameTime) { }

		public virtual void Exit()
		{
			if (this.TransitionOffTime == TimeSpan.Zero)
			{
				this.RemoveScreen();
			}
			else
			{
				this.isExiting = true;
			}
		}

		private void RemoveScreen()
		{
			this.Status = ScreenStatus.Hidden;
			this.ScreenManager.RemoveScreen(this);

			if (this.OnScreenRemoved != null)
			{
				this.OnScreenRemoved(this, null);
			}
		}

		private bool UpdateTransition(GameTime gameTime, TimeSpan transitionTime, int direction)
		{
			float transitionProgress;

			if (transitionTime == TimeSpan.Zero)
			{
				transitionProgress = 1;
			}
			else
			{
				transitionProgress = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / transitionTime.TotalMilliseconds);
			}

			this.TransitionOffset += transitionProgress * direction;

			if ((direction < 0 && this.TransitionOffset <= 0) || (direction > 0 && this.TransitionOffset >= 1))
			{
				this.TransitionOffset = MathHelper.Clamp(this.TransitionOffset, 0, 1);
				return false;
			}

			return true;
		}

		public override string ToString()
		{
			return this.GetType().Name.ToString();
		}
	}

	public enum ScreenStatus
	{
		TransitionOn,
		Active,
		TransitionOff,
		Hidden
	}
}