using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSingleton<T> where T : class, new()
{
	protected static T m_instance;

	static public T Instance
	{
		get
		{
			if (m_instance == null)
				m_instance = new T();

			return m_instance;
		}
	}

	public virtual void DestroyInst()
	{
		m_instance = null;
		m_instance = new T();
	}
}

public class SingletonMono<T> : MonoBehaviour where T : Component
{
    protected static T m_instance;

    public bool IsDontDestroy = false;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {

                m_instance = FindObjectOfType<T>();
                if (m_instance == null)
                {
                    Debug.LogError($"not found {typeof(T).ToString()} by scene");
                    Debug.Break();
                }

            }

            return m_instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this as T;

            if (IsDontDestroy)
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        m_instance = null;
    }
}