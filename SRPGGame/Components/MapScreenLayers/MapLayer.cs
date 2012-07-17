using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstSRPG.SRPGGame.GameScreens.SceneScreens;

namespace MyFirstSRPG.SRPGGame.Components.MapScreenLayers
{
	public class MapLayer : MapScreenLayer
	{
		private Rectangle[] destRects;
		private Rectangle[] srcRects;

		public MapLayer(MapScreen screen)
			: base(screen)
		{
			this.srcRects = new Rectangle[this.Map.TileSheet.GetLength(0) * this.Map.TileSheet.GetLength(1)];
			this.destRects = new Rectangle[this.srcRects.Length];
			Rectangle rect;
			Point point;
			int n = 0;

			for (int x = 0; x < this.Map.TileSheet.GetLength(0); x++)
			{
				for (int y = 0; y < this.Map.TileSheet.GetLength(1); y++)
				{
					point = this.Map.TileSheet[x, y];
					rect = new Rectangle(0, 0, this.Scene.TileSize.Width, this.Scene.TileSize.Height);
					rect.Location = new Point(rect.Width * point.X, rect.Height * point.Y);
					this.srcRects[n] = rect;

					this.destRects[n] = new Rectangle(0, 0, GameMain.CellSize.Width, GameMain.CellSize.Height);
					rect.Location = new Point(rect.Width * x - GameMain.Camera.Location.X, rect.Height * y - GameMain.Camera.Location.Y);
					n++;
				}
			}
		}

		public override void Update(GameTime gameTime)
		{
			int n = 0;

			for (int x = 0; x < this.Map.TileSheet.GetLength(0); x++)
			{
				for (int y = 0; y < this.Map.TileSheet.GetLength(1); y++)
				{
					this.destRects[n].Location = new Point(this.destRects[n].Width * x - GameMain.Camera.Location.X, this.destRects[n].Height * y - GameMain.Camera.Location.Y);
					n++;
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			this.DrawMap(this.Scene.SpriteBatch);
			//this.DrawMapCost(this.SpriteBatch);
		}

		private void DrawMap(SpriteBatch sb)
		{
			sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			for (int i = 0; i < this.destRects.Length; i++)
			{
				sb.Draw(this.Map.MapTexture, this.destRects[i], this.srcRects[i], Color.White);
			}

			sb.End();
		}

		private void DrawMapCost(SpriteBatch sb)
		{
			Color color = new Color(0xff, 0xff, 0xff, 0x60);
			Vector2 location;
			sb.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			for (int x = 0; x < this.Scene.Terrains.GetLength(0); x++)
			{
				for (int y = 0; y < this.Scene.Terrains.GetLength(1); y++)
				{
					location = new Vector2(x * GameMain.CellSize.Width - GameMain.Camera.Location.X, y * GameMain.CellSize.Height - GameMain.Camera.Location.Y);
					sb.DrawString(GameMain.NormalFont, this.Scene.Terrains[x, y].MPCost.ToString("d2"), location, color, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
				}
			}

			sb.End();
		}
	}
}
