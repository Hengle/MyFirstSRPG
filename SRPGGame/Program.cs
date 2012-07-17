using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGame
{
#if WINDOWS || XBOX

	internal static class Program
	{
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		private static void Main(string[] args)
		{
			using (Game game = new GameMain())
			{
				game.Run();
			}
		}
	}

#endif
}