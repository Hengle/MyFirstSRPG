using System;

namespace MyFirstSRPG.SRPGGameLibrary
{
	[Flags]
	public enum Direction
	{
		Unknown = 0x0,
		Up = 0x1,
		Right = 0x2,
		Down = 0x4,
		Left = 0x8
	}
}