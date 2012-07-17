using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGame.States;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGame.GameScreens.SceneScreens
{
	public class SceneScreen : GameScreen
	{
		public event EventHandler OnTurnStarted;

		public event EventHandler OnTurnEnded;

		public event EventHandler OnPhaseChanged;

		public SceneScript Script { get; private set; }

		public string Title { get; private set; }

		public Size MapSize { get; private set; }

		public Terrain[,] Terrains { get; private set; }

		public Size TileSize { get; private set; }

		public TurnPhase Phase { get; private set; }

		public int Turns { get; private set; }

		public List<SceneActor> Actors { get; private set; }

		public bool IsInputReady { get { return this.Phase != TurnPhase.EventPhase; } }

		private GameStateManager<SceneScreen, PhaseState> stateManager;
		private PlayerPhaseState playerPhase;
		private EnemyPhaseState enemyPhase;
		private AllyPhaseState allyPhase;
		private EventPhaseState eventPhase;
		private Dictionary<Point, SceneActor> actorsOnMap;

		public SceneScreen(SceneScript script)
		{
			this.Script = script;
			this.Title = this.Script.Title;
			this.MapSize = this.Script.MapSize;
			this.Terrains = this.Script.MapData;
			this.TileSize = this.Script.TileSize;
			this.stateManager = new GameStateManager<SceneScreen, PhaseState>();
			this.stateManager.AddState(this.playerPhase = new PlayerPhaseState(this));
			this.stateManager.AddState(this.enemyPhase = new EnemyPhaseState(this));
			this.stateManager.AddState(this.allyPhase = new AllyPhaseState(this));
			this.stateManager.AddState(this.eventPhase = new EventPhaseState(this));
			this.actorsOnMap = new Dictionary<Point, SceneActor>();
			this.Actors = new List<SceneActor>();

			GameMain.PathFinder.SetPathPoints(this.Terrains);
		}

		public override void LoadContent()
		{
			GameMain.Camera.SetCamera(new Size(this.MapSize.Width * GameMain.CellSize.Width, this.MapSize.Height * GameMain.CellSize.Height), GameMain.CellSize.ToPoint());
			GameMain.Camera.Location.Y = this.MapSize.Height * GameMain.CellSize.Height - GameMain.ScreenBounds.Height;

			GameMain.ActorTextures = this.Content.Load<Texture2D[]>("Texture/Act01");
			GameMain.ActorMoveTextures = this.Content.Load<Texture2D[]>("Texture/ActM01");
			GameMain.ActorFaceTexture = this.Content.Load<Texture2D>("Texture/Face01");

			this.ScreenManager.AddScreen(new MapScreen(this, this.Script.TileSheet));
			//this.ScreenManager.AddScreen(new SceneTitleScreen(this));
			//this.ScreenManager.AddScreen(new ActorInfoScreen(this, null));
			this.StartScene();
		}

		public override void Update(GameTime gameTime, bool isCovered, bool isFocusLost)
		{
			base.Update(gameTime, isCovered, isFocusLost);

			this.stateManager.Update(gameTime);
		}

		private void AddActorInternal(SceneActor actor)
		{
			if (!this.Actors.Contains(actor))
			{
				this.Actors.Add(actor);
				actor.Scene = this;
			}

			this.actorsOnMap[actor.MapPoint] = actor;
		}

		public void RemoveActor(SceneActor actor)
		{
			if (this.Actors.Contains(actor))
			{
				this.Actors.Remove(actor);
				this.actorsOnMap[actor.MapPoint] = null;
			}
		}

		public void MoveActor(SceneActor actor, Point mapPoint)
		{
			if (this.actorsOnMap.Values.Contains(actor))
			{
				this.actorsOnMap[actor.MapPoint] = null;
			}

			this.actorsOnMap[mapPoint] = actor;
		}

		public SceneActor FindActor(Point mapPoint)
		{
			if (this.actorsOnMap.ContainsKey(mapPoint))
			{
				return this.actorsOnMap[mapPoint];
			}

			return null;
		}

		public void StartScene()
		{
			this.Script.LoadScene(this);
			this.StartTurn();
		}

		public void StartTurn()
		{
			this.Turns++;

			if (this.OnTurnStarted != null)
			{
				this.OnTurnStarted(this, null);
			}
		}

		public void EndTurn()
		{
			if (this.OnTurnEnded != null)
			{
				this.OnTurnEnded(this, null);
			}
		}

		public void EndPhase()
		{
			this.ChangePhase(this.stateManager.CurrentState.NextPhase);
		}

		public void ChangePhase(TurnPhase phase)
		{
			this.Phase = phase;

			if (this.Phase != TurnPhase.EventPhase)
			{
				this.ScreenManager.AddScreen(new PhaseScreen(this, this.Phase));
			}

			this.stateManager.ChangeState(this.Phase);

			if (this.OnPhaseChanged != null)
			{
				this.OnPhaseChanged(this, null);
			}
		}

		public void AddActionPause(double pauseMS)
		{
			this.AddDelayAction(null, pauseMS);
		}

		public void AddDelayAction(Action action, double pauseMS = 0d)
		{
			this.eventPhase.AddDelayAction(action, pauseMS);
		}

		public void AddActionLoadActor(SceneActor actor, double pauseMS = 0d)
		{
			actor.LoadContent();
			this.AddDelayAction(() => this.AddActorInternal(actor), pauseMS);
		}

		public void AddActionUnLoadActor(SceneActor actor, double pauseMS = 0d)
		{
			this.AddDelayAction(() => this.RemoveActor(actor), pauseMS);
		}

		public void AddActionMoveActor(SceneActor actor, Point destMapPoint, double pauseMS = 0d)
		{
			this.AddDelayAction(delegate()
			{
				actor.Move(destMapPoint);
				this.MoveActor(actor, destMapPoint);
			}, pauseMS);
		}

		public void AddActionMoveActor(SceneActor actor, Point srcMapPoint, Point destMapPoint, double pauseMS = 0d)
		{
			this.AddDelayAction(delegate()
			{
				actor.Move(srcMapPoint, destMapPoint);
				this.MoveActor(actor, destMapPoint);
			}, pauseMS);
		}

		public void AddActionSpeech(SceneActor actor, string text, bool isPrimary, float pauseMS = 0.5f)
		{
			this.eventPhase.AddSpeech(actor, text, isPrimary, pauseMS);
		}

		#region states

		private abstract class PhaseState : GameState<SceneScreen>
		{
			public TurnPhase NextPhase { get; protected set; }

			public PhaseState(SceneScreen gameObject, TurnPhase phase)
				: base(gameObject, phase)
			{
			}

			public override void Enter()
			{
				foreach (var actor in this.GameObject.Actors)
				{
					actor.StateManager.ChangeState(ActorStatus.Wait);
				}
			}
		}

		private class PlayerPhaseState : PhaseState
		{
			public PlayerPhaseState(SceneScreen gameObject)
				: base(gameObject, TurnPhase.PlayerPhase)
			{
				this.NextPhase = TurnPhase.EnemyPhase;
			}

			public override void Enter()
			{
				base.Enter();
			}

			public override void Update(GameTime gameTime)
			{
				if (this.GameObject.Actors.Where(a => a.Faction == Faction.Player).All(a => a.Status == ActorStatus.Rest))
				{
					this.GameObject.EndPhase();
				}
			}
		}

		private class EnemyPhaseState : PhaseState
		{
			public EnemyPhaseState(SceneScreen gameObject)
				: base(gameObject, TurnPhase.EnemyPhase)
			{
				this.NextPhase = TurnPhase.AllyPhase;
			}

			public override void Enter()
			{
				base.Enter();

				//this.GameObject.EndPhase();
			}

			public override void Update(GameTime gameTime)
			{
				if (this.GameObject.Actors.Where(a => a.Faction == Faction.Enemy).All(a => a.Status == ActorStatus.Rest))
				{
					this.GameObject.EndPhase();
				}
			}
		}

		private class AllyPhaseState : PhaseState
		{
			public AllyPhaseState(SceneScreen gameObject)
				: base(gameObject, TurnPhase.AllyPhase)
			{
				this.NextPhase = TurnPhase.PlayerPhase;
			}

			public override void Enter()
			{
				base.Enter();

				this.GameObject.EndPhase();
			}

			public override void Update(GameTime gameTime)
			{
				if (this.GameObject.Actors.Where(a => a.Faction == Faction.Ally).All(a => a.Status == ActorStatus.Rest))
				{
					this.GameObject.EndPhase();
				}
			}
		}

		private class EventPhaseState : PhaseState
		{
			private SceneDialogScreen dialogScreen;
			private Queue<DelayedAction> delayedActionQueue;
			private TimeSpan delayTime;

			public EventPhaseState(SceneScreen gameObject)
				: base(gameObject, TurnPhase.EventPhase)
			{
				this.dialogScreen = new SceneDialogScreen(this.GameObject);
				this.delayedActionQueue = new Queue<DelayedAction>();
			}

			public override void Update(GameTime gameTime)
			{
				if (this.dialogScreen.Status != ScreenStatus.Hidden)
				{
					return;
				}

				if (this.delayedActionQueue.Count > 0)
				{
					DelayedAction da = this.delayedActionQueue.Peek();
					this.delayTime += gameTime.ElapsedGameTime;

					if (this.delayTime >= da.DelayTime)
					{
						if (da.Action != null)
						{
							da.Action();
						}

						this.delayedActionQueue.Dequeue();
						this.delayTime = TimeSpan.Zero;
					}
				}
			}

			public void AddDelayAction(Action action, double pauseMS = 0d)
			{
				this.delayedActionQueue.Enqueue(new DelayedAction(TimeSpan.FromMilliseconds(pauseMS), action));
			}

			public void AddSpeech(SceneActor actor, string text, bool isPrimary, float pauseMS = 0.5f)
			{
				this.dialogScreen.AddSpeech(new ActorSpeech(actor, text, isPrimary, pauseMS));
				this.AddDelayAction(() => this.StartDialog());
			}

			private void StartDialog()
			{
				if (this.dialogScreen.SpeechQuene.Count == 0)
				{
					return;
				}

				if (this.dialogScreen.Status == ScreenStatus.Hidden)
				{
					this.dialogScreen.Status = ScreenStatus.TransitionOn;
					this.GameObject.ScreenManager.AddScreen(this.dialogScreen);
				}
			}
		}

		#endregion states
	}

	public enum TurnPhase
	{
		PlayerPhase,
		EnemyPhase,
		AllyPhase,
		EventPhase
	}
}