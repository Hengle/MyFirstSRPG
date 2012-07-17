using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace MyFirstSRPG.SRPGGameContentPipeline
{
	[ContentProcessor(DisplayName = "MyFirstSRPG.LocalizedFontTextureProcessor")]
	public class LocalizedFontTextureProcessor : FontTextureProcessor
	{
		private string charCodesFileName;
		private int[] charCodes;

		protected override char GetCharacterForIndex(int index)
		{
			if (this.charCodes == null)
			{
				using (StreamReader sr = new StreamReader(this.charCodesFileName))
				{
					this.charCodes = sr.ReadToEnd().Trim().TrimEnd(',').Split(',').Select(s => int.Parse(s)).ToArray();
				}
			}

			return (char)(this.charCodes[index]);
		}

		public override SpriteFontContent Process(Texture2DContent input, ContentProcessorContext context)
		{
			this.charCodesFileName = string.Format("Font\\{0}.txt", Path.GetFileNameWithoutExtension(context.OutputFilename));
			return base.Process(input, context);
		}
	}
}