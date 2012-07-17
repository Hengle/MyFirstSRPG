using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace MyFirstSRPG.SRPGGameContentPipeline
{
	[ContentProcessor(DisplayName = "MyFirstSRPG.DigitFontTextureProcessor")]
	public class DigitFontTextureProcessor : FontTextureProcessor
	{
		private static string chars = " 0123456789/-~*ABCDEHLMepvx";

		protected override char GetCharacterForIndex(int index)
		{
			return chars[index];
		}
	}
}