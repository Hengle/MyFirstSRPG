using System;
using Microsoft.Xna.Framework;
using MyFirstSRPG.SRPGGame.GameScreens.SceneScreens;
using System.Collections.Generic;

namespace MyFirstSRPG.SRPGGame.Actions
{
	public abstract class ActionBase
	{
		public bool Complete { get; protected set; }
		public abstract void Update(GameTime gameTime);
	}

	public class PauseAction : ActionBase
	{
		private TimeSpan pauseTime;
		private TimeSpan elapsedTime;

		public PauseAction(TimeSpan pauseTime)
		{
			this.pauseTime = pauseTime;
		}

		public override void Update(GameTime gameTime)
		{
			this.elapsedTime += gameTime.ElapsedGameTime;

			if (this.elapsedTime >= pauseTime)
			{
				this.Complete = true;
			}
		}
	}

	public abstract class SceneAction : ActionBase
	{
		public SceneScreen Scene { get; private set; }

		public SceneAction(SceneScreen scene)
		{
			this.Scene = scene;
		}
	}

	public class PhaseChangeAction:SceneAction
	{
		private TurnPhase phase;

		public PhaseChangeAction(SceneScreen scene, TurnPhase phase):base(scene)
		{
			this.phase = phase;
		}

		public override void Update(GameTime gameTime)
		{
			this.Scene.ChangePhase(this.phase);
			this.Complete = true;
		}
	}

	public class ActorLoadAction : SceneAction
	{
		private SceneActor actor;

		public ActorLoadAction(SceneScreen scene, SceneActor actor)
			: base(scene)
		{
			this.actor = actor;
		}

		public override void Update(GameTime gameTime)
		{
			this.actor.LoadContent();
			this.Scene.AddActor(this.actor);
			this.Complete = true;
		}
	}

	public class ActorUnloadAction : SceneAction
	{
		private SceneActor actor;

		public ActorUnloadAction(SceneScreen scene, SceneActor actor)
			: base(scene)
		{
			this.actor = actor;
		}

		public override void Update(GameTime gameTime)
		{
			this.Scene.RemoveActor(this.actor);
			this.Complete = true;
		}
	}

	public class ActorMoveAction : SceneAction
	{
		private SceneActor actor;
		private Point srcMapPoint;
		private Point destMapPoint;

		public ActorMoveAction(SceneScreen scene, SceneActor actor, Point destMapPoint)
			: this(scene, actor, actor.MapPoint, destMapPoint)
		{ }

		public ActorMoveAction(SceneScreen scene, SceneActor actor, Point srcMapPoint, Point destMapPoint)
			: base(scene)
		{
			this.actor = actor;
			this.srcMapPoint = srcMapPoint;
			this.destMapPoint = destMapPoint;
		}

		public override void Update(GameTime gameTime)
		{
			this.actor.Move(srcMapPoint, destMapPoint);
			this.Scene.MoveActor(this.actor, destMapPoint);
			this.Complete = true;
		}
	}

	public class SceneDialogueAction : SceneAction
	{
		private IEnumerable<ActorSpeech> speeches;

		public SceneDialogueAction(SceneScreen scene, IEnumerable<ActorSpeech> speeches)
			: base(scene)
		{
			this.speeches = speeches;
		}

		public override void Update(GameTime gameTime)
		{
			this.Scene.StartDialogue(this.speeches);
			this.Complete = true;
		}
	}
}