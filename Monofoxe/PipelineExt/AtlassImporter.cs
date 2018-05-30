﻿using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.Xna.Framework;

/*
 * FUTURE NOTE:
 * To create Pipeline Extension project,
 * choose C# Class Library template,
 * then reference Monogame for Desktop GL
 * and get Monogame.Framework.Content.Pipeline
 * from NuGet.
 * 
 * To add library to pipeline project, reference
 * dll with project name.
 */
namespace PipelineExt
{
	/// <summary>
	/// Atlass importer. Parses json, loads texture and generates 
	/// frame array, which will be passed to AtlassProcessor.
	/// All atlasses should come in json-png pairs.
	/// Importer is oriented to TexturePacker JSON format. 
	/// </summary>
	[ContentImporter(".json", DefaultProcessor = "AtlassProcessor", 
	DisplayName = "Atlass Importer - Monofoxe")]
	public class AtlassImporter : ContentImporter<AtlassContainer<Frame>>
	{
		public override AtlassContainer<Frame> Import(string filename, ContentImporterContext context)
		{
			var atlassFrames = new AtlassContainer<Frame>();
			
			try
			{
				var textureImporter = new TextureImporter();
				TextureContent texture = textureImporter.Import(Path.ChangeExtension(filename, ".png"), context);	
				atlassFrames.Texture = texture;
			}
			catch(Exception)
			{
				// Each json must be paired with png texture with the same name.
				throw(new FileNotFoundException("Texture for " + Path.GetFileNameWithoutExtension(filename) + " is missing!"));
			}

			try
			{
				// Parsing config.
				var json = File.ReadAllText(filename);
				JToken framesData = JObject.Parse(json)["frames"];
				// Parsing config.

				foreach(JProperty token in framesData)
				{
					JToken frameData = token.Value;

					Point size = new Point(
						Int32.Parse(frameData["sourceSize"]["w"].ToString()),
						Int32.Parse(frameData["sourceSize"]["h"].ToString())
					);

					Point origin = new Point(
							Int32.Parse(frameData["spriteSourceSize"]["x"].ToString()),
							Int32.Parse(frameData["spriteSourceSize"]["y"].ToString())
					);

					Frame frame = new Frame(
						token.Name,
						size,
						origin,
						new Rectangle(
							Int32.Parse(frameData["frame"]["x"].ToString()),
							Int32.Parse(frameData["frame"]["y"].ToString()),
							Int32.Parse(frameData["frame"]["w"].ToString()),
							Int32.Parse(frameData["frame"]["h"].ToString())
						)
					);

					atlassFrames.Add(frame);
				}
			}
			catch(Exception)
			{
				throw(new InvalidContentException("Incorrect JSON format!"));
			}

			return atlassFrames;
			
		}
	}
}
