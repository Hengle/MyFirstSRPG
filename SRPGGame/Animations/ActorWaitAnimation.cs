using System;
using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGame.Animations
{
	public class ActorWaitAnimation : ActorAnimation
	{
		public ActorWaitAnimation(SceneActor actor = null)
			: base(actor)
		{
			if (actor != null)
			{
				this.TextureRectangle = actor.Class.Region1;
				this.DestOffest = actor.Class.Offset1;
			}
		}

		public override void Update(GameTime gameTime, TimeSpan elapsedGameTime)
		{
			double ms = elapsedGameTime.TotalMilliseconds % (GameMain.MSPF * 60);
			this.SrcRectangle = this.TextureRectangle;

			if (ms < GameMain.MSPF * 15)
			{
			}
			else if (ms < GameMain.MSPF * 30)
			{
				this.SrcRectangle.X += this.TextureRectangle.Width;
			}
			else if (ms < GameMain.MSPF * 45)
			{
				this.SrcRectangle.X += this.TextureRectangle.Width * 2;
			}
			else if (ms < GameMain.MSPF * 60)
			{
				this.SrcRectangle.X += this.TextureRectangle.Width;
			}

			if (this.Actor != null)
			{
				this.DestRectangle.X = this.Actor.MapPoint.X * GameMain.CellSize.Width;
				this.DestRectangle.Y = this.Actor.MapPoint.Y * GameMain.CellSize.Height;
			}

			base.Update(gameTime);
		}

		public override object Clone()
		{
			var copy = new ActorWaitAnimation();
			copy.Texture = this.Texture;
			copy.TextureRectangle = this.TextureRectangle;
			copy.DestOffest = this.DestOffest;
			copy.Texture = this.Texture;
			return copy;
		}
	}
}