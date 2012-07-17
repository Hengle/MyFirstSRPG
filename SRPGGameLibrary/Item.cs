using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MyFirstSRPG.SRPGGameLibrary
{
	public class Item
	{
		public int ID;
		public ItemType Type;
		public string Name;
		public string Comment;
		public Rectangle SrcRegion;
		public bool Exchangale = true;

		public Item(int id, ItemType type, string name, Rectangle srcRegion = default(Rectangle), string comment = "")
		{
			this.ID = id;
			this.Type = type;
			this.Name = name;
			this.SrcRegion = srcRegion;
			this.Comment = comment;
		}
	}

	public class ItemReader : ContentTypeReader<Item>
	{
		protected override Item Read(ContentReader input, Item existingInstance)
		{
			Item item = existingInstance;

			if (item == null)
			{
				item = new Item(
					input.ReadInt32(),
					input.ReadObject<ItemType>(),
					input.ReadString(),
					input.ReadObject<Rectangle>(),
					input.ReadString()
				);
			}

			return item;
		}
	}
}