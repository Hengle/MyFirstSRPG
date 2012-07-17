using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Collections.Generic;

namespace MyFirstSRPG.SRPGGameContentPipeline
{
	[ContentProcessor(DisplayName = "MyFirstSRPG.LocalizedFontProcessor")]
	public class LocalizedFontProcessor : ContentProcessor<FontDescription, SpriteFontContent>
	{
		public override SpriteFontContent Process(FontDescription input, ContentProcessorContext context)
		{
			List<char> chars = new List<char>();

			foreach (string filename in Directory.GetFiles("Data").Where(f => Path.GetExtension(f).ToLower() == ".csv").Select(f => Path.GetFullPath(f)))
			{
				this.AddCharactersFromTxt(chars, filename);
				context.AddDependency(filename);
			}

			foreach (string filename in Directory.GetFiles("Txt").Where(f => Path.GetExtension(f).ToLower() == ".txt").Select(f => Path.GetFullPath(f)))
			{
				this.AddCharactersFromTxt(chars, filename);
				context.AddDependency(filename);
			}

			foreach (char c in chars.Distinct().OrderBy(c => c))
			{
				input.Characters.Add(c);
			}

			return context.Convert<FontDescription, SpriteFontContent>(input, "FontDescriptionProcessor");
		}

		private void AddCharactersFromTxt(List<char> chars, string filename)
		{
			using (StreamReader sr = new StreamReader(filename))
			{
				chars.AddRange(sr.ReadToEnd().Where(c => c > 126));
			}
		}
	}
}