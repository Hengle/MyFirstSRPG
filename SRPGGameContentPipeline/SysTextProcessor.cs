using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MyFirstSRPG.SRPGGameContentPipeline
{
	[ContentProcessor(DisplayName = "MyFirstSRPG.SysTextProcessor")]
	public class SysTextProcessor : ContentProcessor<string[], Dictionary<string, string>>
	{
		public override Dictionary<string, string> Process(string[] input, ContentProcessorContext context)
		{
			Dictionary<string, string> output = new Dictionary<string, string>();
			int index;

			foreach (string line in input.Where(l => !string.IsNullOrWhiteSpace(l)))
			{
				index = line.IndexOf('=');

				if (index > 0)
				{
					output.Add(line.Substring(0, index).Trim(), line.Substring(index + 1).Trim());
				}
			}

			return output;
		}
	}
}