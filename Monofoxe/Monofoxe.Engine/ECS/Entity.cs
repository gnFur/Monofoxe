﻿using System;
using System.Collections.Generic;
using Monofoxe.Engine.SceneSystem;

namespace Monofoxe.Engine.ECS
{
	/// <summary>
	/// Parent class of every in-game object.
	/// Can hold components, or implement its own logic.
	/// </summary>
	public class Entity
	{
		/// <summary>
		/// Unique tag for identifying entity.
		/// NOTE: Entity tags should be unique!
		/// </summary>
		public readonly string Tag;
		
		/// <summary>
		/// Depth of Draw event. Objects with the lowest depth draw the last.
		/// </summary>
		public int Depth
		{
			get => _depth;
			set
			{
				if (value != _depth)
				{
					_depth = value;
					Layer._depthListOutdated = true;
				}
			}
		}
		private int _depth;


		/// <summary>
		/// Tells f object was destroyed.
		/// </summary>
		public bool Destroyed {get; internal set;} = false;

		/// <summary>
		/// If false, Update events won't be executed.
		/// </summary>
		public bool Enabled = true;
		
		/// <summary>
		/// If false, Draw events won't be executed.
		/// </summary>
		public bool Visible = true;
		

		public Layer Layer
		{
			get => _layer;
			set
			{
				if (_layer != null)
				{
					foreach(var componentPair in _components)
					{
						_layer.RemoveComponent(componentPair.Value);
					}
					_layer.RemoveEntity(this);
				}
				_layer = value;
				foreach(var componentPair in _components)
				{
					_layer.AddComponent(componentPair.Value);
				}
				_layer.AddEntity(this);
			}
		}
		private Layer _layer;

		public Scene Scene => _layer.Scene;


		/// <summary>
		/// Component hash table.
		/// </summary>
		private Dictionary<Type, Component> _components;


		public Entity(Layer layer, string tag = "entity")
		{
			_components = new Dictionary<Type, Component>();
			Tag = tag;
			Layer = layer;
		}


		
		#region Events.

		/*
		 * Event order:
		 * - FixedUpdate
		 * - Update
		 * - Draw
		 * 
		 * NOTE: Component events are executed before entity events.
		 */

		
		/// <summary>
		/// Updates at a fixed rate.
		/// </summary>
		public virtual void FixedUpdate() {}
		
		
		
		/// <summary>
		/// Updates every frame.
		/// </summary>
		public virtual void Update() {}
		
		

		/// <summary>
		/// Draw updates.
		/// 
		/// NOTE: DO NOT put any significant logic into Draw.
		/// It may skip frames.
		/// </summary>
		public virtual void Draw() {}
		


		/// <summary>
		///	Triggers right before destruction, if entity is active. 
		/// </summary>
		public virtual void Destroy() {}

		#endregion Events.



		#region Components.

		/// <summary>
		/// Adds component to the entity.
		/// </summary>
		public void AddComponent(Component component)
		{
			_components.Add(component.GetType(), component);
			component.Owner = this;
			Layer.AddComponent(component);
		}
		
		

		/// <summary>
		/// Returns component of given class.
		/// </summary>
		public T GetComponent<T>() where T : Component =>
			(T)_components[typeof(T)];
		
		/// <summary>
		/// Returns all the components. All of them.
		/// </summary>
		public Component[] GetAllComponents()
		{
			var array = new Component[_components.Count];
			var id = 0;

			foreach(var componentPair in _components)
			{
				array[id] = componentPair.Value;
				id += 1;
			}

			return array;
		}


		/// <summary>
		/// Checks of an entity has component with given tag.
		/// </summary>
		public bool HasComponent<T>() where T : Component =>
			_components.ContainsKey(typeof(T));
		
		
		/// <summary>
		/// Removes component from an entity and returns it.
		/// </summary>
		public Component RemoveComponent<T>()
		{
			var type = typeof(T);
			if (_components.ContainsKey(type))
			{
				var component = _components[type];
				_components.Remove(type);
				Layer.RemoveComponent(component);
				component.Owner = null;
				return component;
			}
			return null;
		}


		/// <summary>
		/// Removes all components.
		/// </summary>
		internal void RemoveAllComponents()
		{
			foreach(var componentPair in _components)
			{
				Layer.RemoveComponent(componentPair.Value);
			}
			_components.Clear();
		}

		#endregion Components.

	}
}
