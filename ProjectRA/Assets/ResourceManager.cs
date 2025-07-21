using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : SingletonMono<ResourceManager>
{
	Dictionary<string, Object> resources = new Dictionary<string, Object>();

	public T LoadResource<T>(string resourcePath) where T : Object
	{
		if (resources.TryGetValue(resourcePath, out Object resource))
		{
			return resource as T;
		}

		T loadedResource = Resources.Load<T>(resourcePath);
		if (loadedResource != null)
		{
			resources[resourcePath] = loadedResource;
			return loadedResource;
		}

		Debug.LogWarning($"Resource at path '{resourcePath}' not found.");
		return null;
	}
}
