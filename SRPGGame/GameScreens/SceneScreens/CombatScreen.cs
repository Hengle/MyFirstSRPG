using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MyFirstSRPG.SRPGGameLibrary;
using Microsoft.Xna.Framework.Audio;
using MyFirstSRPG.SRPGGame.GameScreens;
using MyFirstSRPG.SRPGGame.GameScreens.SceneScreens;
using MyFirstSRPG.SRPGGame.States;
using MyFirstSRPG.SRPGGame.Components.MapScreenLayers;

namespace MyFirstSRPG.SRPGGame
{
	public class CombatScreen : GameScreen
	{
		private static readonly double msPerFrame = GameMain.MSPF * 30;

		private SceneActor player;
		private SceneActor enemy;
		private SceneActor attacker;
		private SceneActor defender;
		private bool isToTop;

		private Rectangle hpSrcRect;
		private Rectangle attackerHPDestRect;
		private Rectangle defenderHPDestRect;
		private Vector2 attackerHPLocation;
		private Vector2 defenderHPLocation;

		private Rectangle attackerFrameSrcRect;
		private Rectangle defenderFrameSrcRect;
		private Rectangle attackerFrameDestRect;
		private Rectangle defenderFrameDestRect;

		private Rectangle expFrameSrcRect;
		private Rectangle expFrameDestRect;
		private Rectangle expSrcRect;
		private Rectangle expDestRect;
		private Vector2 expLocation;

		private CombatResult Result;
		private int CurrentExp;
		private SoundEffect HitSfx;
		private SoundEffect AvoidSfx;
		private SceneScreen Scene;
		private GameStateManager<CombatScreen, CombatState> StateManager;

		public CombatScreen(SceneScreen scene, SceneActor attacker, SceneActor defender, bool isToTop)
		{
			this.IsPopup = true;
			this.TransitionOnTime =
				this.TransitionOffTime = TimeSpan.FromSeconds(2d);
			this.Scene = scene;
			this.attacker = attacker;
			this.defender = defender;

			if (this.attacker.Faction == Faction.Player)
			{
				this.player = this.attacker;
				this.enemy = this.defender;
			}
			else if (this.defender.Faction == Faction.Player)
			{
				this.player = this.defender;
				this.enemy = this.attacker;
			}

			this.isToTop = isToTop;
		}

		public override void LoadContent()
		{
			this.HitSfx = this.Content.Load<SoundEffect>("SFX/Cbt01");
			this.AvoidSfx = this.Content.Load<SoundEffect>("SFX/Cbt02");

			this.attackerFrameSrcRect = this.GetFrameSrcRect(this.attacker.Faction);
			this.defenderFrameSrcRect = this.GetFrameSrcRect(this.defender.Faction);
			this.attackerFrameDestRect = new Rectangle
			(
				0,
				0,
				this.attackerFrameSrcRect.Width * GameMain.TextureScale,
				this.attackerFrameSrcRect.Height * GameMain.TextureScale
			);
			this.defenderFrameDestRect = new Rectangle
			(
				0,
				0,
				this.defenderFrameSrcRect.Width * GameMain.TextureScale,
				this.defenderFrameSrcRect.Height * GameMain.TextureScale
			);

			this.attackerFrameDestRect.X = GameMain.ScreenBounds.Width / 2 - this.attackerFrameDestRect.Width;
			this.defenderFrameDestRect.X = GameMain.ScreenBounds.Width / 2;

			if (this.isToTop)
			{
				this.attackerFrameDestRect.Y =
					this.defenderFrameDestRect.Y = (GameMain.ScreenBounds.Height - this.attackerFrameDestRect.Height) / 3;
			}
			else
			{
				this.attackerFrameDestRect.Y =
					this.defenderFrameDestRect.Y = (GameMain.ScreenBounds.Height - this.attackerFrameDestRect.Height) / 3 * 2 + this.attackerFrameDestRect.Height;
			}

			this.hpSrcRect = new Rectangle(196, 47, 1, 4);
			this.attackerHPDestRect = new Rectangle
			(
				this.attackerFrameDestRect.X + 32 * GameMain.TextureScale,
				this.attackerFrameDestRect.Y + 19 * GameMain.TextureScale,
				52 * GameMain.TextureScale,
				this.hpSrcRect.Height * GameMain.TextureScale
			);
			this.defenderHPDestRect = new Rectangle
			(
				this.defenderFrameDestRect.X + 32 * GameMain.TextureScale,
				this.defenderFrameDestRect.Y + 19 * GameMain.TextureScale,
				52 * GameMain.TextureScale,
				this.hpSrcRect.Height * GameMain.TextureScale
			);
			this.attackerHPLocation = new Vector2(this.attackerFrameDestRect.X + 13 * GameMain.TextureScale, this.attackerFrameDestRect.Y + 15 * GameMain.TextureScale);
			this.defenderHPLocation = new Vector2(this.defenderFrameDestRect.X + 13 * GameMain.TextureScale, this.defenderFrameDestRect.Y + 15 * GameMain.TextureScale);

			this.expFrameSrcRect = new Rectangle(0, 21, 160, 26);
			this.expFrameDestRect.Width = this.expFrameSrcRect.Width * GameMain.TextureScale;
			this.expFrameDestRect.Height = this.expFrameSrcRect.Height * GameMain.TextureScale;
			this.expFrameDestRect.X = (GameMain.ScreenBounds.Width - this.expFrameDestRect.Width) / 2;
			this.expFrameDestRect.Y = (GameMain.ScreenBounds.Height - this.expFrameDestRect.Height - 8 * GameMain.TextureScale) / 2;

			this.expSrcRect = new Rectangle(197, 47, 1, 4);
			this.expDestRect.Width = this.attacker.EXP * GameMain.TextureScale;
			this.expDestRect.Height = this.expSrcRect.Height * GameMain.TextureScale;
			this.expDestRect.X = this.expFrameDestRect.X + 32 * GameMain.TextureScale;
			this.expDestRect.Y = this.expFrameDestRect.Y + 11 * GameMain.TextureScale;

			this.expLocation = new Vector2(this.expFrameDestRect.Right - 24 * GameMain.TextureScale, this.expFrameDestRect.Y + 6 * GameMain.TextureScale);

			this.StateManager = new GameStateManager<CombatScreen, CombatState>();
			this.StateManager.AddState(new AttackState(this));
			this.StateManager.AddState(new CounterAttackState(this));
			this.StateManager.AddState(new ChaseAttackState(this));
			this.StateManager.AddState(new ExpState(this));
			this.StateManager.AddState(new EndingState(this));
			this.StateManager.AddState(new OpeningState(this), true);
		}

		public override void Update(GameTime gameTime, bool isCovered, bool isFocusLost)
		{
			base.Update(gameTime, isCovered, isFocusLost);

			switch (this.Status)
			{
				case ScreenStatus.TransitionOn:
					this.Brush = Color.White * this.TransitionOffset;
					break;
				case ScreenStatus.TransitionOff:
					this.Brush = Color.White * this.TransitionOffset;
					break;
			}

			this.StateManager.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			this.StateManager.Draw(gameTime);

			this.SpriteBatch.End();
		}

		private void DrawHP()
		{
			this.SpriteBatch.Draw(GameMain.SysTexture, this.attackerFrameDestRect, this.attackerFrameSrcRect, this.Brush);
			this.SpriteBatch.Draw(GameMain.SysTexture, this.attackerHPDestRect, this.hpSrcRect, this.Brush);
			this.SpriteBatch.DrawString(GameMain.SmallFont, this.attacker.CurrentHP.ToString(), this.attackerHPLocation, this.Brush);

			this.SpriteBatch.Draw(GameMain.SysTexture, this.defenderFrameDestRect, this.defenderFrameSrcRect, this.Brush);
			this.SpriteBatch.Draw(GameMain.SysTexture, this.defenderHPDestRect, this.hpSrcRect, this.Brush);
			this.SpriteBatch.DrawString(GameMain.SmallFont, this.defender.CurrentHP.ToString(), this.defenderHPLocation, this.Brush);
		}

		private void DrawExp()
		{
			this.SpriteBatch.Draw(GameMain.SysTexture, this.expFrameDestRect, this.expFrameSrcRect, this.Brush);
			this.SpriteBatch.Draw(GameMain.SysTexture, this.expDestRect, this.expSrcRect, this.Brush);
			this.SpriteBatch.DrawString(GameMain.SmallFont, this.CurrentExp.ToString("d2"), this.expLocation, Color.Black);
		}

		private Rectangle GetFrameSrcRect(Faction faction)
		{
			Rectangle rect = Rectangle.Empty;

			switch (faction)
			{
				case Faction.Player:
					rect = new Rectangle(98, 47, 98, 34);
					break;
				case Faction.Enemy:
					rect = new Rectangle(0, 47, 98, 34);
					break;
			}

			return rect;
		}

		private AttackResult CalculateCombat(SceneActor attacker, SceneActor defender)
		{
			if ((attacker.ActualAccuracy - defender.ActualAvoid).Clamp(1, 99) > GameMain.RandomNumber)
			{
				int atk = attacker.ActualAttackPower;

				if (attacker.ActualCritical - defender.ActualCriticalEvade > GameMain.RandomNumber)
				{
					atk *= 2;
				}

				int damage = (atk - defender.ActualDEF).Clamp(0, defender.CurrentHP);

				if (this.player == attacker)
				{
					this.Result = CombatResult.Hit;
				}

				return new AttackResult(true, damage);
			}
			else
			{
				return new AttackResult(false);
			}
		}

		private double GetHPWidth(SceneActor actor, double hpLoss = 0)
		{
			return (actor.CurrentHP - hpLoss) / actor.ActualHP * 52 * GameMain.TextureScale;
		}

		private void UpdateCombat(SceneActor attacker, SceneActor defender, bool isOriginalAttacker, AttackResult attackResult, CombatStatus combatStatus, ref TimeSpan combatTime, ref bool isSfxPlayed)
		{
			if (combatTime.TotalMilliseconds < msPerFrame)
			{
				attacker.StateManager.ChangeState(ActorStatus.Attack);

				if (attackResult.Hit)
				{
					defender.StateManager.ChangeState(ActorStatus.Defend);
				}
				else
				{
					defender.StateManager.ChangeState(ActorStatus.Avoid);
				}
			}
			else if (combatTime.TotalMilliseconds < msPerFrame * 2)
			{
				if (attackResult.Damage > 0)
				{
					double hpLoss = (combatTime.TotalMilliseconds / msPerFrame - 1d) * attackResult.Damage;

					if (isOriginalAttacker)
					{
						this.defenderHPDestRect.Width = (int)this.GetHPWidth(defender, hpLoss);
					}
					else
					{
						this.attackerHPDestRect.Width = (int)this.GetHPWidth(attacker, hpLoss);
					}
				}

				if (!isSfxPlayed)
				{
					if (attackResult.Hit)
					{
						GameMain.PlaySfx(this.HitSfx);
					}
					else
					{
						GameMain.PlaySfx(this.AvoidSfx);
					}

					isSfxPlayed = true;
				}
			}
			else
			{
				combatTime = TimeSpan.Zero;
				isSfxPlayed = false;
				defender.CurrentHP -= attackResult.Damage;

				if (defender.CurrentHP == 0)
				{
					defender.StateManager.ChangeState(ActorStatus.Die);
					this.StateManager.ChangeState(CombatStatus.Exp);
				}
				else
				{
					switch (combatStatus)
					{
						case CombatStatus.Attack:
							this.StateManager.ChangeState(CombatStatus.CounterAttack);
							break;
						case CombatStatus.CounterAttack:
							int spd = this.attacker.ActualAttackSpeed - this.defender.ActualAttackSpeed;

							if (Math.Abs(spd) >= 4)
							{
								this.StateManager.ChangeState(CombatStatus.ChaseAttack);
							}
							else
							{
								this.StateManager.ChangeState(CombatStatus.Exp);
							}
							break;
						case CombatStatus.ChaseAttack:
							this.StateManager.ChangeState(CombatStatus.Exp);
							break;
					}
				}
			}
		}

		private void ShowLevelUpScreen(object sender, EventArgs e)
		{
			this.ScreenManager.AddScreen(new LevelUpScreen(this.player));
		}

		private enum CombatResult
		{
			Miss,
			Hit,
			KO
		}

		public enum CombatStatus
		{
			Opening,
			Attack,
			CounterAttack,
			ChaseAttack,
			Exp,
			Ending
		}

		private struct AttackResult
		{
			public bool Hit;
			public int Damage;

			public AttackResult(bool hit = false, int damage = 0)
			{
				this.Hit = hit;
				this.Damage = damage;
			}
		}

		private abstract class CombatState : GameState<CombatScreen>
		{
			protected TimeSpan CombatTime;
			protected AttackResult AttackResult;
			protected SceneActor Attacker;
			protected SceneActor Defender;
			protected bool IsOriginalAttacker;
			protected bool IsSfxPlayed;

			public CombatState(CombatScreen gameObject, CombatStatus status) : base(gameObject, status) { }

			public override void Update(GameTime gameTime)
			{
				this.CombatTime += gameTime.ElapsedGameTime;
			}
		}

		private class OpeningState : CombatState
		{
			public OpeningState(CombatScreen gameObject) : base(gameObject, CombatStatus.Opening) { }

			public override void Enter()
			{
				base.Enter();
				this.Attacker = this.GameObject.attacker;
				this.Defender = this.GameObject.defender;
				this.Attacker.SetFaceDirection(this.Defender.MapPoint);
				this.GameObject.attackerHPDestRect.Width = (int)this.GameObject.GetHPWidth(this.GameObject.attacker);
				this.GameObject.defenderHPDestRect.Width = (int)this.GameObject.GetHPWidth(this.GameObject.defender);
			}

			public override void Update(GameTime gameTime)
			{
				base.Update(gameTime);

				if (this.GameObject.Status == ScreenStatus.Active)
				{
					this.GameObject.StateManager.ChangeState(CombatStatus.Attack);
				}
			}

			public override void Draw(GameTime gameTime)
			{
				this.GameObject.DrawHP();
			}
		}

		private class AttackState : CombatState
		{
			public AttackState(CombatScreen gameObject) : base(gameObject, CombatStatus.Attack) { }

			public override void Enter()
			{
				base.Enter();
				this.Attacker = this.GameObject.attacker;
				this.Defender = this.GameObject.defender;
				this.IsOriginalAttacker = true;
				this.AttackResult = this.GameObject.CalculateCombat(this.Attacker, this.Defender);

				if (this.Attacker == this.GameObject.enemy)
				{
					this.GameObject.Result = CombatResult.Hit;
				}
			}

			public override void Update(GameTime gameTime)
			{
				base.Update(gameTime);

				this.GameObject.UpdateCombat(this.Attacker, this.Defender, this.IsOriginalAttacker, this.AttackResult, CombatStatus.Attack, ref this.CombatTime, ref this.IsSfxPlayed);
			}

			public override void Draw(GameTime gameTime)
			{
				this.GameObject.DrawHP();
			}
		}

		private class CounterAttackState : CombatState
		{
			public CounterAttackState(CombatScreen gameObject) : base(gameObject, CombatStatus.CounterAttack) { }

			public override void Enter()
			{
				base.Enter();
				this.Attacker = this.GameObject.defender;
				this.Defender = this.GameObject.attacker;
				this.IsOriginalAttacker = false;
				this.AttackResult = this.GameObject.CalculateCombat(this.Attacker, this.Defender);
			}

			public override void Update(GameTime gameTime)
			{
				base.Update(gameTime);

				this.GameObject.UpdateCombat(this.Attacker, this.Defender, this.IsOriginalAttacker, this.AttackResult, CombatStatus.CounterAttack, ref this.CombatTime, ref this.IsSfxPlayed);
			}

			public override void Draw(GameTime gameTime)
			{
				this.GameObject.DrawHP();
			}
		}

		private class ChaseAttackState : CombatState
		{
			public ChaseAttackState(CombatScreen gameObject) : base(gameObject, CombatStatus.ChaseAttack) { }

			public override void Enter()
			{
				base.Enter();
				int spd = this.GameObject.attacker.ActualAttackSpeed - this.GameObject.defender.ActualAttackSpeed;

				if (Math.Abs(spd) >= 4)
				{
					if (spd > 0)
					{
						this.Attacker = this.GameObject.attacker;
						this.Defender = this.GameObject.defender;
						this.IsOriginalAttacker = true;
					}
					else
					{
						this.Attacker = this.GameObject.defender;
						this.Defender = this.GameObject.attacker;
						this.IsOriginalAttacker = false;
					}

					this.AttackResult = this.GameObject.CalculateCombat(this.Attacker, this.Defender);
				}
				else
				{
					this.GameObject.StateManager.ChangeState(CombatStatus.Ending);
				}
			}

			public override void Update(GameTime gameTime)
			{
				base.Update(gameTime);

				this.GameObject.UpdateCombat(this.Attacker, this.Defender, this.IsOriginalAttacker, this.AttackResult, CombatStatus.ChaseAttack, ref this.CombatTime, ref this.IsSfxPlayed);
			}

			public override void Draw(GameTime gameTime)
			{
				this.GameObject.DrawHP();
			}
		}

		private class ExpState : CombatState
		{
			private int gainedExp;
			private bool isPlayerLvup;
			private bool isExpUpdated;

			public ExpState(CombatScreen gameObject) : base(gameObject, CombatStatus.Exp) { }

			public override void Enter()
			{
				base.Enter();

				if (this.GameObject.Result == CombatResult.Hit)
				{
					this.gainedExp = this.GainExp();
				}
			}

			public override void Update(GameTime gameTime)
			{
				base.Update(gameTime);

				if (this.CombatTime.TotalSeconds < 1d)
				{
					return;
				}

				if (isExpUpdated)
				{
					if (this.isPlayerLvup)
					{
						this.GameObject.OnScreenRemoved += this.GameObject.ShowLevelUpScreen;
						this.GameObject.TransitionOffTime = TimeSpan.Zero;
					}

					this.GameObject.StateManager.ChangeState(CombatStatus.Ending);
				}
				else if (this.GameObject.Result == CombatResult.Miss)
				{
					this.GameObject.StateManager.ChangeState(CombatStatus.Ending);
				}
				else
				{
					this.UpdateExp();
				}
			}

			public override void Draw(GameTime gameTime)
			{
				if (this.CombatTime.TotalSeconds < 0.5d)
				{
					if (isExpUpdated || this.GameObject.Result == CombatResult.Miss)
					{
					}
					else
					{
						this.GameObject.DrawHP();
						return;
					}
				}

				this.GameObject.DrawExp();
			}

			private int GainExp()
			{
				int exp = (31 - this.GameObject.player.LV) / 3;

				if (this.GameObject.enemy.CurrentHP == 0)
				{
					exp = exp + ((this.GameObject.enemy.LV - this.GameObject.player.LV) * 3 + 20).Clamp(0, 100);
				}

				return (exp * 5).Clamp(0, 100);
			}

			private void UpdateExp()
			{
				int n = (int)((this.CombatTime.TotalMilliseconds - 1000d) / GameMain.MSPF);

				if (n >= this.gainedExp)
				{
					this.GameObject.CurrentExp =
						this.GameObject.player.EXP = this.gainedExp;
					this.isExpUpdated = true;
					this.CombatTime = TimeSpan.Zero;
				}
				else
				{
					this.GameObject.CurrentExp = this.GameObject.player.EXP + n;
				}

				if (this.GameObject.CurrentExp >= 100)
				{
					this.isPlayerLvup = true;
					this.GameObject.CurrentExp = this.GameObject.CurrentExp - 100;
				}

				this.GameObject.expDestRect.Width = this.GameObject.CurrentExp * GameMain.TextureScale;
			}
		}

		private class EndingState : CombatState
		{
			public EndingState(CombatScreen gameObject) : base(gameObject, CombatStatus.Ending) { }

			public override void Enter()
			{
				base.Enter();
				this.Attacker = this.GameObject.attacker;
				this.Defender = this.GameObject.defender;

				if (this.Attacker.Status == ActorStatus.Die)
				{
					this.GameObject.Scene.RemoveActor(this.Attacker);
				}
				else if ((this.Attacker.Faction == Faction.Player && this.GameObject.Scene.Phase == TurnPhase.PlayerPhase)
					|| (this.Attacker.Faction == Faction.Enemy && this.GameObject.Scene.Phase == TurnPhase.EnemyPhase)
					|| (this.Attacker.Faction == Faction.Ally && this.GameObject.Scene.Phase == TurnPhase.AllyPhase))
				{
					this.Attacker.StateManager.ChangeState(ActorStatus.Rest);
				}
				else
				{
					this.Attacker.StateManager.ChangeState(ActorStatus.Wait);
				}

				if (this.Defender.Status == ActorStatus.Die)
				{
					this.GameObject.Scene.RemoveActor(this.Defender);
				}
				else
				{
					this.Defender.StateManager.ChangeState(ActorStatus.Wait);
				}
			}

			public override void Update(GameTime gameTime)
			{
				base.Update(gameTime);

				if (this.CombatTime > this.GameObject.TransitionOffTime)
				{
					this.GameObject.Exit();
				}
			}

			public override void Exit()
			{
				base.Exit();
			}
		}
	}

}
