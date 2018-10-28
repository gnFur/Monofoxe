﻿using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Monofoxe.Tiled.MapStructure;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.Xna.Framework;

namespace Pipefoxe.Tiled
{
	[ContentTypeWriter]
	public class TiledMapWriter : ContentTypeWriter<TiledMap>
	{
		protected override void Write(ContentWriter output, TiledMap map)
		{
			output.WriteObject(map.BackgroundColor);
			output.Write(map.Width);
			output.Write(map.Height);
			output.Write(map.TileWidth);
			output.Write(map.TileHeight);
			
			WriteTilesets(output, map.Tilesets);
			WriteTileLayers(output, map.TileLayers);

			output.WriteObject(map.Properties);
		}

		

		void WriteTilesets(ContentWriter output, TiledMapTileset[] tilesets)
		{
			output.Write(tilesets.Length);
			foreach(var tileset in tilesets)
			{
				output.Write(tileset.Name);
				output.WriteObject(tileset.TexturePaths);
		
				output.Write(tileset.FirstGID);
				output.Write(tileset.TileWidth);
				output.Write(tileset.TileHeight);
				output.Write(tileset.Spacing);
				output.Write(tileset.Margin);
				output.Write(tileset.TileCount);
				output.Write(tileset.Columns);

				output.Write(tileset.Offset);

				foreach(var tile in tileset.Tiles)
				{
					output.WriteObject(tile);
				}
				
				output.WriteObject(tileset.BackgroundColor);
				output.WriteObject(tileset.Properties);
			}
		}


		void WriteLayer(ContentWriter output, TiledMapLayer layer)
		{
			output.Write(layer.Name);
			output.Write(layer.ID);
			output.Write(layer.Visible);
			output.Write(layer.Opacity);
			output.Write(layer.Offset);
			
			output.WriteObject(layer.Properties);
		}

		void WriteTileLayers(ContentWriter output, TiledMapTileLayer[] layers)
		{
			output.Write(layers.Length);
			foreach(var layer in layers)
			{
				WriteLayer(output, layer);
				output.Write(layer.Width);
				output.Write(layer.Height);
				output.Write(layer.TileWidth);
				output.Write(layer.TileHeight);
				
				for(var y = 0; y < layer.Height; y += 1)
				{
					for(var x = 0; x < layer.Width; x += 1)
					{
						output.WriteObject(layer.Tiles[x][y]);
					}
				}
			}
		}



		

		public override string GetRuntimeType(TargetPlatform targetPlatform) =>
			"Monofoxe.Tiled.MapStructure.TiledMap, Monofoxe.Tiled";



		public override string GetRuntimeReader(TargetPlatform targetPlatform) =>
			"Monofoxe.Tiled.ContentReaders.TiledMapReader, Monofoxe.Tiled";
	}
}

