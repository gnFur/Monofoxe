# Adding resources

Now we know how resource management in Monofoxe works, but how to access resources in the game?

All resources are loaded via `ResourceBox` classes. There are a couple of default resource boxes:

- `SpriteGroupResourceBox` - Loads sprites from the sprite group.
- `DirectoryResourceBox<T>` - Loads all resources of a single type from a specified folder.

 You can make custom resource boxes like this:

```c#
namespace Resources
{
	public class Fonts : ResourceBox<IFont>
	{
		private ContentManager _content;

		public Fonts() : base("Fonts")
		{
			_content = new ContentManager(GameMgr.Game.Services);
			_content.RootDirectory = ResourceInfoMgr.ContentDir + "/Fonts";
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

			AddResource("FancyFont", new TextureFont(fontSprite, 1, 1, TextureFont.Ascii, false));
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

```

As you can see, it has `Load()` and `Unload()` methods. They are called by the `ResourceHub` class to load and unload the resources.

To load another font, you should add a new entry in the class by hand. Additionally, you can use `ResourceInfoMgr.GetAssetPaths()` method, which returns names of all the assets in a specified path.



## [<< Nopipeline](NoPipeline.md) | [Resource Hub >>](ResourceHub.md) 

[<<< Contents](../Contents.md)

