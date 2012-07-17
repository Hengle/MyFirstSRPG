using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGame.Components;
using MyFirstSRPG.SRPGGame.Components.MapScreenLayers;

namespace MyFirstSRPG.SRPGGame.GameScreens.SceneScreens
{
	public class MapScreen : GameScreen
	{
		public SceneScreen Scene;
		public Point[,] TileSheet;
		public Texture2D MapTexture;
		public SoundEffectInstance CusorSfx;

		private List<MapScreenLayer> components;

		public MapScreen(SceneScreen scene, int[,] tileSheet)
		{
			this.Scene = scene;
			this.components = new List<MapScreenLayer>();
			this.TileSheet = new Point[tileSheet.GetLength(0), tileSheet.GetLength(1)];
			int data;

			for (int i = 0; i < tileSheet.GetLength(0); i++)
			{
				for (int j = 0; j < tileSheet.GetLength(1); j++)
				{
					data = tileSheet[i, j];
					this.TileSheet[i, j] = new Point((byte)(data >> 8), (byte)(data >> 0));
				}
			}
		}

		public override void LoadContent()
		{
			this.MapTexture = this.Content.Load<Texture2D>("Texture/Map01");
			this.CusorSfx = this.Content.Load<SoundEffect>("SFX/Cur01").CreateInstance();

			this.components.Add(new MapLayer(this));
			this.components.Add(new ActorLayer(this));
			this.components.Add(new CursorLayer(this));
		}

		public override void UnloadContent()
		{
			this.MapTexture = null;
			this.CusorSfx.Dispose();
		}

		public override void Update(GameTime gameTime, bool isCovered, bool isFocusLost)
		{
			base.Update(gameTime, isCovered, isFocusLost);

			foreach (var component in this.components)
			{
				component.Update(gameTime, isCovered);
			}
		}

		public override void UpdateInput(GameTime gameTime)
		{
			if (this.Scene.IsInputReady)
			{
				foreach (var component in this.components)
				{
					component.UpdateInput(gameTime);
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			foreach (var component in this.components)
			{
				component.Draw(gameTime);
			}
		}

		public static Point GetScreenLocation(Point mapPoint)
		{
			Point location = Point.Zero;
			location.X = mapPoint.X * GameMain.CellSize.Width - GameMain.Camera.Location.X;
			location.Y = mapPoint.Y * GameMain.CellSize.Height - GameMain.Camera.Location.Y;
			return location;
		}
	}
}