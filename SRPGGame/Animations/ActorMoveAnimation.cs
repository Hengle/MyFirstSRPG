using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGame.Animations
{
	public class ActorMoveAnimation : ActorAnimation
	{
		public SoundEffectInstance sfx;

		private Rectangle lastSrcRectangle;

		public ActorMoveAnimation(SceneActor actor)
			: base(actor)
		{
			this.TextureRectangle = actor.Class.Region2;
			this.DestOffest = actor.Class.Offset2;
			this.sfx = this.GetSfx(this.Actor.Class.Sfx);
			this.lastSrcRectangle = this.TextureRectangle;
			this.DestRectangle.Location = new Point(actor.MapPoint.X * GameMain.CellSize.Width, actor.MapPoint.Y * GameMain.CellSize.Height);
		}

		public override void Update(GameTime gameTime, TimeSpan elapsedGameTime)
		{
			double ms = elapsedGameTime.TotalMilliseconds % (GameMain.MSPF * 40);
			this.SrcRectangle = this.TextureRectangle;

			switch (this.Actor.Direction)
			{
				case Direction.Unknown:
					this.SrcRectangle = this.TextureRectangle;
					break;
				case Direction.Left:
					this.SrcRectangle.Y = this.TextureRectangle.Y;
					break;
				case Direction.Up:
					this.SrcRectangle.Y = this.TextureRectangle.Y + this.TextureRectangle.Height;
					break;
				case Direction.Right:
					this.SrcRectangle.Y = this.TextureRectangle.Y + this.TextureRectangle.Height * 2;
					break;
				case Direction.Down:
					this.SrcRectangle.Y = this.TextureRectangle.Y + this.TextureRectangle.Height * 3;
					break;
			}

			this.lastSrcRectangle = this.SrcRectangle;

			if (ms < GameMain.MSPF * 15)
			{
			}
			else if (ms < GameMain.MSPF * 20)
			{
				this.SrcRectangle.X += this.TextureRectangle.Width;
			}
			else if (ms < GameMain.MSPF * 35)
			{
				this.SrcRectangle.X += this.TextureRectangle.Width * 2;
			}
			else if (ms < GameMain.MSPF * 40)
			{
				this.SrcRectangle.X += this.TextureRectangle.Width * 3;
			}

			if (this.Actor != null && this.Actor.Direction == Direction.Unknown)
			{
				this.DestRectangle.X = this.Actor.MapPoint.X * GameMain.CellSize.Width;
				this.DestRectangle.Y = this.Actor.MapPoint.Y * GameMain.CellSize.Height;
			}

			base.Update(gameTime);
		}

		private SoundEffectInstance GetSfx(int sfxNumber)
		{
			if (sfxNumber > 0)
			{
				return GameMain.ActorSfx[sfxNumber - 1];
			}

			return null;
		}

		public override void PlaySfx()
		{
			GameMain.PlaySfx(this.sfx);
		}

		public override object Clone()
		{
			return new ActorMoveAnimation(this.Actor);
		}
	}
}