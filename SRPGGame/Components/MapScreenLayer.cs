using Microsoft.Xna.Framework;
using MyFirstSRPG.SRPGGame.GameScreens.SceneScreens;

namespace MyFirstSRPG.SRPGGame.Components
{
	public abstract class MapScreenLayer : DrawableGameComponent
	{
		public SceneScreen Scene { get; private set; }
		protected MapScreen Map;

		public MapScreenLayer(MapScreen screen)
			: base(screen.Game)
		{
			this.Map = screen;
			this.Scene = screen.Scene;
		}

		public virtual void Update(GameTime gameTime, bool isCovered)
		{
			this.Update(gameTime);
		}

		public virtual void UpdateInput(GameTime gameTime) { }
	}
}