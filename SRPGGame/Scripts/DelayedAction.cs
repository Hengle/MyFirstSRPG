using System;

namespace MyFirstSRPG.SRPGGame
{
	public class DelayedAction
	{
		public TimeSpan DelayTime { get; private set; }

		public Action Action { get; private set; }

		public DelayedAction(TimeSpan delayTime, Action action)
		{
			this.DelayTime = delayTime;
			this.Action = action;
		}
	}
}