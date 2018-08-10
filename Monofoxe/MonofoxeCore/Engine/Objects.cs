﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Engine;
using System.Diagnostics;


namespace Monofoxe.Engine
{
	public static class Objects
	{

		/// <summary>
		/// List of all game objects.
		/// </summary>
		public static List<Entity> GameObjects = new List<Entity>();

		/// <summary>
		/// List of newly created game objects. Since it won't be that cool to modify main list 
		/// in mid-step, they'll be added in next one.
		/// </summary>
		private static List<Entity> _newGameObjects = new List<Entity>();

		private static double _fixedUpdateAl;


		internal static void Update(GameTime gameTime)
		{
			// Clearing main list from destroyed objects.
			var updatedList = new List<Entity>();
			foreach(Entity obj in GameObjects)
			{
				if (!obj.Destroyed)
				{
					updatedList.Add(obj);
				}
			}
			GameObjects = updatedList;
			// Clearing main list from destroyed objects.


			// Adding new objects to the list.
			GameObjects.AddRange(_newGameObjects);
			_newGameObjects.Clear();
			// Adding new objects to the list.


			// Fixed updates.
			_fixedUpdateAl += gameTime.ElapsedGameTime.TotalSeconds;

			if (_fixedUpdateAl >= GameCntrl.FixedUpdateRate)
			{
				var overflow = (int)(_fixedUpdateAl / GameCntrl.FixedUpdateRate); // In case of lags.
				_fixedUpdateAl -= GameCntrl.FixedUpdateRate * overflow;

				foreach(Entity obj in GameObjects)
				{
					if (obj.Active)
					{
						obj.FixedUpdateBegin();
					}
				}

				foreach(Entity obj in GameObjects)
				{
					if (obj.Active)
					{
						obj.FixedUpdate();
					}
				}

				foreach(Entity obj in GameObjects)
				{
					if (obj.Active)
					{
						obj.FixedUpdateEnd(); 
					}
				}
			}
			// Fixed updates.


			// Normal updates.
			foreach(Entity obj in GameObjects)
			{
				if (obj.Active)
				{
					obj.UpdateBegin();
				}
			}

			ECS.ECSMgr.Update();
			foreach(Entity obj in GameObjects)
			{
				if (obj.Active)
				{ 
					obj.Update(); 
				}
			}

			foreach(Entity obj in GameObjects)
			{
				if (obj.Active)
				{ 
					obj.UpdateEnd();
				}
			}
			// Normal updates.
		}





		/// <summary>
		/// Adds object to object list.
		/// </summary>
		internal static void AddObject(Entity obj) => 
			_newGameObjects.Add(obj);




		#region User functions. 

		/// <summary>
		/// Returns list of objects of certain type.
		/// </summary>
		public static List<T> GetList<T>() where T : Entity => 
			GameObjects.OfType<T>().ToList();


		/// <summary>
		/// Counts amount of objects of certain type.
		/// </summary>
		public static int Count<T>() where T : Entity => 
			GameObjects.OfType<T>().Count();


		/// <summary>
		/// Destroys game object.
		/// </summary>
		public static void Destroy(Entity obj)
		{
			if (!obj.Destroyed)
			{
				obj.Destroyed = true;
				if (obj.Active)
				{
					obj.Destroy();
				}
			}
		}


		/// <summary>
		/// Checks if given instance exists.
		/// </summary>
		public static bool ObjExists<T>() where T : Entity
		{
			foreach(Entity obj in GameObjects)
			{
				if (obj is T)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Finds n-th object of given type.
		/// </summary>
		/// <typeparam name="T">Type to search.</typeparam>
		/// <param name="count">Number of the object in object list.</param>
		/// <returns>Returns object if it was found, or null, if it wasn't.</returns>
		public static T ObjFind<T>(int count) where T : Entity
		{
			var counter = 0;

			foreach(Entity obj in GameObjects)
			{
				if (obj is T)
				{
					if (counter >= count)
					{
						return (T)obj;
					}
					counter += 1;
				}
			}
			return null;
		}

		#endregion User functions.


		#region ECS functions.

		/// <summary>
		/// Returns list of objects of certain type.
		/// </summary>
		public static List<Entity> GetList(string tag)
		{
			var list = new List<Entity>();

			foreach(Entity obj in GameObjects)
			{
				if (obj.Tag == tag)
				{
					list.Add(obj);
				}
			}
			return list;
		}
		

		/// <summary>
		/// Counts amount of objects of certain type.
		/// </summary>
		public static int Count(string tag)
		{
			var counter = 0;

			foreach(Entity obj in GameObjects)
			{
				if (obj.Tag == tag)
				{
					counter += 1;
				}
			}
			return counter;
		}
		

		/// <summary>
		/// Checks if given instance exists.
		/// </summary>
		public static bool ObjExists(string tag)
		{
			foreach(Entity obj in GameObjects)
			{
				if (obj.Tag == tag)
				{
					return true;
				}
			}
			return false;
		}
		

		/// <summary>
		/// Finds n-th object of given type.
		/// </summary>
		/// <typeparam name="T">Type to search.</typeparam>
		/// <param name="count">Number of the object in object list.</param>
		/// <returns>Returns object if it was found, or null, if it wasn't.</returns>
		public static Entity ObjFind(string tag, int count)
		{
			var counter = 0;

			foreach(Entity obj in GameObjects)
			{
				if (obj.Tag == tag)
				{
					if (counter >= count)
					{
						return obj;
					}
					counter += 1;
				}
			}
			return null;
		}

		#endregion ECS functions.

	}
}
