﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.ECS;


namespace Monofoxe.Engine.Utils.Tilemaps
{
	/// <summary>
	/// System for basic tilemap. Based on Monofoxe.ECS.
	/// Draws tilemaps in camera's bounds.
	/// </summary>
	public class BasicTilemapSystem : BaseSystem
	{
		public override Type ComponentType => typeof(BasicTilemapComponent);

		public override void Draw(Component component)
		{
			var tilemap = (BasicTilemapComponent)component;

			var offsetCameraPos = GraphicsMgr.CurrentCamera.Position
				- tilemap.Offset
				- GraphicsMgr.CurrentCamera.Offset / GraphicsMgr.CurrentCamera.Zoom;

			var scaledCameraSize = GraphicsMgr.CurrentCamera.Size / GraphicsMgr.CurrentCamera.Zoom;
			var startX = (int)(offsetCameraPos.X / tilemap.TileWidth) - tilemap.Padding;
			var startY = (int)(offsetCameraPos.Y / tilemap.TileHeight) - tilemap.Padding;

			var endX = startX + (int)scaledCameraSize.X / tilemap.TileWidth + tilemap.Padding + 2; // One for mama, one for papa.
			var endY = startY + (int)scaledCameraSize.Y / tilemap.TileHeight + tilemap.Padding + 2;

			// It's faster to determine bounds for whole region.

			// Bounding.
			if (startX < 0)
			{
				startX = 0;
			}
			if (startY < 0)
			{
				startY = 0;
			}
			if (endX > tilemap.Width)
			{
				endX = tilemap.Width;
			}
			if (endY > tilemap.Height)
			{
				endY = tilemap.Height;
			}
			// Bounding.

			// Telling whatever is waiting to be drawn to draw itself.
			// If graphics mode is not switched, drawing raw sprite batch may interfere with primitives.
			GraphicsMgr.SwitchGraphicsMode(GraphicsMode.Sprites); 

			for(var y = startY; y < endY; y += 1)
			{
				for(var x = startX; x < endX; x += 1)
				{
					// It's fine to use unsafe get, since we know for sure, we are in bounds.
					var tile = tilemap.GetTileUnsafe(x, y);

					if (!tile.IsBlank)
					{
						var tilesetTile = tile.GetTilesetTile();
						
						if (tilesetTile != null)
						{
							var flip = SpriteEffects.None;
							var offset = Vector2.UnitY * (tilesetTile.Frame.Height - tilemap.TileHeight);
							var rotation = 0;
							
							// A bunch of Tiled magic.
							/*
							 * Ok, so here's the deal.
							 * Monogame, understandibly, has no diagonal flip,
							 * so it's implemented by offsetting, rotating, and horizontal flipping.
							 * Also, order actually matters. Diagonal flip should always go first.
							 * 
							 * Yes, this can be implemented with primitives. 
							 * If you've got nothing better to do -- go bananas.
							 * 
							 * I'm really sorry, if you'll need to modify this.
							 */
							if (tile.FlipDiag)
							{
								rotation = -90;
								offset.Y = -tilemap.TileHeight;
								offset.X = 0;

								flip |= SpriteEffects.FlipHorizontally;
							}

							if (tile.FlipHor)
							{
								// Sprite is rotated by 90 degrees, so X axis becomes Y.
								if (tile.FlipDiag)
								{
									flip ^= SpriteEffects.FlipVertically;
								}
								else
								{
									flip ^= SpriteEffects.FlipHorizontally;
								}
							}

							if (tile.FlipVer)
							{
								if (tile.FlipDiag)
								{
									flip ^= SpriteEffects.FlipHorizontally;
								}
								else
								{
									flip ^= SpriteEffects.FlipVertically;
								}
							}
							// A bunch of Tiled magic.

							// Mass-drawing srpites with spritebatch is a bit faster.
							GraphicsMgr.Batch.Draw(
								tilesetTile.Frame.Texture,
								tilemap.Offset + new Vector2(tilemap.TileWidth * x, tilemap.TileHeight * y) - offset + tile.Tileset.Offset,
								tilesetTile.Frame.TexturePosition,
								Color.White,
								MathHelper.ToRadians(rotation),
								Vector2.Zero,
								Vector2.One,
								flip,
								0
							);
						}
					}

				}

			}
		}

	}
}
