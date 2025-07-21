using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameObjectPoolable
{
	public void Init();
}

public class GameObjectPool
{
	Stack<GameObject> m_objects;
	int m_maxSize;
	GameObject m_prefab;
	Transform m_transform;
	public GameObjectPool(GameObject prefab, Transform transform, int size = 4)
	{
		m_objects = new Stack<GameObject>(size);
		m_maxSize = size;
		m_prefab = prefab;
		m_transform = transform;

		for (int i = 0; i < m_maxSize; ++i)
		{
			GameObject newObject = GameObject.Instantiate(prefab, transform);
			newObject.SetActive(false);
			m_objects.Push(newObject);
		}
	}
	public GameObject NewObjectInstance()
	{
		if (m_objects.Count == 0)
		{
			Expand();
		}

		GameObject retVal = m_objects.Pop();
		return retVal;
	}
	void Expand()
	{
		for (int i = 0; i < m_maxSize; ++i)
		{
			GameObject newObject = GameObject.Instantiate(m_prefab, m_transform);
			newObject.SetActive(false);
			m_objects.Push(newObject);
		}
		m_maxSize += m_maxSize;
	}
	public void ReturnObjectInstance(GameObject obj)
	{
		m_objects.Push(obj);
	}
}

public class GameObjectPool<T> where T : Component
{
	Stack<T> m_objects;
	int m_maxSize;
	GameObject m_prefab;
	Transform m_transform;
	public GameObjectPool(GameObject prefab, Transform transform, int size = 4)
	{
		m_objects = new Stack<T>(size);
		m_maxSize = size;
		m_prefab = prefab;
		m_transform = transform;

		for (int i = 0; i < m_maxSize; ++i)
		{
			GameObject newObject = GameObject.Instantiate(prefab, transform);
			T component = newObject.GetComponent<T>();
			if(component == null)
			{
				component = newObject.AddComponent<T>();
			}
			newObject.SetActive(false);

			m_objects.Push(component);
		}
	}
	public T NewObjectInstance()
	{
		if (m_objects.Count == 0)
		{
			Expand();
		}

		T retVal = m_objects.Pop();
		return retVal;
	}
	void Expand()
	{
		for (int i = 0; i < m_maxSize; ++i)
		{
			GameObject newObject = GameObject.Instantiate(m_prefab, m_transform);
			T component = newObject.GetComponent<T>();
			if (component == null)
			{
				component = newObject.AddComponent<T>();
			}
			newObject.SetActive(false);

			m_objects.Push(component);
		}
		m_maxSize += m_maxSize;
	}
	public void ReturnObjectInstance(T obj)
	{
		m_objects.Push(obj);
	}
}