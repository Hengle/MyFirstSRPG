using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public class ActorPerk
	{
		public static Dictionary<int, ActorPerk> Perks;

		public int ID { get; private set; }

		public string Name { get; private set; }

		public Rectangle SrcRegion { get; private set; }

		public string Comment { get; private set; }

		public ActorPerk(int id, string name, Rectangle srcRegion = default(Rectangle), string comment = "")
		{
			this.ID = id;
			this.Name = name;
			this.SrcRegion = srcRegion;
			this.Comment = comment;
		}
	}

	public class ActorPerkReader : ContentTypeReader<ActorPerk>
	{
		protected override ActorPerk Read(ContentReader input, ActorPerk existingInstance)
		{
			ActorPerk perk = existingInstance;

			if (perk == null)
			{
				perk = new ActorPerk(
					input.ReadInt32(),
					input.ReadString(),
					input.ReadObject<Rectangle>(),
					input.ReadString()
				);
			}

			return perk;
		}
	}
}