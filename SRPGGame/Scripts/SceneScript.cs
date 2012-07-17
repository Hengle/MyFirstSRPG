using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyFirstSRPG.SRPGGame.GameScreens.SceneScreens;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGame
{
	public abstract class SceneScript
	{
		public string Title { get; protected set; }

		public Size MapSize { get; protected set; }

		public Size TileSize { get; protected set; }

		public int[,] TileSheet { get; protected set; }

		public Terrain[,] MapData { get; protected set; }

		protected static Dictionary<int, Terrain> Terrains;
		protected SceneScreen Scene;

		static SceneScript()
		{
			Terrains = new Dictionary<int, Terrain>();
			Terrains.Add(00, new Terrain("--", 1, false, 5, 20));
			Terrains.Add(01, new Terrain("大山", 1, false, 5, 40));
			Terrains.Add(02, new Terrain("平地", 1, true));
			Terrains.Add(03, new Terrain("民家", 1, true, 1, 10));
			Terrains.Add(04, new Terrain("道路", 1, true));
			Terrains.Add(05, new Terrain("森林", 2, true, 2, 20));
			Terrains.Add(06, new Terrain("密林", 2, true, 5, 30));
			Terrains.Add(07, new Terrain("山崖", 1, false));
			Terrains.Add(08, new Terrain("城门", 1, true, 10, 30, true));
			Terrains.Add(09, new Terrain("桥梁", 1, true));
			Terrains.Add(10, new Terrain("道具店", 1, true, 1, 10));
			Terrains.Add(11, new Terrain("武器店", 1, true, 1, 10));
			Terrains.Add(12, new Terrain("斗技场", 1, true, 1, 10));
		}

		public SceneScript()
		{
		}

		public void LoadScene(SceneScreen scene)
		{
			this.Scene = scene;
			this.Scene.OnTurnStarted += this.TurnStarted;
			this.Scene.OnTurnEnded += this.TurnEnded;
			this.Scene.OnPhaseChanged += this.PhaseChanged;
		}

		protected virtual void TurnStarted(object sender, EventArgs e) { }

		protected virtual void TurnEnded(object sender, EventArgs e) { }

		protected virtual void PhaseChanged(object sender, EventArgs e) { }
	}
}