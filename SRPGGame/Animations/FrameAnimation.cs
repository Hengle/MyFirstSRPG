using System;
using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGame.Animations
{
	public class FrameAnimation : Animation
	{
		public override void Update(GameTime gameTime)
		{
			this.SrcRectangle = this.TextureRectangle;
			this.ElapsedGameTime += gameTime.ElapsedGameTime;
			double msPerFrame = GameMain.MSPF * this.FrameSkip;

			if (this.ElapsedGameTime.TotalMilliseconds > msPerFrame * this.FrameCount)
			{
				this.ElapsedGameTime = TimeSpan.Zero;
			}
			else
			{
				int frameIndex = (int)Math.Floor(this.ElapsedGameTime.TotalMilliseconds / msPerFrame);
				this.SrcRectangle.X += frameIndex * this.TextureRectangle.Width;
			}

			base.Update(gameTime);
		}

		public void Update(GameTime gameTime, Point location)
		{
			this.Update(gameTime, location.X, location.Y);
		}

		public void Update(GameTime gameTime, int x, int y)
		{
			this.SetDestLocation(x, y);
			this.Update(gameTime);
		}
	}
}