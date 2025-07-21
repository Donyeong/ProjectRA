using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public interface IObjectPoolable
{
	public void Init();
}

public class StaticObjectPool<T> : ObjectPool<T> where T : IObjectPoolable, new()
{
	static StaticObjectPool<T> m_instance = null;

	static StaticObjectPool<T> Instance
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new StaticObjectPool<T>();
			}
			return m_instance;
		}
	}
	public static T NewObject()
	{
		return Instance.NewObjectInstance();
	}
	public static void ReturnObject(T obj)
	{
		Instance.ReturnObjectInstance(obj);
	}
	Stack<T> m_objects;
	int m_maxSize;

}


public class ObjectPool<T> where T : IObjectPoolable, new()
{
	Stack<T> m_objects;
	int m_maxSize;

	public ObjectPool(int size = 100)
	{
		m_objects = new Stack<T>(size);
		m_maxSize = size;

		for (int i = 0; i < m_maxSize; ++i)
		{
			T newObject = new T();
			m_objects.Push(newObject);
		}
	}
	protected T NewObjectInstance()
	{
		if (m_objects.Count == 0)
		{
			Expand();
		}

		T retVal = m_objects.Pop();
		retVal.Init();
		return retVal;
	}
	void Expand()
	{
		for (int i = 0; i < m_maxSize; ++i)
		{
			T newObject = new T();
			m_objects.Push(newObject);
		}
		m_maxSize += m_maxSize;
	}
	protected void ReturnObjectInstance(T obj)
	{
		m_objects.Push(obj);
	}
}