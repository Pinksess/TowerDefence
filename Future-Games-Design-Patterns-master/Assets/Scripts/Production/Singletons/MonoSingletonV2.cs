using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// This singleton doesn't contains any kind of Unity object serialized dependency
/// </summary>
[AttributeUsage(validOn: AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class MonoSingletonSecureAttribute : Attribute
{
	
}

/// <summary>
/// We have to set the path where this object is loaded from resources folder,
/// if the object doesn't include this attribute an invalid operation exception is gonna be raised.
/// </summary>
[AttributeUsage(validOn:AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class MonoSingletonConfigurationAttribute : Attribute
{
	public string ResourcesPath { get; }

	public MonoSingletonConfigurationAttribute(string resourcesPath)
	{
		ResourcesPath = resourcesPath;
	}
}


public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T m_Instance;

    public static T Instance {
		get
		{
			if (m_Instance == null)
			{
				T[] instances = FindObjectsOfType<T>();

				if (instances.Length > 1)
				{
					throw new InvalidOperationException($"There is more than one { typeof(T).Name } instance in the scene");
				}

				if (instances.Length > 0)
				{
					m_Instance = instances[0];					
				}

				if (m_Instance == null)
				{
					Attribute attribute = typeof(T).GetCustomAttribute(typeof(MonoSingletonSecureAttribute));					

					if (attribute is MonoSingletonSecureAttribute secureAttribute)
					{
						GameObject go = new GameObject(typeof(T).Name);
						m_Instance = go.AddComponent<T>();						
					}
					else
					{
						Attribute customAttribute = typeof(T).GetCustomAttribute(typeof(MonoSingletonConfigurationAttribute));

						if (customAttribute is MonoSingletonConfigurationAttribute singletonConfig)
						{
							string path = singletonConfig.ResourcesPath;
							GameObject singletonPrefab = Resources.Load<GameObject>(path);

							if (singletonPrefab == null)
							{
								throw new NullReferenceException($"There is no { typeof(T).Name } prefab in the Resources folder");
							}
					
							GameObject gOInstance = Instantiate(singletonPrefab);
							m_Instance = gOInstance.GetComponent<T>();

							if (m_Instance == null)
							{
								throw new NullReferenceException($"There is no { typeof(T).Name } component attached to the singleton prefab");	
							}	
						}
						else
						{
							throw new InvalidOperationException($"The singleton type {typeof(T).Name} doesn't include the mandatory attribute {typeof(MonoSingletonConfigurationAttribute)}");
						}
					}					
				}				
				DontDestroyOnLoad(m_Instance.gameObject);
			}	
			return m_Instance;
		}    
	}

	protected virtual void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = (T)this; //GetComponent<T>();
			DontDestroyOnLoad(gameObject);
		} 
		else if (m_Instance != this)
		{
			throw new InvalidOperationException($"There is more than one { typeof(T).Name } instance in the scene");
		}		
	}
}
