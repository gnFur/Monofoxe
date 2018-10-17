﻿using Monofoxe.Engine.Drawing;

namespace Monofoxe.Utils.Tilemaps
{
	public struct BasicTile : ITile
	{
		public int Index {get; private set;}
		public bool IsBlank => Tileset == null || Index == (Tileset.StartingIndex - 1);

		public Tileset Tileset;

		public BasicTile(int index, Tileset tileset)
		{
			Index = index;
			Tileset = tileset;
		}

		public Frame GetFrame() =>
			Tileset.GetFrame(Index);
	}
}
