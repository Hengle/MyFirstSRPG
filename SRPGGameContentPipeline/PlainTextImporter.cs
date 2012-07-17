using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MyFirstSRPG.SRPGGameContentPipeline
{
	[ContentImporter(".txt", ".csv", DisplayName = "MyFirstSRPG.PlainTextImporter")]
	public class PlainTextImporter : ContentImporter<string[]>
	{
		public override string[] Import(string filename, ContentImporterContext context)
		{
			List<string> lines = new List<string>();
			string line;

			using (StreamReader sr = new StreamReader(filename))
			{
				while (!sr.EndOfStream)
				{
					line = sr.ReadLine();

					if (!string.IsNullOrWhiteSpace(line))
					{
						lines.Add(line);
					}
				}
			}

			return lines.ToArray();
		}
	}
}