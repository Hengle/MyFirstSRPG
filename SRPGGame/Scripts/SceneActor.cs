using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGame.Animations;
using MyFirstSRPG.SRPGGame.States;
using MyFirstSRPG.SRPGGameLibrary;
using MyFirstSRPG.SRPGGame.GameScreens.SceneScreens;

namespace MyFirstSRPG.SRPGGame
{
	public class SceneActor : Actor
	{
		#region fields & properties
		public ActorStatus Status { get; protected set; }

		public ActorWaitAnimation AnimationWait { get; private set; }

		public ActorWaitAnimation AnimationRest { get; private set; }

		public ActorMoveAnimation AnimationMove { get; private set; }

		public Rectangle FaceRect { get; private set; }

		public Direction Direction;

		public Point LastMapPoint { get; protected set; }

		public SceneScreen Scene;
		public Faction Faction;
		public Point MapPoint;
		public bool Visible;
		public Point[] MoveRange;
		public AttackRangePoint[] AttackRange;

		public GameStateManager<SceneActor, ActorState> StateManager { get; protected set; }
		protected bool IsLoaded;

		private SpriteBatch SpriteBatch;
		private Point[] MoveTracks;
		private Action ActionOnMoved;
		#endregion

		public SceneActor(Actor actor, Point faceLocation)
			: base(actor.ID, actor.ClassID, actor.LV, actor.EXP, actor.Name, actor.Basics, actor.PerkIDs)
		{
			this.FaceRect = new Rectangle(faceLocation.X, faceLocation.Y, 48, 64);
			this.Status = ActorStatus.Wait;
			this.Direction = Direction.Unknown;
		}

		public void LoadContent()
		{
			if (!IsLoaded)
			{
				this.AnimationWait = new ActorWaitAnimation(this) { Texture = this.GetTexture() };
				this.AnimationRest = new ActorWaitAnimation(this) { Texture = GameMain.ActorTextures[3] };
				this.AnimationMove = new ActorMoveAnimation(this) { Texture = this.GetMoveTexture() };
				this.SetStates();
				this.IsLoaded = true;
			}
		}

		protected virtual void SetStates()
		{
			this.StateManager = new GameStateManager<SceneActor, ActorState>();
			this.StateManager.AddState(new RestState(this));
			this.StateManager.AddState(new SelectedState(this));
			this.StateManager.AddState(new MoveState(this));
			this.StateManager.AddState(new InteractState(this));
			this.StateManager.AddState(new AimState(this));
			this.StateManager.AddState(new AttackState(this));
			this.StateManager.AddState(new DefendState(this));
			this.StateManager.AddState(new AvoidState(this));
			this.StateManager.AddState(new DieState(this));
			this.StateManager.AddState(new WaitState(this), true);
		}

		public virtual void Update(GameTime gameTime, TimeSpan animationTime)
		{
			if (this.Visible)
			{
				this.StateManager.CurrentState.AnimationTime = animationTime;
				this.StateManager.Update(gameTime);
			}
		}

		public virtual void Draw(SpriteBatch sb, GameTime gameTime)
		{
			if (this.Visible)
			{
				this.SpriteBatch = sb;
				this.StateManager.CurrentState.Draw(gameTime);
			}
		}

		private void DrawAnimation(Animation animation, GameTime gameTime)
		{
			animation.Draw(this.SpriteBatch, gameTime);
		}

		private Texture2D GetTexture()
		{
			Texture2D texture;

			switch (this.Faction)
			{
				case Faction.Player:
					texture = GameMain.ActorTextures[0];
					break;
				case Faction.Enemy:
					texture = GameMain.ActorTextures[1];
					break;
				case Faction.Ally:
					texture = GameMain.ActorTextures[2];
					break;
				default:
					texture = GameMain.ActorTextures[1];
					break;
			}

			return texture;
		}

		private Texture2D GetMoveTexture()
		{
			Texture2D texture;

			switch (this.Faction)
			{
				case Faction.Player:
					texture = GameMain.ActorMoveTextures[0];
					break;
				case Faction.Enemy:
					texture = GameMain.ActorMoveTextures[1];
					break;
				case Faction.Ally:
					texture = GameMain.ActorMoveTextures[2];
					break;
				default:
					texture = GameMain.ActorMoveTextures[0];
					break;
			}

			return texture;
		}

		private ActorAnimation GetAnimation(ActorStatus status)
		{
			ActorAnimation animation;

			switch (status)
			{
				case ActorStatus.Rest:
					animation = this.AnimationRest;
					break;
				case ActorStatus.Wait:
					animation = this.AnimationWait;
					break;
				default:
					animation = this.AnimationMove;
					break;
			}

			return animation;
		}

		public int[] GetGrowth()
		{
			Console.Write("HP: {0}, ", this.Growth.HP);
			int hp = this.Growth.HP >= GameMain.RandomNumber ? 1 : 0;
			Console.Write("STR: {0}, ", this.Growth.STR);
			int str = this.Growth.STR >= GameMain.RandomNumber ? 1 : 0;
			Console.Write("MAG: {0}, ", this.Growth.MAG);
			int mag = this.Growth.MAG >= GameMain.RandomNumber ? 1 : 0;
			Console.Write("SKL: {0}, ", this.Growth.SKL);
			int skl = this.Growth.SKL >= GameMain.RandomNumber ? 1 : 0;
			Console.Write("SPD: {0}, ", this.Growth.SPD);
			int spd = this.Growth.SPD >= GameMain.RandomNumber ? 1 : 0;
			Console.Write("LUK: {0}, ", this.Growth.LUK);
			int luk = this.Growth.LUK >= GameMain.RandomNumber ? 1 : 0;
			Console.Write("DEF: {0}, ", this.Growth.DEF);
			int def = this.Growth.DEF >= GameMain.RandomNumber ? 1 : 0;
			Console.Write("BLD: {0}, ", this.Growth.BLD);
			int bld = this.Growth.BLD >= GameMain.RandomNumber ? 1 : 0;

			return new int[8] { hp, str, mag, skl, spd, luk, def, bld };
		}

		public void ClearRange()
		{
			this.MoveRange = null;
			this.AttackRange = null;
		}

		public void SetFaceDirection(Point targetPoint)
		{
			// 
		}

		public void Move(Point destMapPoint, Action actionOnMoved = null)
		{
			this.Move(this.MapPoint, destMapPoint, actionOnMoved);
		}

		public void Move(Point srcMapPoint, Point destMapPoint, Action actionOnMoved = null)
		{
			if (this.Status == ActorStatus.Move)
			{
				return;
			}

			this.Visible = true;
			this.LastMapPoint = this.MapPoint = srcMapPoint;
			this.MoveTracks = GameMain.GetMoveTracks(this.MapPoint, destMapPoint, this.ActualMOV);
			this.ActionOnMoved = actionOnMoved;
			this.StateManager.ChangeState(ActorStatus.Move);
		}

		#region states

		#region ActorState

		public abstract class ActorState : GameState<SceneActor>
		{
			public TimeSpan AnimationTime;

			protected ActorAnimation Animation { get; private set; }

			public ActorState(SceneActor gameObject, ActorStatus status)
				: base(gameObject, status)
			{
				this.Animation = gameObject.GetAnimation(status);
			}

			public override void Enter()
			{
				base.Enter();
				this.GameObject.Status = (ActorStatus)this.Status;
			}

			public override void Update(GameTime gameTime)
			{
				this.Animation.Update(gameTime, this.AnimationTime);
			}

			public override void Draw(GameTime gameTime)
			{
				this.GameObject.DrawAnimation(this.Animation, gameTime);
			}
		}

		#endregion ActorState

		#region WaitState
		protected class WaitState : ActorState
		{
			public WaitState(SceneActor gameObject)
				: base(gameObject, ActorStatus.Wait) { }

			public override void Enter()
			{
				base.Enter();
				this.GameObject.Direction = Direction.Unknown;
				this.GameObject.ClearRange();
			}

			public override void Draw(GameTime gameTime)
			{
				base.Draw(gameTime);
			}
		}
		#endregion

		#region RestState
		protected class RestState : ActorState
		{
			public RestState(SceneActor gameObject)
				: base(gameObject, ActorStatus.Rest) { }

			public override void Enter()
			{
				base.Enter();
				this.GameObject.Direction = Direction.Unknown;
				this.GameObject.Scene.MoveActor(this.GameObject, this.GameObject.MapPoint);
			}
		}
		#endregion

		#region SelectedState
		private class SelectedState : ActorState
		{
			public SelectedState(SceneActor gameObject)
				: base(gameObject, ActorStatus.Select) { }

			public override void Enter()
			{
				base.Enter();
				this.GameObject.Direction = Direction.Unknown;
				this.GameObject.MoveRange = GameMain.GetMoveRange(this.GameObject.MapPoint, this.GameObject.ActualMOV);
				this.GameObject.AttackRange = GameMain.GetAttackRange(this.GameObject.MoveRange, this.GameObject.WeaponRanges);
				GameMain.PlaySfx(GameMain.SysSfx[1]);
			}
		}
		#endregion

		#region MoveState
		protected class MoveState : ActorState
		{
			private TimeSpan moveTime;

			public MoveState(SceneActor gameObject)
				: base(gameObject, ActorStatus.Move) { }

			public override void Enter()
			{
				base.Enter();
				this.moveTime = TimeSpan.Zero;
			}

			public override void Update(GameTime gameTime)
			{
				this.UpdateMovement(gameTime);
				this.Animation.Update(gameTime, this.AnimationTime);
			}

			private void UpdateMovement(GameTime gameTime)
			{
				//if (!this.isMoving)
				//{
				//    return;
				//}

				if (this.GameObject.MoveTracks == null || this.GameObject.MoveTracks.Length == 0)
				{
					this.EndMove();
					return;
				}

				this.moveTime += gameTime.ElapsedGameTime;
				double msPerFrame = GameMain.MSPF * 8;
				int trackIndex = (int)Math.Floor(this.moveTime.TotalMilliseconds / msPerFrame);

				if (trackIndex + 1 >= this.GameObject.MoveTracks.Length)
				{
					this.GameObject.MapPoint = this.GameObject.MoveTracks.Last();
					this.EndMove();
					return;
				}

				this.GameObject.MapPoint = this.GameObject.MoveTracks[trackIndex];
				this.GameObject.Direction = this.GameObject.MapPoint.GetFaceDirection(this.GameObject.MoveTracks[trackIndex + 1]);
				double stepRate = (this.moveTime.TotalMilliseconds % msPerFrame) / msPerFrame;
				Point destLocation = new Point(this.GameObject.MapPoint.X * GameMain.CellSize.Width, this.GameObject.MapPoint.Y * GameMain.CellSize.Height);

				if ((this.GameObject.Direction & Direction.Up) == Direction.Up)
				{
					destLocation.Y -= (int)(stepRate * GameMain.CellSize.Height);
				}
				else if ((this.GameObject.Direction & Direction.Right) == Direction.Right)
				{
					destLocation.X += (int)(stepRate * GameMain.CellSize.Width);
				}
				else if ((this.GameObject.Direction & Direction.Down) == Direction.Down)
				{
					destLocation.Y += (int)(stepRate * GameMain.CellSize.Height);
				}
				else if ((this.GameObject.Direction & Direction.Left) == Direction.Left)
				{
					destLocation.X -= (int)(stepRate * GameMain.CellSize.Width);
				}

				this.Animation.PlaySfx();
				this.Animation.SetDestLocation(destLocation);
			}

			private void EndMove()
			{
				this.GameObject.MoveTracks = null;

				if (this.GameObject.ActionOnMoved == null)
				{
					this.GameObject.StateManager.ChangeState(ActorStatus.Wait);
				}
				else
				{
					this.GameObject.ActionOnMoved();
				}
			}
		}
		#endregion

		#region InteractState
		private class InteractState : ActorState
		{
			public InteractState(SceneActor gameObject)
				: base(gameObject, ActorStatus.Interact) { }

			public override void Enter()
			{
				base.Enter();
				this.Animation.SetDestLocation(this.GameObject.MapPoint.X * GameMain.CellSize.Width, this.GameObject.MapPoint.Y * GameMain.CellSize.Height);
			}
		}
		#endregion

		#region AimState
		private class AimState : ActorState
		{
			public AimState(SceneActor gameObject)
				: base(gameObject, ActorStatus.Aim) { }

			public override void Enter()
			{
				base.Enter();
				this.Animation.SetDestLocation(this.GameObject.MapPoint.X * GameMain.CellSize.Width, this.GameObject.MapPoint.Y * GameMain.CellSize.Height);
			}
		}
		#endregion

		#region AttackState
		protected class AttackState : ActorState
		{
			public AttackState(SceneActor gameObject) : base(gameObject, ActorStatus.Attack) { }
		}
		#endregion

		#region DefendState
		protected class DefendState : ActorState
		{
			public DefendState(SceneActor gameObject) : base(gameObject, ActorStatus.Defend) { }
		}
		#endregion

		#region AvoidState
		protected class AvoidState : ActorState
		{
			public AvoidState(SceneActor gameObject) : base(gameObject, ActorStatus.Avoid) { }
		}
		#endregion

		#region DieState
		protected class DieState : ActorState
		{
			public DieState(SceneActor gameObject) : base(gameObject, ActorStatus.Die) { }
		}
		#endregion

		#endregion states
	}

	public enum ActorStatus
	{
		Wait,
		Rest,
		Select,
		Move,
		Interact,
		Aim,
		Attack,
		Defend,
		Avoid,
		Die,
		AIAct
	}
}