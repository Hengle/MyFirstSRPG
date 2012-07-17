using System;

namespace MyFirstSRPG.SRPGGame
{
	public class ActorSpeech
	{
		public SceneActor Actor { get; private set; }

		public string Text { get; private set; }

		public bool IsLeft { get; private set; }

		public TimeSpan PauseTime { get; private set; }

		public ActorSpeech(SceneActor actor, string text, bool isLeft) : this(actor, text, isLeft, 0.5f) { }

		public ActorSpeech(SceneActor actor, string text, bool isLeft, float pauseSeconds)
		{
			this.Actor = actor;
			this.Text = text;
			this.IsLeft = isLeft;
			this.PauseTime = TimeSpan.FromSeconds(pauseSeconds);
		}
	}
}