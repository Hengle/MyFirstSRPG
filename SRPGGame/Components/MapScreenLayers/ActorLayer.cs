using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGame.GameScreens.SceneScreens;
using MyFirstSRPG.SRPGGame.States;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGame.Components.MapScreenLayers
{
	public class ActorLayer : MapScreenLayer
	{
		public Point[] MoveRange;
		public AttackRangePoint[] AttackRange;
		public readonly GameStateManager<ActorLayer, PlayState> StateManager;

		private Color color = new Color(0xff, 0xff, 0xff, 0x60);
		private Rectangle moveRangeSrcDest;
		private Rectangle attackRangeSrcDest;
		private TimeSpan animationTime;

		private SceneActor SelectedActor;
		private SceneActor TargetActor;
		private Point? TargetMapPoint;
		public bool IsActorLvup;
		private TurnPhase LastTurnPhase;

		public ActorLayer(MapScreen screen)
			: base(screen)
		{
			this.StateManager = new GameStateManager<ActorLayer, PlayState>();
			this.StateManager.AddState(new NpcSelectedState(this));
			this.StateManager.AddState(new PlayerSelectedState(this));
			this.StateManager.AddState(new PlayerMoveState(this));
			this.StateManager.AddState(new PlayerInteractState(this));
			this.StateManager.AddState(new PlayerAimState(this));
			this.StateManager.AddState(new CombatInfoState(this));
			this.StateManager.AddState(new CombatState(this));
			this.StateManager.AddState(new ExploreState(this), true);
			this.LastTurnPhase = this.Scene.Phase;

			this.moveRangeSrcDest = new Rectangle(100, 0, 16, 16);
			this.attackRangeSrcDest = new Rectangle(84, 0, 16, 16);
		}

		public override void Update(GameTime gameTime)
		{
			if (this.Scene.Phase != this.LastTurnPhase)
			{
				this.LastTurnPhase = this.Scene.Phase;
				this.StateManager.ChangeState(PlayStatus.Explore, true);
			}

			this.StateManager.Update(gameTime);

			this.animationTime += gameTime.ElapsedGameTime;

			if (this.animationTime.TotalMilliseconds >= GameMain.MSPF * 120)
			{
				this.animationTime = TimeSpan.Zero;
			}

			foreach (var actor in this.Scene.Actors)
			{
				actor.Update(gameTime, this.animationTime);
			}
		}

		public override void UpdateInput(GameTime gameTime)
		{
			this.StateManager.UpdateInput(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			this.StateManager.Draw(gameTime);

			this.DrawActors(gameTime);
		}

		private void DrawRange()
		{
			if (this.MoveRange == null && this.AttackRange == null)
			{
				return;
			}

			this.Scene.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, null);

			Rectangle destRactangle;

			if (this.AttackRange != null)
			{
				foreach (var p in this.AttackRange)
				{
					destRactangle = new Rectangle(p.MapPoint.X * GameMain.CellSize.Width - GameMain.Camera.Location.X, p.MapPoint.Y * GameMain.CellSize.Height - GameMain.Camera.Location.Y, GameMain.CellSize.Width, GameMain.CellSize.Height);
					this.Scene.SpriteBatch.Draw(GameMain.SysTexture, destRactangle, this.moveRangeSrcDest, this.color);
				}
			}

			if (this.MoveRange != null)
			{
				foreach (var p in this.MoveRange)
				{
					destRactangle = new Rectangle(p.X * GameMain.CellSize.Width - GameMain.Camera.Location.X, p.Y * GameMain.CellSize.Height - GameMain.Camera.Location.Y, GameMain.CellSize.Width, GameMain.CellSize.Height);
					this.Scene.SpriteBatch.Draw(GameMain.SysTexture, destRactangle, this.attackRangeSrcDest, this.color);
					//this.Scene.SpriteBatch.DrawString(GameMain.NormalFont, this.Scene.Terrains[p.X, p.Y].MPCost.ToString("d2"), destRactangle.Location.ToVector2(), this.color);
				}
			}

			this.Scene.SpriteBatch.End();
		}

		private void DrawActors(GameTime gameTime)
		{
			this.Scene.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			foreach (var actor in this.Scene.Actors.OrderBy(a => a.MapPoint, new PointComparer()))
			{
				actor.Draw(this.Scene.SpriteBatch, gameTime);
			}

			this.Scene.SpriteBatch.End();
		}

		private void ShowMapMenu(Point mapPoint)
		{
			bool isToLeft = (mapPoint.X + GameMain.Camera.Location.X / GameMain.CellSize.Width) > this.Scene.MapSize.Width / 2;
			var menuScreen = new PopupMenuScreen(this, isToLeft);
			menuScreen.AddOptions(
				PopupMenuScreen.MenuOptionTroop,
				PopupMenuScreen.MenuOptionBrief,
				PopupMenuScreen.MenuOptionOptions,
				PopupMenuScreen.MenuOptionSave,
				PopupMenuScreen.MenuOptionEndPhase
			);

			this.Map.ScreenManager.AddScreen(menuScreen);
		}

		private void ShowInteractMenu()
		{
			Point mapPoint = this.SelectedActor.MapPoint;
			bool isToLeft = (mapPoint.X + GameMain.Camera.Location.X / GameMain.CellSize.Width) > this.Scene.MapSize.Width / 2;
			var menuScreen = new PopupMenuScreen(this, isToLeft);
			menuScreen.CurrentActor = this.SelectedActor;

			SceneActor actor;

			foreach (var p in GameMain.GetAttackRange(this.SelectedActor))
			{
				actor = this.Scene.FindActor(p.MapPoint);

				if (actor != null && actor.Faction == Faction.Enemy)
				{
					menuScreen.AddOptions(PopupMenuScreen.MenuOptionAttack);
					break;
				}
			}

			menuScreen.AddOptions(
				PopupMenuScreen.MenuOptionItem,
				PopupMenuScreen.MenuOptionRest
			);

			this.Map.ScreenManager.AddScreen(menuScreen);
		}

		private void UnselectActor()
		{
			this.SelectedActor.StateManager.ChangeState(ActorStatus.Wait);
			this.SelectedActor = null;
			this.MoveRange = null;
			this.AttackRange = null;
			this.StateManager.ChangeState(PlayStatus.Explore);
			GameMain.PlaySfx(GameMain.SysSfx[2]);
		}

		private void ShowCombatInfo()
		{
			bool isToLeft = (this.TargetActor.MapPoint.X + GameMain.Camera.Location.X / GameMain.CellSize.Width) > this.Scene.MapSize.Width / 2;
			var combatInfoScreen = new CombatInfoScreen(this, this.SelectedActor, this.TargetActor, isToLeft);
			this.Scene.ScreenManager.AddScreen(combatInfoScreen);
		}

		private void StartCombat(SceneActor attacker, SceneActor defender)
		{
			bool isToTop = (attacker.MapPoint.Y + GameMain.Camera.Location.Y / GameMain.CellSize.Height) > this.Scene.MapSize.Height / 2;
			var combatScreen = new CombatScreen(this.Scene, attacker, defender, isToTop);
			combatScreen.OnScreenRemoved += (s, e) => this.StateManager.ChangeState(PlayStatus.Explore);
			this.Scene.ScreenManager.AddScreen(combatScreen);
		}

		#region states

		public abstract class PlayState : GameState<ActorLayer>
		{
			protected MapScreen Map;
			protected SceneScreen Scene;

			public PlayState(ActorLayer layer, PlayStatus status)
				: base(layer, status)
			{
				this.Map = layer.Map;
				this.Scene = this.Map.Scene;
			}
		}

		private class ExploreState : PlayState
		{
			private Queue<AIActor> aiQueue;

			public ExploreState(ActorLayer layer)
				: base(layer, PlayStatus.Explore)
			{
			}

			public override void Enter()
			{
				base.Enter();

				this.GameObject.MoveRange = null;
				this.GameObject.AttackRange = null;
				this.aiQueue = new Queue<AIActor>();
				IEnumerable<SceneActor> actors = null;

				if (this.Scene.Phase == TurnPhase.EnemyPhase)
				{
					actors = this.Scene.Actors.Where(a => a.Faction == Faction.Enemy && a.Status == ActorStatus.Wait);
				}
				else if (this.Scene.Phase == TurnPhase.AllyPhase)
				{
					actors = this.Scene.Actors.Where(a => a.Faction == Faction.Ally && a.Status == ActorStatus.Wait);
				}

				if (actors != null)
				{
					foreach (var aiActor in actors.OfType<AIActor>().OrderBy(a => a.ID))
					{
						this.aiQueue.Enqueue(aiActor);
					}
				}
			}

			public override void Update(GameTime gameTime)
			{
				base.Update(gameTime);

				if (this.aiQueue.Count > 0)
				{
					var ai = this.aiQueue.Peek();

					switch (ai.AIStatus)
					{
						case AIStatus.Idle:
							ai.StartAI();
							break;
						case AIStatus.Finished:
							this.aiQueue.Dequeue();
							break;
					}
				}
			}

			public override void UpdateInput(GameTime gameTime)
			{
				if (this.Scene.Phase != TurnPhase.PlayerPhase)
				{
					return;
				}

				if (Input.Mouse.IsMouseOver(GameMain.SafeArea))
				{
					if (Input.Mouse.IsLeftButtonClicked())
					{
						Point mapPoint = GameMain.GetMapPoint(Input.Mouse);
						SceneActor actor = this.Scene.FindActor(mapPoint);

						if (actor == null || actor.Status == ActorStatus.Rest)
						{
							this.GameObject.ShowMapMenu(mapPoint);
						}
						else
						{
							this.GameObject.SelectedActor = actor;
							this.GameObject.MoveRange = GameMain.GetMoveRange(actor.MapPoint, actor.ActualMOV);
							this.GameObject.AttackRange = GameMain.GetAttackRange(this.GameObject.MoveRange, actor.WeaponRanges);

							switch (actor.Faction)
							{
								case Faction.Player:
									this.GameObject.StateManager.ChangeState(PlayStatus.PlayerSelected);
									break;
								case Faction.Enemy:
								case Faction.Ally:
									this.GameObject.StateManager.ChangeState(PlayStatus.NpcSelected);
									break;
							}
						}
					}
					else if (Input.Mouse.IsRightButtonClicked())
					{
						Point mapPoint = GameMain.GetMapPoint(Input.Mouse);
						SceneActor actor = this.Scene.FindActor(mapPoint);

						if (actor != null)
						{
							var infoScreen = new ActorInfoScreen(actor);
							this.Scene.ScreenManager.AddScreen(infoScreen);
						}
					}
				}
			}
		}

		private class NpcSelectedState : PlayState
		{
			public NpcSelectedState(ActorLayer layer) : base(layer, PlayStatus.NpcSelected) { }

			public override void Enter()
			{
				base.Enter();

				this.GameObject.SelectedActor.StateManager.ChangeState(ActorStatus.Select);
			}

			public override void UpdateInput(GameTime gameTime)
			{
				if (Input.Mouse.IsLeftButtonClicked())
				{
					this.GameObject.UnselectActor();
				}
			}

			public override void Draw(GameTime gameTime)
			{
				this.GameObject.DrawRange();
			}
		}

		private class PlayerSelectedState : PlayState
		{
			public PlayerSelectedState(ActorLayer layer)
				: base(layer, PlayStatus.PlayerSelected)
			{
			}

			public override void Enter()
			{
				base.Enter();

				this.GameObject.SelectedActor.StateManager.ChangeState(ActorStatus.Select);
			}

			public override void UpdateInput(GameTime gameTime)
			{
				if (Input.Mouse.IsLeftButtonClicked())
				{
					Point mapPoint = GameMain.GetMapPoint(Input.Mouse);

					if (this.GameObject.MoveRange.Contains(mapPoint))
					{
						SceneActor actor = this.Scene.FindActor(mapPoint);

						if (actor == null || actor == this.GameObject.SelectedActor)
						{
							// clicked on empty map point.
							this.GameObject.TargetMapPoint = mapPoint;
							this.GameObject.StateManager.ChangeState(PlayStatus.PlayerMove);
						}
						else
						{
							// clicked on another player.
							// do nothing.
						}
					}
					else
					{
						this.GameObject.UnselectActor();
					}
				}
			}

			public override void Draw(GameTime gameTime)
			{
				this.GameObject.DrawRange();
			}
		}

		private class PlayerMoveState : PlayState
		{
			public PlayerMoveState(ActorLayer layer)
				: base(layer, PlayStatus.PlayerMove)
			{
			}

			public override void Enter()
			{
				this.GameObject.SelectedActor.Move(
					this.GameObject.TargetMapPoint.Value,
					() => this.GameObject.StateManager.ChangeState(PlayStatus.PlayerInteract)
				);
			}
		}

		private class PlayerInteractState : PlayState
		{
			public PlayerInteractState(ActorLayer layer)
				: base(layer, PlayStatus.PlayerInteract)
			{
			}

			public override void Enter()
			{
				base.Enter();

				this.GameObject.SelectedActor.StateManager.ChangeState(ActorStatus.Interact);
				this.GameObject.ShowInteractMenu();
			}
		}

		private class PlayerAimState : PlayState
		{
			public PlayerAimState(ActorLayer layer) : base(layer, PlayStatus.PlayerAim) { }

			public override void Enter()
			{
				base.Enter();
				this.GameObject.MoveRange = null;
				this.GameObject.AttackRange = GameMain.GetAttackRange(this.GameObject.SelectedActor);
			}

			public override void UpdateInput(GameTime gameTime)
			{
				if (Input.Mouse.IsLeftButtonClicked())
				{
					Point mapPoint = GameMain.GetMapPoint(Input.Mouse);

					if (this.GameObject.AttackRange.Select(p => p.MapPoint).Contains(mapPoint))
					{
						SceneActor actor = this.Scene.FindActor(mapPoint);

						if (actor != null && actor.Faction == Faction.Enemy)
						{
							this.GameObject.TargetActor = actor;
							this.GameObject.StateManager.ChangeState(PlayStatus.CombatInfo);
							return;
						}
					}

					this.GameObject.StateManager.ChangeState(PlayStatus.PlayerInteract);
				}
			}

			public override void Draw(GameTime gameTime)
			{
				this.GameObject.DrawRange();
			}
		}

		private class CombatInfoState : PlayState
		{
			public CombatInfoState(ActorLayer layer) : base(layer, PlayStatus.CombatInfo) { }

			public override void Enter()
			{
				base.Enter();
				this.GameObject.ShowCombatInfo();
			}
		}

		private class CombatState : PlayState
		{
			public CombatState(ActorLayer layer) : base(layer, PlayStatus.Combat) { }

			public override void Enter()
			{
				base.Enter();
				this.GameObject.StartCombat(this.GameObject.SelectedActor, this.GameObject.TargetActor);
			}
		}

		#endregion states
	}

	public enum PlayStatus
	{
		Explore,
		NpcSelected,
		PlayerSelected,
		PlayerMove,
		PlayerInteract,
		PlayerAim,
		CombatInfo,
		Combat,
		AIInit
	}
}