using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyFirstSRPG.SRPGGameLibrary;
using MyFirstSRPG.SRPGGame.Animations;
using MyFirstSRPG.SRPGGame.States;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGame.Components.MapScreenLayers;

namespace MyFirstSRPG.SRPGGame
{
	public class AIActor : SceneActor
	{
		public bool Acting;
		public bool Offensive;
		public bool Moveable;
		public int ScoutRange;
		public Point? TargetMapPoint;
		public SceneActor TargetActor;
		public AIStatus AIStatus;

		public AIActor(int id, string name, int classID, int lv, Point faceLocation)
			: this(id, name, classID, lv, 0, new ActorAttributes(), null, faceLocation)
		{ }

		public AIActor(int id, string name, int classID, int lv, int exp, int hp, int str, int mag, int skl, int spd, int luk, int def, int bld, int[] perkIDs, Point faceLocation)
			: this(id, name, classID, lv, exp, new ActorAttributes(hp, str, mag, skl, spd, luk, def, bld), perkIDs, faceLocation)
		{ }

		public AIActor(int id, string name, int classID, int lv, int exp, ActorAttributes basics, int[] perkIDs, Point faceLocation)
			: base(id, name, classID, lv, exp, basics, perkIDs, faceLocation)
		{
			this.AIStatus = AIStatus.Idle;
		}

		protected override void SetStates()
		{
			this.StateManager = new GameStateManager<SceneActor, ActorState>();
			this.StateManager.AddState(new RestState(this));
			this.StateManager.AddState(new MoveState(this));
			this.StateManager.AddState(new AttackState(this));
			this.StateManager.AddState(new DefendState(this));
			this.StateManager.AddState(new AvoidState(this));
			this.StateManager.AddState(new DieState(this));
			this.StateManager.AddState(new WaitState(this), true);
		}

		public void StartAI()
		{
			this.AIStatus = AIStatus.Running;

			if (this.Offensive)
			{
				AttackPlan attackPlan = this.FindAttackPlan(false);

				if (attackPlan != null)
				{
					this.PerformAttackPlan(attackPlan);
				}
				else if (this.Moveable)
				{
					attackPlan = this.FindAttackPlan(true);

					if (attackPlan != null)
					{
						this.PerformAttackPlan(attackPlan);
					}
					else
					{
						if (this.TargetActor != null)
						{
							this.TargetMapPoint = this.TargetActor.MapPoint;
						}

						this.ApproachTargetMapPoint();
					}
				}
				else
				{
					this.EndAI();
				}
			}
			else if (this.Moveable)
			{
				if (this.TargetActor != null)
				{
					this.TargetMapPoint = this.TargetActor.MapPoint;
				}

				this.ApproachTargetMapPoint();
			}
			else
			{
				this.EndAI();
			}
		}

		private void PerformAttackPlan(AttackPlan attackPlan)
		{
			if (attackPlan.TargetMapPoint != this.MapPoint)
			{
				this.Move(attackPlan.TargetMapPoint, () => this.StartCombat(this, attackPlan.Target));
			}
			else
			{
				this.StartCombat(this, attackPlan.Target);
			}
		}

		private void ApproachTargetMapPoint()
		{
			if (this.TargetMapPoint.HasValue)
			{
				this.Approach(this.TargetMapPoint.Value, () => this.EndAI());
			}
			else
			{
				this.EndAI();
			}
		}

		private void Approach(Point destMapPoint, Action actionOnMoved = null)
		{
			this.Approach(this.MapPoint, destMapPoint, actionOnMoved);
		}

		private void Approach(Point srcMapPoint, Point destMapPoint, Action actionOnMoved = null)
		{
			if (this.Status == ActorStatus.Move)
			{
				return;
			}

			this.Visible = true;
			this.LastMapPoint = this.MapPoint = srcMapPoint;
			this.MoveTracks = GameMain.GetApproachTracks(this.MapPoint, destMapPoint, this.ActualMOV);
			this.ActionOnMoved = actionOnMoved;
			this.StateManager.ChangeState(ActorStatus.Move);
		}

		private void StartCombat(AIActor attacker, SceneActor defender)
		{
			bool isToTop = (attacker.MapPoint.Y + GameMain.Camera.Location.Y / GameMain.CellSize.Height) > this.Scene.MapSize.Height / 2;
			var combatScreen = new CombatScreen(this.Scene, attacker, defender, isToTop);
			combatScreen.OnScreenRemoved += (s, e) => this.EndAI();
			this.Scene.ScreenManager.AddScreen(combatScreen);
		}

		private void EndAI()
		{
			this.StateManager.ChangeState(ActorStatus.Rest);
			this.AIStatus = AIStatus.Finished;
		}

		internal AttackPlan FindAttackPlan(bool moveable = false)
		{
			AttackRangePoint[] attackRange;

			if (moveable)
			{
				attackRange = GameMain.GetAttackRange(this.MapPoint, this.ActualMOV, this.WeaponRanges);
			}
			else
			{
				attackRange = GameMain.GetAttackRange(this);
			}

			List<AttackPlan> plans = new List<AttackPlan>();

			foreach (var p in attackRange)
			{
				AttackPlan plan;
				var target = this.Scene.FindActor(p.MapPoint);
				var isEnemy = false;

				if (target != null)
				{
					if (this.Faction == Faction.Enemy)
					{
						isEnemy = target.Faction.In(Faction.Player, Faction.Ally);
					}
					else
					{
						isEnemy = target.Faction == Faction.Enemy;
					}
				}

				if (isEnemy)
				{
					plan = new AttackPlan();
					plan.Target = target;
					plan.Weapon = target.Items.OfType<Weapon>().First(w => w.Range == p.WeaponRange);
					plan.TargetMapPoint = p.SourcePoint;
				}
			}

			return plans.OrderBy(p => p.Target.CurrentHP).FirstOrDefault();
		}

		internal class AttackPlan
		{
			public SceneActor Target;
			public Weapon Weapon;
			public Point TargetMapPoint;
		}

		#region states
		//private abstract class AIActorState : AIState
		//{
		//    public AIActorState(AIActor actor, ActorStatus status)
		//        : base(actor, status)
		//    {
		//    }

		//    public override void Exit()
		//    {
		//        base.Exit();
		//        this.Actor.AIStateManager.NextState();
		//    }
		//}

		//private class AIActState : AIActorState
		//{
		//    public AIActState(AIActor actor)
		//        : base(actor, ActorStatus.AIAct) { }

		//    public override void Enter()
		//    {
		//        base.Enter();

		//        if (this.Actor.Offensive)
		//        {
		//            this.Actor.AIStateManager.AddState(new OffenseState(this.Actor));
		//        }
		//        else
		//        {
		//            if (this.Actor.TargetActor != null)
		//            {
		//                this.Actor.TargetMapPoint = this.Actor.TargetActor.MapPoint;
		//            }

		//            if (this.Actor.TargetMapPoint.HasValue)
		//            {
		//                this.Actor.AIStateManager.AddState(new MarchState(this.Actor));
		//            }
		//        }
		//    }

		//    public override void Update(GameTime gameTime)
		//    {
		//        base.Update(gameTime);
		//    }
		//}

		//private class OffenseState : AIActorState
		//{
		//    public OffenseState(AIActor actor)
		//        : base(actor, ActorStatus.AIAct)
		//    { }

		//    public override void Enter()
		//    {
		//        base.Enter();

		//        var targets = this.Actor.FindAttackPlan();

		//        if (targets.Count() > 0)
		//        {
		//            var target = targets.OrderBy(a => a.ActualHP).First();
		//            bool isToTop = (this.Actor.MapPoint.Y + GameMain.Camera.Location.Y / GameMain.CellSize.Height) > this.Actor.Scene.MapSize.Height / 2;
		//            var combatScreen = new CombatScreen(this.Actor.Scene, this.Actor, target, isToTop);
		//            combatScreen.OnScreenRemoved += (s, e) => this.Actor.StateManager.ChangeState(ActorStatus.Rest);
		//            this.Actor.Scene.ScreenManager.AddScreen(combatScreen);
		//        }
		//        else if(this.Actor.Moveable)
		//    }
		//}

		//private class MarchState : AIActorState
		//{
		//    public MarchState(AIActor actor)
		//        : base(actor, ActorStatus.AIAct)
		//    { }

		//    public override void Enter()
		//    {
		//        base.Enter();

		//        if (this.Actor.IsEnemyInRange())
		//        {

		//        }
		//    }
		//}
		#endregion
	}

	public enum AIStatus
	{
		Idle,
		Running,
		Finished
	}
}
