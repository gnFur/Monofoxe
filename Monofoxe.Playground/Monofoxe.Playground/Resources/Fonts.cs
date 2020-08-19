﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.Resources;

namespace Resources
{
	public class Fonts : ResourceBox<IFont>
	{
		private ContentManager _content;

		static readonly string Ascii = " !" + '"' + @"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
		
		public override string Name => "Fonts";

		public Fonts()
		{
			_content = new ContentManager(GameMgr.Game.Services);
			_content.RootDirectory = ResourceInfoMgr.ContentDir + '/' + ResourceInfoMgr.FontsDir;
		}

		public override void Load()
		{
			if (Loaded)
			{
				return;
			}
			Loaded = true;

			AddResource("Arial", new Font(_content.Load<SpriteFont>("Arial")));

			var fontSprite = ResourceHub.GetResource<Sprite>("DefaultSprites", "Font");

			AddResource("FancyFont", new TextureFont(fontSprite, 1, 1, Ascii, false));
		}

		public override void Unload()
		{
			if (!Loaded)
			{
				return;
			}
			Loaded = false;
			_content.Unload();
		}
	}
}
