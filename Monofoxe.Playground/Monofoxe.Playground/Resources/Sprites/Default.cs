// Template tags: 
// Default - Name of output class.
// Default - Name of current group.
// <sprite_name> - Name of each sprite.
// <sprite_hash_name> - Hash name of each sprite.


using Microsoft.Xna.Framework.Content;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using System.Collections.Generic;

// NOTE: This class is automatically generated by
// Monofoxe. See .cstemplate file.

namespace Resources.Sprites
{
	public static class Default
	{
		#region Sprites.
		public static Sprite Monofoxe;
		public static Sprite Fire;
		public static Sprite Bot;
		public static Sprite Player;
		public static Sprite Font;
		public static Sprite Test;
		public static Sprite AutismCat;
		#endregion Sprites.
		
		private static string _groupName = "Default";
		private static ContentManager _content = new ContentManager(GameMgr.Game.Services);
		
		public static bool Loaded = false;
		
		public static void Load()
		{
			Loaded = true;
			var graphicsPath = AssetMgr.ContentDir + '/' + AssetMgr.GraphicsDir +  '/' + _groupName;
			var sprites = _content.Load<Dictionary<string, Sprite>>(graphicsPath);
			
			#region Sprite constructors.
			
			Monofoxe = sprites["monofoxe"];
			Fire = sprites["fire"];
			Bot = sprites["bot"];
			Player = sprites["player"];
			Font = sprites["font"];
			Test = sprites["Folder/test"];
			AutismCat = sprites["Textures/autism_cat"];
			
			#endregion Sprite constructors.
		}
		
		public static void Unload()
		{
			_content.Unload();
			Loaded = false;
		}
	}
}
