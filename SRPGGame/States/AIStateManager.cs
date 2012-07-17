using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGame.States
{
	public abstract class AIState
	{
		public Enum Status { get; private set; }

		public AIActor Actor { get; private set; }

		public AIState(AIActor aiActor, Enum status)
		{
			this.Actor = aiActor;
			this.Status = status;
		}

		public virtual void Enter()
		{
			Console.WriteLine("+ " + this.ToString());
		}

		public virtual void Exit()
		{
			Console.WriteLine("- " + this.ToString());
		}

		public virtual void Update(GameTime gameTime) { }

		public virtual void UpdateInput(GameTime gameTime) { }

		public virtual void Draw(GameTime gameTime) { }
	}

	public class AIStateManager<T> : IDisposable
		where T : AIState
	{
		private Stack<T> states;
		private T currentState;

		public T CurrentState
		{
			get
			{
				if (this.currentState == null && this.states.Count > 0)
				{
					this.currentState = this.states.Peek();
				}

				return this.currentState;
			}
		}

		public AIStateManager() : this(null) { }

		public AIStateManager(T initState)
		{
			this.states = new Stack<T>();

			if (initState != null)
			{
				this.states.Push(initState);
			}
		}

		public T AddState(T state)
		{
			this.states.Push(state);
			this.currentState = null;
			return this.CurrentState;
		}

		public T NextState()
		{
			if (this.states.Count > 0)
			{
				this.states.Pop();
				this.currentState = null;
				return this.CurrentState;
			}

			return null;
		}

		public void Update(GameTime gameTime)
		{
			if (this.CurrentState != null)
			{
				this.CurrentState.Update(gameTime);
			}
		}

		public void UpdateInput(GameTime gameTime)
		{
			if (this.CurrentState != null)
			{
				this.CurrentState.UpdateInput(gameTime);
			}
		}

		public void Draw(GameTime gameTime)
		{
			if (this.CurrentState != null)
			{
				this.CurrentState.Draw(gameTime);
			}
		}

		public void Dispose()
		{
			this.currentState = null;
			this.states.Clear();
		}
	}
}
