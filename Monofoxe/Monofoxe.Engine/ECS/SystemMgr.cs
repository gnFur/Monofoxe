﻿using System;
using System.Collections.Generic;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Engine.Utils.CustomCollections;

namespace Monofoxe.Engine.ECS
{
	/// <summary>
	/// Manages ECS systems.
	/// </summary>
	public static class SystemMgr
	{
		/// <summary>
		/// List of currently active systems.
		/// </summary>
		internal static SafeSortedDictionary<Type, BaseSystem> _activeSystems 
			= new SafeSortedDictionary<Type, BaseSystem>(x => x.Priority);
		
		/// <summary>
		/// Pool of all game systems.
		/// </summary>
		internal static Dictionary<Type, BaseSystem> _systemPool = new Dictionary<Type, BaseSystem>();

		/// <summary>
		/// Tells if any components were removed in the current step.
		/// </summary>
		internal static bool _componentsWereRemoved = false;


		/// <summary>
		/// If true, systems will be enabled and disabled automatically.
		/// </summary>
		public static bool AutoSystemManagement 
		{
			get => _autoSystemManagement;
			set
			{
				_autoSystemManagement = value;
				_componentsWereRemoved = true;
			}
		}
		static bool _autoSystemManagement = true;


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
		internal static void FixedUpdate(ComponentContainer components)
		{
			foreach(var system in _activeSystems)
			{
				if (components.TryGetList(system.ComponentType, out List<Component> componentList))
				{
					system.FixedUpdate(componentList.FindAll(x => x.Owner.Enabled));
				}
			}
		}


		/// <summary>
		/// Updates every frame.
		/// </summary>
		internal static void Update(ComponentContainer components)
		{
			foreach(var system in _activeSystems)
			{
				if (components.TryGetList(system.ComponentType, out List<Component> componentList))
				{
					componentList = componentList.FindAll(x => x.Owner.Enabled);
					if (componentList.Count > 0)
					{
						system._usedByLayers = true; // Telling that a layer is using this system.
					}

					system.Update(componentList);
				}
			}
		}
		

		/// <summary>
		/// Draw updates.
		/// 
		/// NOTE: DO NOT put any significant logic into Draw.
		/// It may skip frames.
		/// </summary>
		internal static void Draw(Component component)
		{
			if (_activeSystems.TryGetValue(component.GetType(), out BaseSystem system))
			{
				system.Draw(component);
			}
		}

		#endregion Events.


		/// <summary>
		/// Creates an instance of each ISystem implementing class.
		/// </summary>
		internal static void InitSystemPool()
		{

			// Extracting all types from assemblies.
			var systemTypes = new List<Type>();

			foreach(var typePair in GameMgr.Types)
			{
				if (typeof(BaseSystem).IsAssignableFrom(typePair.Value))
				{
					systemTypes.Add(typePair.Value);
				}
			}
			// Extracting all types from assemblies.
			
			
			// Creating an instance of each system.
			foreach(var systemType in systemTypes)
			{
				if (systemType != typeof(BaseSystem))
				{
					var newSystem = (BaseSystem)Activator.CreateInstance(systemType);
					_systemPool.Add(newSystem.ComponentType, newSystem);
				}
			}
			// Creating an instance of each system.
		}



		/// <summary>
		/// Puts system in active system list from system pool.
		/// </summary>
		public static void EnableSystem<T>() where T : BaseSystem
		{
			foreach(var systemPair in _systemPool)
			{
				if (systemPair.Value is T)
				{
					_activeSystems.Add(systemPair.Key, systemPair.Value);
					return;
				}
			}
		}


		/// <summary>
		/// Removes system from active system list and puts it back in pool.
		/// </summary>
		public static void DisableSystem<T>() where T : BaseSystem
		{
			foreach(var system in _activeSystems)
			{
				if (system is T)
				{
					_activeSystems.Remove(system.ComponentType);
					return;
				}
			}
		}
		

		/// <summary>
		/// Puts system in active system list from system pool.
		/// </summary>
		public static void EnableSystem(Type type)
		{
			foreach(var systemPair in _systemPool)
			{
				// Systems are sorted by component type, so we cannot use systemPair.Key.
				if (systemPair.Value.GetType() == type) 
				{
					_activeSystems.Add(systemPair.Key, systemPair.Value);
					return;
				}
			}
		}


		/// <summary>
		/// Removes system from active system list and puts it back in pool.
		/// </summary>
		public static void DisableSystem(Type type)
		{
			foreach(var system in _activeSystems)
			{
				if (system.GetType() == type) 
				{
					_activeSystems.Remove(system.ComponentType);
					return;
				}
			}
		}



		/// <summary>
		/// Enables and disables systems depending on if there are any components for them.
		/// </summary>
		internal static void UpdateActiveSystems()
		{
			// Disabling systems without components.
			if (AutoSystemManagement && _componentsWereRemoved)
			{
				var unusedSystems = new List<Type>();
				foreach(var system in _activeSystems)
				{
					if (!system._usedByLayers)
					{
						unusedSystems.Add(system.ComponentType);	
						_componentsWereRemoved = false;
					}
				}
				foreach(var systemComponentType in unusedSystems)
				{
					_activeSystems.Remove(systemComponentType);
				}
			}
			// Disabling systems without components.
			
			// Resetting system counters.
			foreach(var system in _activeSystems)
			{
				system._usedByLayers = false;
			}
			// Resetting system counters.

			// Managing new components.
			foreach(var scene in SceneMgr.Scenes)
			{
				foreach(var layer in scene.Layers)
				{
					if (layer._newComponents.Count > 0)
					{
						foreach(var component in layer._newComponents)
						{
							var componentType = component.GetType();
							
							if (!_activeSystems.TryGetValue(componentType, out BaseSystem activeSystem) && AutoSystemManagement)
							{
								if (_systemPool.TryGetValue(componentType, out BaseSystem newSystem))
								{
									_activeSystems.Add(componentType, newSystem);

									if (!component.Initialized)
									{
										newSystem.Create(component);
										component.Initialized = true;
									}
								}
							}
							else
							{
								if (!component.Initialized)
								{
									activeSystem.Create(component);
									component.Initialized = true;
								}
							}
							
							layer._components.Add(component);
						}
						layer._newComponents.Clear();
					}
				}
			}	
			// Managing new components.
		}
		


	}
}
