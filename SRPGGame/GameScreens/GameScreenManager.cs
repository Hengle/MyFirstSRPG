using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGameLibrary;
using System;

namespace MyFirstSRPG.SRPGGame.GameScreens
{
	public class GameScreenManager : DrawableGameComponent
	{
		public new GameMain Game;

		public ContentManager Content { get { return this.Game.Content; } }

		public SpriteBatch SpriteBatch { get { return this.Game.SpriteBatch; } }

		private List<GameScreen> screens;
		private List<GameScreen> screensToUpdate;
		private bool isContentLoaded;

		public GameScreenManager(GameMain game)
			: base(game)
		{
			this.Game = game;
			this.screens = new List<GameScreen>();
			this.screensToUpdate = new List<GameScreen>();
		}

		protected override void LoadContent()
		{
			this.isContentLoaded = true;

			foreach (GameScreen screen in this.screens)
			{
				screen.LoadContent();
			}
		}

		protected override void UnloadContent()
		{
			foreach (GameScreen screen in this.screens)
			{
				screen.UnloadContent();
			}
		}

		public override void Update(GameTime gameTime)
		{
			if (!this.Game.IsActive)
			{
				return;
			}

			Input.Update(gameTime);
			this.screensToUpdate.Clear();
			this.screensToUpdate.AddRange(this.screens);

			bool isFocusLost = !this.Game.IsActive;
			bool isCovered = false;
			GameScreen screen;

			while (this.screensToUpdate.Count > 0)
			{
				screen = this.screensToUpdate.Last();
				this.screensToUpdate.Remove(screen);
				screen.Update(gameTime, isFocusLost, isCovered);

				if (screen.Status.In(ScreenStatus.TransitionOn, ScreenStatus.Active))
				{
					if (!isFocusLost)
					{
						screen.UpdateInput(gameTime);
						isFocusLost = true;
					}

					if (!screen.IsPopup)
					{
						isCovered = true;
					}
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			if (!this.Game.IsActive)
			{
				return;
			}

			foreach (GameScreen screen in this.screens.Where(s => s.Status != ScreenStatus.Hidden))
			{
				screen.Draw(gameTime);
			}
		}

		public bool ContainsScreen(GameScreen screen)
		{
			return this.screens.Contains(screen);
		}

		public void AddScreen(GameScreen screen)
		{
			if (this.ContainsScreen(screen))
			{
				this.screens.Remove(screen);
			}
			else
			{
				screen.ScreenManager = this;
			}

			this.screens.Add(screen);
			Console.WriteLine("+ " + screen.ToString());

			if (this.isContentLoaded)
			{
				screen.LoadContent();
			}
		}

		public void RemoveScreen(GameScreen screen)
		{
			if (this.ContainsScreen(screen))
			{
				Console.WriteLine("- " + screen.ToString());
				this.screens.Remove(screen);
				this.screensToUpdate.Remove(screen);
			}

			if (this.isContentLoaded)
			{
				screen.UnloadContent();
			}
		}

		public void ClearScreens()
		{
			GameScreen screen;

			while ((screen = this.screens.LastOrDefault()) != null)
			{
				this.RemoveScreen(screen);
			}
		}
	}
}