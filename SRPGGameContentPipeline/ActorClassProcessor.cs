using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGameContentPipeline
{
	/// <summary>
	/// ID,Name,HP,STR,MAG,SKL,SPD,DEF,BLD,MOV,Sword,Lance,Axe,Bow,Staff,Fire,Thunder,Wind,Light,Dark,Perk,UpperClassID,
	///		Tile1,Region1,Offset1,Tile2,Region2,Offset2,Comment
	/// 1,领主,18,4,0,2,3,2,5,6,50,,,,,,,,,,,2,Act01,144|144|16|16,,ActM01,192|288|24|24,-4|-8,领主－男－步
	/// </summary>
	[ContentProcessor(DisplayName = "MyFirstSRPG.ActorClassProcessor")]
	public class ActorClassProcessor : ContentProcessor<string[], ActorClass[]>
	{
		public override ActorClass[] Process(string[] input, ContentProcessorContext context)
		{
			List<ActorClass> output = new List<ActorClass>();
			string[] fields;

			foreach (string line in input.Skip(1))
			{
				fields = line.Split(',');
				output.Add(new ActorClass(
					fields[0].ParseInt32(),
					fields[1],
					fields[2].ParseInt32(),
					fields[3].ParseInt32(),
					fields[4].ParseInt32(),
					fields[5].ParseInt32(),
					fields[6].ParseInt32(),
					fields[7].ParseInt32(),
					fields[8].ParseInt32(),
					fields[9].ParseInt32(),
					fields[10].ParseInt32(),
					fields[11].ParseInt32(),
					fields[12].ParseInt32(),
					fields[13].ParseInt32(),
					fields[14].ParseInt32(),
					fields[15].ParseInt32(),
					fields[16].ParseInt32(),
					fields[17].ParseInt32(),
					fields[18].ParseInt32(),
					fields[19].ParseInt32(),
					GetPerkIDs(fields[20]),
					fields[21].ParseInt32(),
					fields[22],
					GetRectangle(fields[23]),
					GetPoint(fields[24]),
					fields[25],
					GetRectangle(fields[26]),
					GetPoint(fields[27]),
					fields[28].ParseInt32()
				));
			}

			return output.ToArray();
		}

		private static int[] GetPerkIDs(string perkIDs)
		{
			return perkIDs.Split('|').Select(i => i.ParseInt32()).Where(i => i > 0).ToArray();
		}

		public static Rectangle GetRectangle(string region)
		{
			if (!string.IsNullOrEmpty(region))
			{
				int[] values = region.Split('|').Select(i => i.ParseInt32()).ToArray();
				return new Rectangle(values[0], values[1], values[2], values[3]);
			}

			return Rectangle.Empty;
		}

		public static Point GetPoint(string offset)
		{
			if (!string.IsNullOrEmpty(offset))
			{
				int[] values = offset.Split('|').Select(i => i.ParseInt32()).ToArray();
				return new Point(values[0], values[1]);
			}

			return Point.Zero;
		}
	}

	[ContentTypeWriter]
	public class ActorClassWriter : ContentTypeWriter<ActorClass>
	{
		protected override void Write(ContentWriter output, ActorClass value)
		{
			output.Write(value.ID);
			output.Write(value.Name);
			output.Write(value.HP);
			output.Write(value.STR);
			output.Write(value.MAG);
			output.Write(value.SKL);
			output.Write(value.SPD);
			output.Write(value.DEF);
			output.Write(value.BLD);
			output.Write(value.MOV);
			output.Write(value.Sword);
			output.Write(value.Lance);
			output.Write(value.Axe);
			output.Write(value.Bow);
			output.Write(value.Staff);
			output.Write(value.Fire);
			output.Write(value.Thunder);
			output.Write(value.Wind);
			output.Write(value.Light);
			output.Write(value.Dark);
			output.WriteObject(value.PerkIDs);
			output.Write(value.UpperClassID);
			output.Write(value.Tile1);
			output.WriteObject(value.Region1);
			output.WriteObject(value.Offset1);
			output.Write(value.Tile2);
			output.WriteObject(value.Region2);
			output.WriteObject(value.Offset2);
			output.Write(value.Sfx);
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return typeof(ActorClassReader).AssemblyQualifiedName;
		}
	}
}