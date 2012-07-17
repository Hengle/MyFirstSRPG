using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGame.States
{
	public abstract class GameState<T>
	{
		public Enum Status { get; private set; }

		public T GameObject { get; private set; }

		public GameState(T gameObject, Enum status)
		{
			this.GameObject = gameObject;
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

	public class GameStateManager<TGameObject, TState> : IDisposable
		where TState : GameState<TGameObject>
	{
		private Dictionary<Enum, TState> states;

		public TState CurrentState { get; private set; }

		public GameStateManager() : this(null) { }

		public GameStateManager(TState initState)
		{
			this.states = new Dictionary<Enum, TState>();

			if (initState != null)
			{
				this.states.Add(initState.Status, initState);
				this.CurrentState = initState;
			}
		}

		public void AddState(TState state, bool isCurrentState = false)
		{
			if (!this.states.ContainsKey(state.Status))
			{
				this.states.Add(state.Status, state);
			}

			if (isCurrentState)
			{
				this.ChangeState(state.Status);
			}
		}

		public void ChangeState(Enum status, bool reenter = false)
		{
			if (!this.states.ContainsKey(status))
			{
				throw new ArgumentOutOfRangeException("status");
			}

			if (this.CurrentState != null)
			{
				if (this.CurrentState.Status.Equals(status) && !reenter)
				{
					return;
				}

				this.CurrentState.Exit();
			}

			this.CurrentState = this.states[status];
			this.CurrentState.Enter();
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
			this.CurrentState = null;
			this.states.Clear();
		}
	}
}