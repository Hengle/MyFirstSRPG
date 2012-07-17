using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MyFirstSRPG.SRPGGame.Components;
using MyFirstSRPG.SRPGGame.GameScreens;
using MyFirstSRPG.SRPGGameLibrary;
using Microsoft.Xna.Framework.Input;

namespace MyFirstSRPG.SRPGGame
{
	public class GameMain : Game
	{
		public static readonly byte TextureScale = 2;
		public static readonly double MSPF = 1000d / 60;
		private static bool enableSound = true;
		private static Random random = new Random();

		public static Size CellSize { get; private set; }

		public static Rectangle ScreenBounds { get; private set; }

		public static Rectangle SafeArea { get; private set; }

		public static Camera Camera { get; private set; }

		public static PathFinder PathFinder { get; private set; }

		public static Dictionary<string, string> SysText { get; private set; }

		public static SoundEffectInstance[] SysSfx { get; private set; }

		public static SoundEffectInstance[] ActorSfx { get; private set; }

		public static SpriteFont NormalFont { get; private set; }

		public static SpriteFont SmallFont { get; private set; }

		public static SpriteFont DigitFont { get; private set; }

		public static int RandomNumber
		{
			get
			{
				int n = random.Next(100);
				Console.WriteLine("Rand: {0}", n);
				return n;
			}
		}

		public static Texture2D SysTexture;
		public static Texture2D FrameTexture;
		public static Texture2D[] ActorTextures;
		public static Texture2D[] ActorMoveTextures;
		public static Texture2D ActorFaceTexture;

		public SpriteBatch SpriteBatch;

		private GameScreenManager ScreenManager;
		private FpsComponent fpsComponent;
		private KeyboardState lastKeyboardState;
		public bool IsGamePaused;

		public GameMain()
		{
			GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);

#if WINDOWS_PHONE
			gdm.IsFullScreen = true;
			gdm.PreferredBackBufferWidth = 800;
			gdm.PreferredBackBufferHeight = 480;
#endif

#if WINDOWS
			gdm.PreferredBackBufferWidth = 800;
			gdm.PreferredBackBufferHeight = 480;
			this.IsMouseVisible = true;
#endif

			this.Content.RootDirectory = "Content";

			this.Components.Add(this.ScreenManager = new GameScreenManager(this));
			this.ScreenManager.AddScreen(new GameIntroScreen());
			this.Components.Add(this.fpsComponent = new FpsComponent(this));
			this.lastKeyboardState = Keyboard.GetState();
		}

		protected override void LoadContent()
		{
			CellSize = new Size(32, 32);
			ScreenBounds = new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);
			SafeArea = new Rectangle(CellSize.Width, CellSize.Height, ScreenBounds.Width - CellSize.Width * 2, ScreenBounds.Height - CellSize.Height * 2);
			Camera = new Camera(this);
			PathFinder = new PathFinder();
			SysText = this.Content.Load<Dictionary<string, string>>("Txt/SysText");

			this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
			NormalFont = this.Content.Load<SpriteFont>("Font/Font01");
			SmallFont = this.Content.Load<SpriteFont>("Font/Small");
			DigitFont = this.Content.Load<SpriteFont>("Font/Font03");
			this.Window.Title = SysText["GameTitle"];

			SysTexture = this.Content.Load<Texture2D>("Texture/Sys01");
			FrameTexture = this.Content.Load<Texture2D>("Texture/Frame01");

			SysSfx = new SoundEffectInstance[4];
			SysSfx[0] = this.Content.Load<SoundEffect>("SFX/Sys01").CreateInstance();
			SysSfx[1] = this.Content.Load<SoundEffect>("SFX/Sys02").CreateInstance();
			SysSfx[2] = this.Content.Load<SoundEffect>("SFX/Sys03").CreateInstance();
			SysSfx[3] = this.Content.Load<SoundEffect>("SFX/Sys04").CreateInstance();

			ActorSfx = new SoundEffectInstance[5];
			ActorSfx[0] = this.Content.Load<SoundEffect>("SFX/Mov01").CreateInstance();
			ActorSfx[1] = this.Content.Load<SoundEffect>("SFX/Mov02").CreateInstance();
			ActorSfx[2] = this.Content.Load<SoundEffect>("SFX/Mov03").CreateInstance();
			ActorSfx[3] = this.Content.Load<SoundEffect>("SFX/Mov04").CreateInstance();
			ActorSfx[4] = this.Content.Load<SoundEffect>("SFX/Mov05").CreateInstance();

			ActorClass.Classes = this.Content.Load<ActorClass[]>("Data/ActorClass").ToDictionary(c => c.ID);
			ActorPerk.Perks = this.Content.Load<ActorPerk[]>("Data/ActorPerk").ToDictionary(p => p.ID);
			Weapon.Weapons = this.Content.Load<Weapon[]>("Data/Weapon").ToDictionary(w => w.ID);

			base.LoadContent();
		}

		protected override void UnloadContent()
		{
			foreach (var sfx in SysSfx)
			{
				sfx.Dispose();
			}

			foreach (var sfx in ActorSfx)
			{
				sfx.Dispose();
			}
		}

		protected override void Update(GameTime gameTime)
		{
#if WINDOWS_PHONE
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				this.Exit();
			}
#else
			KeyboardState keyboardState = Keyboard.GetState();

			if (keyboardState.IsKeyDown(Keys.Space) && this.lastKeyboardState.IsKeyUp(Keys.Space))
			{
				this.IsGamePaused = !this.IsGamePaused;
			}

			this.lastKeyboardState = keyboardState;

			if (this.IsGamePaused)
			{
				return;
			}
#endif

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			this.GraphicsDevice.Clear(Color.Black);

			base.Draw(gameTime);
		}

		public static void PlaySfx(SoundEffect sfx)
		{
			if (enableSound && sfx != null)
			{
				sfx.Play();
			}
		}

		public static void PlaySfx(SoundEffectInstance sfx)
		{
			if (enableSound && sfx != null && sfx.State == SoundState.Stopped)
			{
				sfx.Play();
			}
		}

		public static void PlayBgm(Song bgm)
		{
			if (enableSound && bgm != null)
			{
				MediaPlayer.Play(bgm);
			}
		}

		public static Point GetMapPoint(MouseManager mouse)
		{
			return GetMapPoint(mouse.State.X, mouse.State.Y);
		}

		public static Point GetMapPoint(int x, int y)
		{
			x = x.Clamp(0, ScreenBounds.Width - 1) + Camera.Location.X;
			y = y.Clamp(0, ScreenBounds.Height - 1) + Camera.Location.Y;
			return new Point(x / CellSize.Width, y / CellSize.Height);
		}

		public static int GetDistance(Point startPoint, Point endPoint)
		{
			return Math.Abs(endPoint.X - startPoint.X) + Math.Abs(endPoint.Y - startPoint.Y);
		}

		#region path finding

		public static Point[] GetMoveRange(Point startPoint, int movePoints)
		{
			return PathFinder.GetMoveRange(startPoint, movePoints);
		}

		public static AttackRangePoint[] GetAttackRange(SceneActor actor)
		{
			return GetAttackRange(actor.MapPoint, actor.WeaponRanges);
		}

		public static AttackRangePoint[] GetAttackRange(Point mapCell, int movePoints, params WeaponRange[] weaponRanges)
		{
			Point[] moveRange = PathFinder.GetMoveRange(mapCell, movePoints);

			if (moveRange != null)
			{
				return GetAttackRange(moveRange, weaponRanges);
			}

			return null;
		}

		public static AttackRangePoint[] GetAttackRange(Point mapPoint, params WeaponRange[] weaponRanges)
		{
			return PathFinder.GetAttackRange(new[] { mapPoint }, weaponRanges);
		}

		public static AttackRangePoint[] GetAttackRange(Point[] moveRange, params WeaponRange[] weaponRanges)
		{
			return PathFinder.GetAttackRange(moveRange, weaponRanges);
		}

		public static Point[] GetMoveTracks(Point startPoint, Point endPoint, int mov = -1)
		{
			return PathFinder.GetMoveTracks(startPoint, endPoint, mov);
		}

		public static Point[] GetApproachTracks(Point startPoint, Point endPoint, int mov = -1)
		{
			return PathFinder.GetApproachTracks(startPoint, endPoint, mov);
		}

		#endregion
	}
}