using System;

namespace MyFirstSRPG.SRPGGame
{
	public class ActorSpeech
	{
		public SceneActor Actor { get; private set; }

		public string Text { get; private set; }

		public bool IsPrimary { get; private set; }

		public TimeSpan PauseTime { get; private set; }

		public ActorSpeech(SceneActor actor, string text, bool isPrimary) : this(actor, text, isPrimary, 0.5f) { }

		public ActorSpeech(SceneActor actor, string text, bool isPrimary, float pauseSeconds)
		{
			this.Actor = actor;
			this.Text = text;
			this.IsPrimary = isPrimary;
			this.PauseTime = TimeSpan.FromSeconds(pauseSeconds);
		}
	}
}