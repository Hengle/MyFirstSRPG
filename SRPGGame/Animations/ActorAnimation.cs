using System;
using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGame.Animations
{
	public abstract class ActorAnimation : Animation
	{
		public SceneActor Actor { get; private set; }

		public ActorAnimation(SceneActor actor)
		{
			if (actor != null)
			{
				this.Actor = actor;
				this.CameraFollowing = true;
			}
		}

		public abstract void Update(GameTime gameTime, TimeSpan elapsedGameTime);

		public abstract object Clone();

		public virtual void PlaySfx() { }
	}
}