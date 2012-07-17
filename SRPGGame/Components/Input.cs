using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGame
{
	public static class Input
	{
		public static KeyboardManager Keyboard = new KeyboardManager();
		public static MouseManager Mouse = new MouseManager();

		public static void Update(GameTime gameTime)
		{
			Keyboard.Update(gameTime);
			Mouse.Update(gameTime);
		}
	}

	public class KeyboardManager
	{
		public KeyboardState LastState { get; private set; }

		public KeyboardState State { get; private set; }

		public event EventHandler<EventArgs> OnKeyPressed;

		public KeyboardManager()
		{
			this.LastState = Keyboard.GetState();
		}

		public void Update(GameTime gameTime)
		{
			this.LastState = this.State;
			this.State = Keyboard.GetState();

			if (this.OnKeyPressed != null)
			{
				Keys[] pressedKeys = this.State.GetPressedKeys().Except(this.LastState.GetPressedKeys()).ToArray();

				if (pressedKeys.Length > 0)
				{
					this.OnKeyPressed(null, new KeyboardEventArgs(pressedKeys));
				}
			}
		}

		public bool IsKeyPressed(Keys key)
		{
			if (this.LastState.IsKeyDown(key))
			{
				if (this.State.IsKeyUp(key))
				{
					return true;
				}
			}

			return false;
		}

		public bool IsAnyKeyPressed(params Keys[] keys)
		{
			if (keys == null)
			{
				foreach (Keys key in this.LastState.GetPressedKeys())
				{
					if (this.State.IsKeyUp(key))
					{
						return true;
					}
				}
			}
			else
			{
				foreach (Keys key in keys)
				{
					if (this.IsKeyPressed(key))
					{
						return true;
					}
				}
			}

			return false;
		}

		public class KeyboardEventArgs : EventArgs
		{
			public Keys[] PressedKeys { get; private set; }

			public KeyboardEventArgs(Keys[] pressedKeys)
			{
				this.PressedKeys = pressedKeys;
			}
		}
	}

	public class MouseManager
	{
		public MouseState LastState { get; private set; }

		public MouseState State { get; private set; }

		public event EventHandler<MouseEventArgs> OnClick;

		//public event EventHandler<MouseEventArgs> OnDrag;

		//private bool isDraging = false;
		//private MouseEventArgs currentEventArgs = null;

		public MouseManager()
		{
			this.LastState = Mouse.GetState();
		}

		public void Update(GameTime gameTime)
		{
			this.LastState = this.State;
			this.State = Mouse.GetState();

			if (this.LastState.LeftButton == ButtonState.Pressed && this.State.LeftButton == ButtonState.Released)
			{
				if (this.OnClick != null)
				{
					this.OnClick(null, null);
				}
			}
			//else if (this.OnDrag != null)
			//{
			//    if (this.State.LeftButton == ButtonState.Pressed)
			//    {
			//        if (isDraging)
			//        {
			//            this.currentEventArgs.SetState(this.State);
			//            this.OnDrag(null, this.currentEventArgs);
			//        }
			//        else
			//        {
			//            this.currentEventArgs = new MouseEventArgs(this.State);
			//            this.OnDrag(null, this.currentEventArgs);
			//            this.isDraging = true;
			//        }
			//    }
			//    else if (isDraging && this.State.LeftButton == ButtonState.Released)
			//    {
			//        this.currentEventArgs.SetState(this.State);
			//        this.OnDrag(null, this.currentEventArgs);
			//        this.isDraging = false;
			//    }
			//}
		}

		public bool IsLeftButtonClicked()
		{
			return this.LastState.LeftButton == ButtonState.Pressed && this.State.LeftButton == ButtonState.Released;
		}

		public bool IsRightButtonClicked()
		{
			return this.LastState.RightButton == ButtonState.Pressed && this.State.RightButton == ButtonState.Released;
		}

		//public bool IsMouseOver()
		//{
		//    return this.IsMouseOver(Console.ScreenBounds);
		//}

		public bool IsMouseOver(Rectangle rectangle)
		{
			if (this.State.X.IsBetween(rectangle.Left, rectangle.Right))
			{
				return this.State.Y.IsBetween(rectangle.Top, rectangle.Bottom);
			}

			return false;
		}

		//public bool IsMouseOut()
		//{
		//    return this.IsMouseOut(Console.ScreenBounds);
		//}

		public bool IsMouseOut(Rectangle rectangle)
		{
			if (this.State.X.IsBetween(rectangle.Left, rectangle.Right))
			{
				return !this.State.Y.IsBetween(rectangle.Top, rectangle.Bottom);
			}

			return true;
		}

		public class MouseEventArgs : EventArgs
		{
			public MouseState InitState { get; private set; }

			public MouseState LastState { get; private set; }

			public MouseState State { get; private set; }

			public Point Offset { get { return new Point(this.State.X - this.LastState.X, this.State.Y - this.LastState.Y); } }

			public MouseEventArgs(MouseState initState)
			{
				this.State =
					this.LastState =
					this.InitState = initState;
			}

			public void SetState(MouseState state)
			{
				this.LastState = this.State;
				this.State = state;
			}
		}
	}
}