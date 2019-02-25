﻿using System;
using Microsoft.Xna.Framework;

namespace Monofoxe.Engine.Utils
{
	/// <summary>
	/// Vector2 extensions.
	/// </summary>
	public static class Vector2Extensions
	{
		/// <summary>
		/// Rounds each vector's component.
		/// </summary>
		public static Vector2 Round(this Vector2 v) =>
			new Vector2((float)Math.Round(v.X), (float)Math.Round(v.Y));
		
		/// <summary>
		/// Rounds each vector's component down.
		/// </summary>
		public static Vector2 Floor(this Vector2 v) =>
			new Vector2((float)Math.Floor(v.X), (float)Math.Floor(v.Y));

		/// <summary>
		/// Rounds each vector's component up.
		/// </summary>
		public static Vector2 Ceiling(this Vector2 v) =>
			new Vector2((float)Math.Ceiling(v.X), (float)Math.Ceiling(v.Y));
		
		/// <summary>
		/// Returns vector with the same direction and length of 1. 
		/// If original vector is (0;0), returns zero vector.
		/// </summary>
		public static Vector2 GetSafeNormalize(this Vector2 v)
		{
			if (v == Vector2.Zero)
			{
				return Vector2.Zero;
			}
			v.Normalize();
			return v;
		}
	}
}
