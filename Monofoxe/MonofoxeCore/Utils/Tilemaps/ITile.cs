﻿
namespace Monofoxe.Utils.Tilemaps
{
	/// <summary>
	/// Tile interface. 
	/// </summary>
	public interface ITile
	{
		int Index {get;}
		bool IsBlank {get;}
		bool FlipHor {get; set;}
		bool FlipVer {get; set;}
	}
}
