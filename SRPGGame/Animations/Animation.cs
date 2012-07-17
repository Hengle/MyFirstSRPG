using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyFirstSRPG.SRPGGame.Animations
{
	public abstract class Animation
	{
		protected TimeSpan ElapsedGameTime = TimeSpan.Zero;

		public Texture2D Texture;
		public Rectangle TextureRectangle;
		public Rectangle LastDestRectangle;
		public Rectangle DestRectangle;
		public Rectangle SrcRectangle;
		public Point DestOffest;
		public int FrameSkip;
		public int FrameCount;
		public SpriteEffects Flip = SpriteEffects.None;
		public bool CameraFollowing = false;

		private Rectangle destRect;

		public virtual void Update(GameTime gameTime)
		{
			this.destRect = this.DestRectangle;

			if (this.CameraFollowing)
			{
				this.destRect.X -= GameMain.Camera.Location.X;
				this.destRect.Y -= GameMain.Camera.Location.Y;
			}

			this.destRect.X += this.DestOffest.X * GameMain.TextureScale;
			this.destRect.Y += this.DestOffest.Y * GameMain.TextureScale;
			this.destRect.Width = this.TextureRectangle.Width * GameMain.TextureScale;
			this.destRect.Height = this.TextureRectangle.Height * GameMain.TextureScale;
		}

		public virtual void Draw(SpriteBatch sb, GameTime gameTime)
		{
			this.Draw(sb, gameTime, this.Texture, Color.White);
		}

		public virtual void Draw(SpriteBatch sb, GameTime gameTime, Texture2D texture)
		{
			this.Draw(sb, gameTime, texture, Color.White);
		}

		public virtual void Draw(SpriteBatch sb, GameTime gameTime, Texture2D texture, Color brush)
		{
			sb.Draw(texture, this.destRect, this.SrcRectangle, brush, 0, Vector2.Zero, this.Flip, 0);
		}

		public void SetDestLocation(int x, int y)
		{
			this.DestRectangle.X = x;
			this.DestRectangle.Y = y;
		}

		public void SetDestLocation(Point location)
		{
			this.SetDestLocation(location.X, location.Y);
		}
	}
}