using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MyFirstSRPG.SRPGGameLibrary;

namespace MyFirstSRPG.SRPGGameContentPipeline
{
	/// <summary>
	/// ID,ItemType,Name,Rank,Hit,Power,ActualCritical,Range,Weight,SrcRegion,Comment
	/// 1,1,铁剑,50,75,6,0,1,8,
	/// </summary>
	[ContentProcessor(DisplayName = "MyFirstSRPG.WeaponProcessor")]
	public class WeaponProcessor : ContentProcessor<string[], Weapon[]>
	{
		/// <summary>
		/// ID,ItemType,Name,Rank,Hit,Power,ActualCritical,Range,Weight,Region,Comment
		/// </summary>
		/// <param name="input"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override Weapon[] Process(string[] input, ContentProcessorContext context)
		{
			List<Weapon> output = new List<Weapon>();
			output.Add(Weapon.NoneWeapon);
			string[] fields;

			foreach (string line in input.Skip(1))
			{
				fields = line.Split(',');
				output.Add(new Weapon(
					fields[0].ParseInt32(),
					(ItemType)(fields[1].ParseInt32()),
					fields[2],
					Helper.GetAttackRange(fields[7]),
					fields[3].ParseInt32(),
					fields[4].ParseInt32(),
					fields[5].ParseInt32(),
					fields[6].ParseInt32(),
					fields[8].ParseInt32(),
					Helper.GetRectangle(fields[9]),
					fields[10]
				));
			}

			return output.ToArray();
		}
	}

	[ContentTypeWriter]
	public class WeaponWriter : ContentTypeWriter<Weapon>
	{
		protected override void Write(ContentWriter output, Weapon value)
		{
			output.Write(value.ID);
			output.WriteObject(value.Type);
			output.Write(value.Name);
			output.Write(value.Range.ToString());
			output.Write(value.Rank);
			output.Write(value.Hit);
			output.Write(value.Power);
			output.Write(value.Critial);
			output.Write(value.Weight);
			output.WriteObject(value.SrcRegion);
			output.Write(value.Comment);
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return typeof(WeaponReader).AssemblyQualifiedName;
		}
	}
}