using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGameContentPipeline
{
	[ContentProcessor(DisplayName = "MyFirstSRPG.ActorTextureProcessor")]
	public class ActorTextureProcessor : ContentProcessor<Texture2DContent, Texture2DContent[]>
	{
		public static Color[] PlayerPalette;
		public static Color[] EnemyPalette;
		public static Color[] FriendPalette;
		public static Color[] GrayPalette;

		static ActorTextureProcessor()
		{
			uint[] playPalette = new uint[] { 0xd8b890, 0x806048, 0xb0d0f8, 0x4048f8, 0x101850, 0xd07030, 0x783018, 0xf8f8f8, 0xa8a898, 0x484840, 0x202028, 0x081020 };
			uint[] enemyPalette = new uint[] { 0xd8b890, 0x886850, 0xe8b8b0, 0xa83818, 0x481018, 0x8070d8, 0x502098, 0xf8f8f8, 0x989060, 0x484830, 0x282020, 0x100000 };
			uint[] friendPalette = new uint[] { 0xd8b890, 0x886850, 0xa8e0b8, 0x389848, 0x083010, 0xd03830, 0x581000, 0xf8f8f8, 0x887038, 0x503810, 0x202820, 0x082010 };
			uint[] grayPalette = new uint[] { 0x909090, 0x585858, 0xb8b8b8, 0x606060, 0x383838, 0x909090, 0x383838, 0xd8d8d8, 0x808080, 0x383838, 0x101818, 0x080810 };

			PlayerPalette = playPalette.Select(i => Helper.ConvertToColor(i)).ToArray();
			EnemyPalette = enemyPalette.Select(i => Helper.ConvertToColor(i)).ToArray();
			FriendPalette = friendPalette.Select(i => Helper.ConvertToColor(i)).ToArray();
			GrayPalette = grayPalette.Select(i => Helper.ConvertToColor(i)).ToArray();
		}

		/// <summary>
		/// 1: Player, 2: Enemy, 3: Ally, 4: Gray
		/// </summary>
		/// <param name="input"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Texture2DContent[] Process(Texture2DContent input, ContentProcessorContext context)
		{
			Texture2DContent[] output = new Texture2DContent[4];
			PixelBitmapContent<Color> playerBitmap = input.Mipmaps[0] as PixelBitmapContent<Color>;
			output[0] = input;
			output[1] = this.GenerateTexture2DContent(playerBitmap, PlayerPalette, EnemyPalette);
			output[2] = this.GenerateTexture2DContent(playerBitmap, PlayerPalette, FriendPalette);
			output[3] = this.GenerateTexture2DContent(playerBitmap, PlayerPalette, GrayPalette);
			return output;
		}

		private Texture2DContent GenerateTexture2DContent(PixelBitmapContent<Color> inputBitmap, Color[] inputPalette, Color[] outputPalette)
		{
			Texture2DContent output = new Texture2DContent();
			PixelBitmapContent<Color> bitmap = new PixelBitmapContent<Color>(inputBitmap.Width, inputBitmap.Height);
			BitmapContent.Copy(inputBitmap, bitmap);
			ReplaceColors(bitmap, inputPalette, outputPalette);
			output.Mipmaps.Add(bitmap);
			return output;
		}

		public static void ReplaceColors(PixelBitmapContent<Color> source, Color[] srcPalette, Color[] targetPalette)
		{
			for (int j = 0; j < srcPalette.Length; j++)
			{
				source.ReplaceColor(srcPalette[j], targetPalette[j]);
			}
		}
	}
}