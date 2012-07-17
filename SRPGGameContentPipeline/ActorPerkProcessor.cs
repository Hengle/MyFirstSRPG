using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGameContentPipeline
{
	/// <summary>
	/// ID,Name,Region,Comment
	/// </summary>
	[ContentProcessor(DisplayName = "MyFirstSRPG.ActorPerkProcessor")]
	public class ActorPerkProcessor : ContentProcessor<string[], ActorPerk[]>
	{
		public override ActorPerk[] Process(string[] input, ContentProcessorContext context)
		{
			List<ActorPerk> perks = new List<ActorPerk>();
			string[] fields;

			foreach (string line in input.Skip(1))
			{
				fields = line.Split(',');
				perks.Add(new ActorPerk(
					fields[0].ParseInt32(),
					fields[1],
					Helper.GetRectangle(fields[2]),
					fields[3]
				));
			}

			return perks.ToArray();
		}
	}

	[ContentTypeWriter]
	public class ActorPerkWriter : ContentTypeWriter<ActorPerk>
	{
		protected override void Write(ContentWriter output, ActorPerk value)
		{
			output.Write(value.ID);
			output.Write(value.Name);
			output.WriteObject(value.SrcRegion);
			output.Write(value.Comment);
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return typeof(ActorPerkReader).AssemblyQualifiedName;
		}
	}
}