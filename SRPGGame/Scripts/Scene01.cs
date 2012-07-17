using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyFirstSRPG.SRPGGameLibrary;
using MyFirstSRPG.SRPGGame.GameScreens.SceneScreens;
using Microsoft.Xna.Framework;

namespace MyFirstSRPG.SRPGGame
{
	public class Scene01 : SceneScript
	{
		public Scene01()
		{
			this.Title = "菲亚那的战士";
			this.MapSize = new Size(25, 21);

			this.TileSize = new Size(16, 16);

			this.TileSheet = new int[21, 25]
			{
				#region TileSheet

				{0x0007,0x0604,0x0103,0x0704,0x0804,0x0102,0x0a00,0x0203,0x0202,0x0400,0x0107,0x0904,0x0a04,0x0a04,0x0001,0x0600,0x0a00,0x0203,0x0c00,0x0102,0x0a00,0x0301,0x0207,0x0604,0x0103},
				{0x0302,0x0303,0x0c00,0x0704,0x0307,0x0b00,0x0b00,0x0700,0x0402,0x0100,0x0700,0x0400,0x0100,0x0500,0x0400,0x0b00,0x0b00,0x0a00,0x0202,0x0b00,0x0100,0x0a00,0x0203,0x0303,0x0c00},
				{0x0407,0x0302,0x0c00,0x0804,0x0102,0x0b04,0x0403,0x0300,0x0300,0x0300,0x0503,0x0603,0x0700,0x0600,0x0403,0x0300,0x0300,0x0300,0x0300,0x0300,0x0503,0x0603,0x0301,0x0302,0x0c00},
				{0x0301,0x0302,0x0c00,0x0102,0x0000,0x0900,0x0200,0x0500,0x0502,0x0100,0x0703,0x0c04,0x0500,0x0507,0x0200,0x0500,0x0607,0x0707,0x0807,0x0100,0x0200,0x0800,0x0a00,0x0301,0x0c00},
				{0x0a00,0x0301,0x0202,0x0907,0x0100,0x0900,0x0200,0x0401,0x0501,0x0601,0x0100,0x0803,0x0a07,0x0903,0x0005,0x0400,0x0b07,0x0c07,0x0008,0x0400,0x0200,0x0800,0x0100,0x0a00,0x0202},
				{0x0701,0x0108,0x0700,0x0402,0x0000,0x0900,0x0200,0x0801,0x0901,0x0a01,0x0000,0x0208,0x0308,0x0408,0x0200,0x0100,0x0508,0x0608,0x0708,0x0600,0x0200,0x0800,0x0000,0x0000,0x0000},
				{0x0105,0x0701,0x0400,0x0000,0x0500,0x0900,0x0200,0x0101,0x0808,0x0908,0x0500,0x0a08,0x0b08,0x0c08,0x0200,0x0100,0x0400,0x0a03,0x0803,0x0100,0x0200,0x0800,0x0600,0x0000,0x0000},
				{0x0602,0x0001,0x0700,0x0600,0x0402,0x0900,0x0200,0x0400,0x0009,0x0109,0x0209,0x0101,0x0a03,0x0b01,0x0205,0x0309,0x0500,0x0a03,0x0400,0x0903,0x0005,0x0800,0x0305,0x0701,0x0000},
				{0x0405,0x0b03,0x0505,0x0505,0x0000,0x0900,0x0200,0x0700,0x0500,0x0502,0x0409,0x0c01,0x0509,0x0609,0x0c01,0x0605,0x0705,0x0709,0x0502,0x0700,0x0200,0x0800,0x0809,0x0001,0x0000},
				{0x0701,0x0909,0x0305,0x0702,0x0000,0x0900,0x0200,0x0600,0x0401,0x0501,0x0601,0x0100,0x0401,0x0501,0x0601,0x0a09,0x0100,0x0401,0x0501,0x0601,0x0200,0x0800,0x0400,0x0000,0x0000},
				{0x0105,0x0701,0x0b09,0x0001,0x0000,0x0900,0x0200,0x0400,0x0801,0x0901,0x0a01,0x0400,0x0801,0x0901,0x0a01,0x0c09,0x0100,0x0801,0x0901,0x0a01,0x0200,0x0800,0x0700,0x0000,0x0000},
				{0x0602,0x0001,0x000a,0x0700,0x0600,0x0900,0x0200,0x0b00,0x0101,0x0802,0x0b01,0x0b00,0x0101,0x0802,0x0b01,0x010a,0x0b00,0x0101,0x0802,0x0b01,0x0703,0x0c04,0x0805,0x0905,0x0902},
				{0x020a,0x0600,0x0b03,0x0000,0x0000,0x030a,0x0703,0x0500,0x040a,0x0c03,0x0a05,0x0c01,0x0705,0x0c03,0x0a05,0x0c03,0x0c01,0x0605,0x0a02,0x0902,0x0803,0x0000,0x0b05,0x0002,0x0002},
				{0x0405,0x0500,0x050a,0x0100,0x0500,0x0500,0x0500,0x060a,0x0c05,0x0403,0x0300,0x0300,0x0300,0x0300,0x0300,0x0300,0x0503,0x070a,0x0a02,0x0002,0x080a,0x0603,0x090a,0x0201,0x0006},
				{0x0702,0x0a0a,0x0b02,0x0600,0x0402,0x0b0a,0x0c01,0x0c05,0x0500,0x0200,0x0100,0x0700,0x0600,0x0502,0x0100,0x0100,0x0200,0x0c0a,0x0106,0x0201,0x0200,0x0800,0x0000,0x0000,0x0000},
				{0x0001,0x000b,0x0805,0x0905,0x0902,0x0b04,0x0903,0x0300,0x0300,0x0206,0x0b02,0x0100,0x0401,0x0501,0x0601,0x0100,0x0205,0x0300,0x0300,0x0300,0x0206,0x0800,0x0100,0x0000,0x0000},
				{0x0306,0x0100,0x0b05,0x0406,0x0006,0x010b,0x0004,0x0506,0x0506,0x0306,0x0606,0x0500,0x0801,0x0901,0x0a01,0x0600,0x0b02,0x0706,0x0806,0x0706,0x0806,0x0606,0x0400,0x0000,0x0000},
				{0x0500,0x0500,0x020b,0x030b,0x0400,0x0004,0x040b,0x0100,0x0700,0x0600,0x050b,0x0000,0x0101,0x0802,0x0b01,0x0100,0x060b,0x070b,0x0702,0x0100,0x0400,0x0906,0x0104,0x0906,0x0104},
				{0x0c02,0x0902,0x080b,0x090b,0x0700,0x0a0b,0x0004,0x0b0b,0x0c0b,0x0b02,0x0000,0x0100,0x000c,0x0406,0x0000,0x0702,0x010c,0x0602,0x0204,0x0104,0x0304,0x020c,0x0404,0x0a06,0x0504},
				{0x030c,0x0a02,0x0201,0x0400,0x0003,0x0003,0x0000,0x0000,0x0c02,0x040c,0x050c,0x0c02,0x0a02,0x060c,0x0904,0x070c,0x0602,0x0204,0x0304,0x0504,0x0404,0x080c,0x090c,0x0404,0x0504},
				{0x0106,0x0201,0x0003,0x0700,0x0600,0x0400,0x0003,0x0c02,0x0002,0x0201,0x0a0c,0x0002,0x0b0c,0x0201,0x0b03,0x0c0c,0x0204,0x0304,0x0303,0x0b06,0x0c06,0x0a06,0x0103,0x0c06,0x0b06}

				#endregion TileSheet
			}.ToPivot();

			var data = new int[21, 25]
			{
				#region Terrain data
				{01,01,01,01,01,01,02,01,01,02,05,05,06,06,05,02,02,01,01,01,02,01,01,01,01},
				{01,01,01,01,01,02,02,02,02,05,02,02,05,02,02,02,02,02,01,02,05,02,01,01,01},
				{01,01,01,01,01,02,00,00,00,00,00,02,02,02,00,00,00,00,00,00,00,02,01,01,01},
				{01,01,01,01,02,02,00,02,02,05,00,02,02,07,00,02,00,00,00,05,00,02,02,01,01},
				{02,01,01,02,05,02,00,00,00,00,05,02,02,00,00,02,00,00,00,02,00,02,05,02,01},
				{05,02,02,02,02,02,00,00,03,00,02,00,00,00,00,05,02,08,02,02,00,02,02,02,02},
				{06,05,02,02,02,02,00,02,02,02,02,00,10,00,00,05,02,04,02,05,00,02,02,02,02},
				{06,05,02,02,02,02,00,02,02,02,02,02,04,02,00,00,02,04,02,00,00,02,05,02,02},
				{02,02,02,02,02,02,00,02,02,02,04,04,04,04,04,04,04,04,02,02,00,02,02,05,02},
				{05,02,05,05,02,02,00,02,00,00,00,05,00,00,00,04,05,00,00,00,00,02,02,05,02},
				{06,05,05,05,02,02,00,02,00,03,00,02,00,03,00,04,05,00,03,00,00,02,02,02,02},
				{06,05,05,02,02,02,00,02,02,02,02,02,02,02,02,04,02,02,02,02,00,02,02,02,02},
				{06,02,02,02,02,02,00,02,04,04,04,04,04,04,04,04,04,04,04,02,02,02,02,02,02},
				{02,02,07,05,02,02,02,04,04,00,00,00,00,00,00,00,00,02,04,02,00,02,02,02,02},
				{05,07,07,02,02,02,04,04,02,00,05,02,02,02,05,05,00,02,02,02,00,02,02,02,02},
				{05,07,02,02,02,02,00,00,00,00,07,05,00,00,00,05,00,00,00,00,00,02,05,02,02},
				{07,05,02,02,02,07,07,07,07,07,02,02,00,03,00,02,07,02,02,02,02,02,02,02,02},
				{02,02,02,02,02,07,02,05,02,02,02,02,02,02,02,05,02,05,05,05,02,01,01,01,02},
				{02,02,02,02,02,02,07,07,07,07,02,05,02,02,02,05,06,06,06,01,01,01,01,01,01},
				{02,04,02,02,02,02,02,02,02,02,02,02,04,02,05,06,06,06,01,01,01,01,01,01,01},
				{02,02,02,02,02,02,02,02,02,02,02,02,02,02,02,06,06,01,01,01,01,01,01,01,01}
				#endregion Terrain data
			}.ToPivot();

			this.MapData = new Terrain[data.GetLength(0), data.GetLength(1)];

			for (int i = 0; i < data.GetLength(0); i++)
			{
				for (int j = 0; j < data.GetLength(1); j++)
				{
					this.MapData[i, j] = Terrains[data[i, j]];
				}
			}
		}

		protected override void TurnStarted(object sender, EventArgs e)
		{
			if (this.Scene.Turns == 1)
			{
				this.Scene.ChangePhase(TurnPhase.EventPhase);

				#region Ememy appears

				List<SceneActor> enemies = new List<SceneActor>();
				var boss1 = new SceneActor(new Actor(9, 16, 5, "雷多利克"), new Point(96, 384)) { Faction = Faction.Enemy, MapPoint = new Point(17, 6) };
				var boss2 = new SceneActor(new Actor(10, 11, 5, 0, "瓦兹曼", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(288, 384)) { Faction = Faction.Enemy, MapPoint = new Point(17, 5) };
				enemies.Add(boss1);
				enemies.Add(boss2);
				enemies.Add(new SceneActor(new Actor(11, 54, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(0, 576)) { Faction = Faction.Enemy, MapPoint = new Point(8, 5) });
				enemies.Add(new SceneActor(new Actor(12, 54, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(0, 576)) { Faction = Faction.Enemy, MapPoint = new Point(12, 6) });
				enemies.Add(new SceneActor(new Actor(13, 54, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(0, 576)) { Faction = Faction.Enemy, MapPoint = new Point(10, 8) });
				enemies.Add(new SceneActor(new Actor(14, 54, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(0, 576)) { Faction = Faction.Enemy, MapPoint = new Point(13, 10) });
				enemies.Add(new SceneActor(new Actor(15, 54, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(0, 576)) { Faction = Faction.Enemy, MapPoint = new Point(18, 10) });
				enemies.Add(new SceneActor(new Actor(16, 54, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(0, 576)) { Faction = Faction.Enemy, MapPoint = new Point(17, 8) });
				enemies.Add(new SceneActor(new Actor(17, 54, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(0, 576)) { Faction = Faction.Enemy, MapPoint = new Point(10, 12) });
				enemies.Add(new SceneActor(new Actor(18, 54, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(0, 576)) { Faction = Faction.Enemy, MapPoint = new Point(15, 12) });
				enemies.Add(new SceneActor(new Actor(19, 54, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(0, 576)) { Faction = Faction.Enemy, MapPoint = new Point(8, 14) });
				enemies.Add(new SceneActor(new Actor(20, 54, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(0, 576)) { Faction = Faction.Enemy, MapPoint = new Point(18, 13) });
				enemies.Add(new SceneActor(new Actor(21, 55, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(48, 576)) { Faction = Faction.Enemy, MapPoint = new Point(16, 8) });
				enemies.Add(new SceneActor(new Actor(22, 55, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(48, 576)) { Faction = Faction.Enemy, MapPoint = new Point(9, 10) });
				enemies.Add(new SceneActor(new Actor(23, 55, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null), new Point(48, 576)) { Faction = Faction.Enemy, MapPoint = new Point(11, 12) });

				foreach (var enemy in enemies)
				{
					enemy.Visible = true;
					this.Scene.AddActionLoadActor(enemy);
				}

				#endregion Ememy appears

				#region dialog test

				//SceneActor mareeta = new SceneActor(new Actor(6, 60, 7, "玛莉塔"), new Point(264, 64));
				//SceneActor nanna = new SceneActor(new Actor(7, 60, 7, "南娜"), new Point(144, 0));

				//this.Scene.AddActionSpeech(boss1, "怎么样，发现了王子吗？", true);
				//this.Scene.AddActionSpeech(boss2, "没有，在村子里仔细搜查过了，可是仍旧没有发现。", false);
				//this.Scene.AddActionSpeech(boss1, "蠢货，在那里慢吞吞的干什么？兰斯特王国的余孽躲藏在这个村子里是绝对错不了的。把村民们关上的房间统统打开搜。", true);
				//this.Scene.AddActionSpeech(boss2, "是，绝对遵照您的意思去做！可是，不论怎么搜，他不在这个村子里是事实呀。村民说王子一行人与菲亚那义勇军一道去救助被海盗骚扰的村子去了。", false);
				//this.Scene.AddActionSpeech(boss1, "菲亚那义勇军？那是什么？", true);
				//this.Scene.AddActionSpeech(boss2, "这个村子现在是山贼的老窝。在十几年以前，有个叫艾维尔的流浪佣兵铲平了山贼，成为了新的统治者。现在听说被叫做义勇军，保护这附近的村子。不管怎样，是个相当厉害的女子…… 唔……这可有点麻烦呀！", false);
				//this.Scene.AddActionSpeech(boss2, "请不必担心。艾维尔的女儿已经被抓住，成了人质。而且王子的护卫、骑士菲恩的女儿也被抓住了。这样的话，这些家伙们大概无法反抗了吧。", false);
				//this.Scene.AddActionSpeech(boss1, "唔……你做得很不错嘛！好，我就把姑娘们一同带回曼斯塔了。你留在村子里，等待逆贼回来。王子如果回来的话，务必要将其抓住！", true);
				//this.Scene.AddActionSpeech(boss2, "是，请放心的交给我去作吧！来人！把姑娘们带到这儿来！", false);
				//this.Scene.AddActionSpeech(mareeta, string.Empty, false, 2f);
				//this.Scene.AddActionSpeech(boss1, "姑娘，叫什么名字？", true);
				//this.Scene.AddActionSpeech(mareeta, "哼……", false, 2f);
				//this.Scene.AddActionSpeech(nanna, string.Empty, false, 2f);
				//this.Scene.AddActionSpeech(boss1, "唔。顽固的样子也很可爱啊！那，另一个人呢……哦，你是菲恩的女儿吧？母亲好像是诺迪翁公国的公主吧！到底是正宗的血统啊！在这样的边境中生活着，仍旧没有失去高贵的气质啊！", true);
				//this.Scene.AddActionSpeech(nanna, "……", false, 2f);
				//this.Scene.AddActionSpeech(boss2, string.Empty, false, 2f);
				//this.Scene.AddActionSpeech(boss1, "唔，这倒是成了没有想到的好礼物啊！嗯，准备返回曼斯塔城了！瓦兹曼，后面的就交给你了！", true);
				//this.Scene.AddActionSpeech(boss2, "是。", false);

				#endregion dialog test

				#region Player appears

				SceneActor leaf = new SceneActor(new Actor(1, 1, 1, 0, "利夫", 4, 0, 0, 0, 1, 4, 0, 0, new int[] { 2 }), new Point(0, 0)) { Faction = Faction.Player, MapPoint = new Point(0, 20) };
				leaf.Growth = new ActorAttributes(70, 35, 10, 35, 40, 40, 25, 15);
				leaf.Items.Add(Weapon.Weapons[1]);
				this.Scene.AddActionLoadActor(leaf);
				this.Scene.AddActionMoveActor(leaf, new Point(3, 16), 200);

				SceneActor fin = new SceneActor(new Actor(5, 60, 7, 0, "菲恩", 8, 5, 1, 5, 7, 5, 4, 2, new int[] { 11 }), new Point(96, 0)) { Faction = Faction.Player, MapPoint = new Point(0, 20) };
				fin.Growth = new ActorAttributes(60, 35, 5, 30, 35, 45, 30, 10);
				fin.Items.Add(Weapon.Weapons[2]);
				this.Scene.AddActionLoadActor(fin);
				this.Scene.AddActionMoveActor(fin, new Point(2, 16), 200);

				SceneActor eyvel = new SceneActor(new Actor(4, 51, 12, 0, "艾维尔", 8, 5, 3, 10, 10, 10, 3, 3, new int[] { 2 }), new Point(192, 64)) { Faction = Faction.Player, MapPoint = new Point(0, 20) };
				eyvel.Growth = new ActorAttributes(30, 15, 10, 15, 10, 25, 5, 5);
				eyvel.Items.Add(Weapon.Weapons[1]);
				this.Scene.AddActionLoadActor(eyvel);
				this.Scene.AddActionMoveActor(eyvel, new Point(3, 15), 500);

				SceneActor othin = new SceneActor(new Actor(2, 48, 1, 0, "奥辛", 7, 2, 0, 3, 4, 3, 2, 3, new int[] { 12 }), new Point(480, 64)) { Faction = Faction.Player, MapPoint = new Point(0, 20) };
				othin.Growth = new ActorAttributes(85, 30, 5, 25, 35, 55, 25, 25);
				othin.Items.Add(Weapon.Weapons[3]);
				this.Scene.AddActionLoadActor(othin);
				this.Scene.AddActionMoveActor(othin, new Point(4, 15), 200);

				SceneActor halvan = new SceneActor(new Actor(3, 48, 2, 0, "哈鲁巴恩", 8, 3, 0, 3, 2, 2, 3, 4, new int[] { 4 }), new Point(432, 64)) { Faction = Faction.Player, MapPoint = new Point(0, 20) };
				halvan.Growth = new ActorAttributes(80, 40, 5, 20, 30, 30, 30, 30);
				halvan.Items.Add(Weapon.Weapons[3]);
				this.Scene.AddActionLoadActor(halvan);
				this.Scene.AddActionMoveActor(halvan, new Point(2, 15), 200);

				#endregion Player appears

				this.Scene.AddActionPause(1000d);
				this.Scene.AddDelayAction(() => this.Scene.ChangePhase(TurnPhase.PlayerPhase));

				#region riders invasion test

				//List<Point> pointList = new List<Point>();
				//Stack<SceneActor> actors = new Stack<SceneActor>();
				//int classId = 60;

				//for (int x = 0; x < this.layer.MapSize.Width; x++)
				//{
				//    for (int y = 0; y < this.layer.MapSize.Height; y++)
				//    {
				//        if (this.layer.Terrains[x, y] < 0x63)
				//        {
				//            pointList.Add(new Point(x, y));
				//            actors.Push(new SceneActor(new Actor(actors.Count + 1, classId, 1, 0, "曼斯塔兵", 0, 0, 0, 0, 0, 0, 0, 0, null))
				//            {
				//                Faction = Faction.Enemy,
				//                MapPoint = new Point(0, 20),
				//            });
				//        }
				//    }
				//}

				//SceneActor attacker;

				//foreach (var point in pointList.OrderBy(p => (this.layer.MapSize.Width - p.X + p.Y)))
				//{
				//    attacker = actors.Pop();
				//    this.layer.AddActionLoadActor(TimeSpan.Zero, attacker);
				//    this.layer.MoveActorInternal(TimeSpan.FromMilliseconds(50), attacker, point);
				//}

				#endregion riders invasion test
			}
		}
	}
}
