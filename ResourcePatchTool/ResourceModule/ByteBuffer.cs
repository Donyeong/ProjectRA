
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
#if UNITY_ENGINE
using UnityEngine;
#endif

#if UNITY_ENGINE
public class ByteBuffer : IDisposable, IObjectPoolable
#else
public class ByteBuffer : IDisposable
#endif
{

    private static Dictionary<Type, MethodInfo> createMethodCache = new Dictionary<Type, MethodInfo>();
	List<byte> Buff;
	byte[] readBuff;
	int readpos;
	bool buffUpdate = false;

	public ByteBuffer()
	{
		Buff = new List<byte>();
		readpos = 0;
	}

	public int GetReadPos()
	{
		return readpos;
	}

	public byte[] ToArray()
	{
		return Buff.ToArray();
	}

	public int Count()
	{
		return Buff.Count;
	}

	public int Length()
	{
		return Count() - readpos;
	}

	public void Clear()
	{
		Buff.Clear();
		readpos = 0;
	}

	public void AddReadPos(int pos)
	{

		if (Buff.Count > readpos)
		{
			if (buffUpdate)
			{
				readBuff = Buff.ToArray();
				buffUpdate = false;
			}

			if (Buff.Count > readpos + pos)
			{
				readpos += pos;
			}
			return;
		}
		else
		{
			throw new Exception("Byte Buffer Past Limit!");
		}
	}

	#region"Write Data"

#if UNITY_ENGINE
	public void WriteVector3(Vector3 vec)
	{
		WriteFloat(vec.x);
		WriteFloat(vec.y);
		WriteFloat(vec.z);
	}

	public void WriteQuaternion(Quaternion rot)
	{
		WriteFloat(rot.x);
		WriteFloat(rot.y);
		WriteFloat(rot.z);
		WriteFloat(rot.w);
	}
#endif

	public void WriteByte(byte Inputs)
	{
		Buff.Add(Inputs);
		buffUpdate = true;
	}

	public void WriteBytes(byte[] Input)
	{
		Buff.AddRange(Input);
		buffUpdate = true;
	}

	public void WriteShort(short Input)
	{
		Buff.AddRange(BitConverter.GetBytes(Input));
		buffUpdate = true;
	}
	public void WriteLong(long Input)
	{
		Buff.AddRange(BitConverter.GetBytes(Input));
		buffUpdate = true;
	}

	public void WriteInteger(int Input)
	{
		Buff.AddRange(BitConverter.GetBytes(Input));
		buffUpdate = true;
	}

	public void WriteFloat(float Input)
	{
		Buff.AddRange(BitConverter.GetBytes(Input));
		buffUpdate = true;
    }

    public void WriteDouble(double Input)
    {
        Buff.AddRange(BitConverter.GetBytes(Input));
        buffUpdate = true;
    }

    public void WriteBool(bool Input)
	{
		Buff.AddRange(BitConverter.GetBytes(Input));
		buffUpdate = true;
	}

	public void WriteStringUnicode(string Input)
	{
		Buff.AddRange(BitConverter.GetBytes(Input.Length * 2));
		Buff.AddRange(Encoding.Unicode.GetBytes(Input));
		buffUpdate = true;
	}

	public void WriteString(string Input)
	{
		Buff.AddRange(BitConverter.GetBytes(Input.Length));
		Buff.AddRange(Encoding.ASCII.GetBytes(Input));
		buffUpdate = true;
	}

	void WriteCustom(Type type, object input)
	{
		var method = this.GetType().GetMethod("WriteCustom", new Type[] { typeof(object) });
		var genericMethod = method.MakeGenericMethod(type);
		genericMethod.Invoke(this, new object[] { input });
	}

	public void WriteCustom<T>(object input)
    {
        if (typeof(T) == typeof(int))
        {
            WriteInteger((int)input);
        }
        else if (typeof(T) == typeof(string))
        {
            WriteString((string)input);
		}
		else if (typeof(T) == typeof(bool))
		{
			WriteBool((bool)input);
		}
		else if (typeof(T) == typeof(float))
        {
            WriteFloat((float)input);
        }
#if UNITY_ENGINE
		else if (typeof(T) == typeof(Vector3))
		{
			WriteVector3((Vector3)input);
		}
#endif
		else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Dictionary<,>))
		{
			Type keyType = typeof(T).GetGenericArguments()[0];
			Type valueType = typeof(T).GetGenericArguments()[1];


			IDictionary dictionary = (IDictionary)input;

			int dict_size = dictionary.Count;
			WriteInteger(dict_size);
			foreach (DictionaryEntry entry in dictionary)
			{
				WriteCustom(keyType, entry.Key);
				WriteCustom(valueType, entry.Value);
			}
		}
		else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
		{
			Type elementType = typeof(T).GetGenericArguments()[0];

			IList list = (IList)input;
			int listSize = list.Count;
			WriteInteger(listSize);

			foreach (var element in list)
			{
				WriteCustom(elementType, element);
			}

        }
#if UNITY_ENGINE
		else if (typeof(ISerializable).IsAssignableFrom(typeof(T)))
        {
            ISerializable data = (ISerializable)input;
            data.Serialize(this);
        }
#endif
        else
        {
#if UNITY_ENGINE
            Debug.LogError($"invalid type {typeof(T)}");
#endif
        }
    }

#endregion

#region "Read Data"

#if UNITY_ENGINE
    public Vector3 ReadVector3()
	{
		return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
	}
	public Quaternion ReadQuaternion()
	{
		return new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
	}
#endif

	public string ReadString(bool Peek = true)
	{
		int len = ReadInteger(true);
		if (buffUpdate)
		{
			readBuff = Buff.ToArray();
			buffUpdate = false;
		}

		string ret = Encoding.ASCII.GetString(readBuff, readpos, len);
		if (Peek & Buff.Count > readpos)
		{
			if (ret.Length > 0)
			{
				readpos += len;
			}
		}
		return ret;
    }

    public string ReadStringUnicode(bool Peek = true)
    {
        int len = ReadInteger(true);
        if (buffUpdate)
        {
            readBuff = Buff.ToArray();
            buffUpdate = false;
        }

        string ret = Encoding.Unicode.GetString(readBuff, readpos, len);
        if (Peek & Buff.Count > readpos)
        {
            if (ret.Length > 0)
            {
                readpos += len;
            }
        }
        return ret;
    }

    public byte ReadByte(bool Peek = true)
	{
		if (Buff.Count > readpos)
		{
			if (buffUpdate)
			{
				readBuff = Buff.ToArray();
				buffUpdate = false;
			}

			byte ret = readBuff[readpos];
			if (Peek & Buff.Count > readpos)
			{
				readpos += 1;
			}
			return ret;
		}

		else
		{
			throw new Exception("Byte Buffer Past Limit!");
		}
	}

	public byte[] ReadBytes(int Length, bool Peek = true)
	{
		if (buffUpdate)
		{
			readBuff = Buff.ToArray();
			buffUpdate = false;
		}

		byte[] ret = Buff.GetRange(readpos, Length).ToArray();
		if (Peek)
		{
			readpos += Length;
		}
		return ret;
	}

	public bool ReadBool(bool Peek = true)
	{
		if (Buff.Count > readpos)
		{
			if (buffUpdate)
			{
				readBuff = Buff.ToArray();
				buffUpdate = false;
			}

			bool ret = BitConverter.ToBoolean(readBuff, readpos);
			if (Peek & Buff.Count > readpos)
			{
				readpos += 1;
			}
			return ret;
		}

		else
		{
			throw new Exception("Byte Buffer is Past its Limit!");
		}
	}

	public float ReadFloat(bool Peek = true)
	{
		if (Buff.Count > readpos)
		{
			if (buffUpdate)
			{
				readBuff = Buff.ToArray();
				buffUpdate = false;
			}

			float ret = BitConverter.ToSingle(readBuff, readpos);
			if (Peek & Buff.Count > readpos)
			{
				readpos += 4;
			}
			return ret;
		}

		else
		{
			throw new Exception("Byte Buffer is Past its Limit!");
		}
	}

	public double ReadDouble(bool Peek = true)
    {
        if (Buff.Count > readpos)
        {
            if (buffUpdate)
            {
                readBuff = Buff.ToArray();
                buffUpdate = false;
            }
            double ret = BitConverter.ToDouble(readBuff, readpos);
            if (Peek & Buff.Count > readpos)
            {
                readpos += 8;
            }
            return ret;
        }
        else
        {
            throw new Exception("Byte Buffer is Past its Limit!");
        }
    }
    public long ReadLong(bool Peek = true)
	{
		if (Buff.Count > readpos)
		{
			if (buffUpdate)
			{
				readBuff = Buff.ToArray();
				buffUpdate = false;
			}

			long ret = BitConverter.ToInt64(readBuff, readpos);
			if (Peek & Buff.Count > readpos)
			{
				readpos += 4;
			}
			return ret;
		}

		else
		{
			throw new Exception("Byte Buffer is Past its Limit!");
		}
	}

	public int ReadShort(bool Peek = true)
    {
        if (Buff.Count > readpos)
        {
            if (buffUpdate)
            {
                readBuff = Buff.ToArray();
                buffUpdate = false;
            }
            short ret = BitConverter.ToInt16(readBuff, readpos);
            if (Peek & Buff.Count > readpos)
            {
                readpos += 2;
            }
            return ret;
        }
        else
        {
            throw new Exception("Byte Buffer is Past its Limit!");
        }
    }
    public int ReadInteger(bool Peek = true)
	{
		if (Buff.Count > readpos)
		{
			if (buffUpdate)
			{
				readBuff = Buff.ToArray();
				buffUpdate = false;
			}

			int ret = BitConverter.ToInt32(readBuff, readpos);
			if (Peek & Buff.Count > readpos)
			{
				readpos += 4;
			}
			return ret;
		}

		else
		{
			throw new Exception("Byte Buffer is Past its Limit!");
		}
	}
	public void ReadDictionary<K, V>(ref Dictionary<K, V> _refDict)
		where K : new()
		where V : new()
    {
		_refDict = new Dictionary<K, V>();

        int dict_size = ReadInteger();
		for (int i = 0; i < dict_size; i++)
		{
			K key = (K)ReadCustom<K>();
            V value = (V)ReadCustom<V>();
			_refDict.Add(key, value);
		}
    }
    public void ReadList<T>(ref List<T> _refList)
        where T : new()
    {
		_refList = new List<T>();
        int dict_size = ReadInteger();
        for (int i = 0; i < dict_size; i++)
        {
            T d = (T)ReadCustom<T>();
            _refList.Add(d);
        }
    }

	public object ReadCustom<T>() 
	{
		var type = typeof(T);
		if (type == typeof(int))
		{
			return ReadInteger();
		}
		else if (type == typeof(string))
		{
			return ReadString();
        }
#if UNITY_ENGINE
		else if (type == typeof(Vector3))
		{
			return ReadVector3();
		}
#endif
		else if (type == typeof(float))
		{
			return ReadFloat();
		}
		else if (type == typeof(bool))
		{
			return ReadBool();
		}
		else if (type == typeof(byte))
		{
			return ReadByte();
		}
		else if (type == typeof(long))
		{
			return ReadLong();
		}
		else if (type.IsEnum)
		{
			return Enum.ToObject(type, ReadInteger());
		}
		else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
		{
			Type keyType = type.GetGenericArguments()[0];
			Type valueType = type.GetGenericArguments()[1];


			var dictionary = (IDictionary)Activator.CreateInstance(type);


			int dict_size = ReadInteger();
			for (int i = 0; i < dict_size; i++)
			{
				// Read the key using reflection
				MethodInfo readKeyMethod = typeof(ByteBuffer) // Replace 'ThisClassName' with the actual class name
					.GetMethod(nameof(ReadCustom))
					.MakeGenericMethod(keyType);
				object key = readKeyMethod.Invoke(this, null);

				// Read the value using reflection
				MethodInfo readValueMethod = typeof(ByteBuffer) // Replace 'ThisClassName' with the actual class name
					.GetMethod(nameof(ReadCustom))
					.MakeGenericMethod(valueType);
				object value = readValueMethod.Invoke(this, null);

				dictionary.Add(key, value);
			}
			return dictionary;

        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type valueType = type.GetGenericArguments()[0];


            var list = (IList)Activator.CreateInstance(type);


            int dict_size = ReadInteger();
            for (int i = 0; i < dict_size; i++)
            {
                // Read the value using reflection
                MethodInfo readValueMethod = typeof(ByteBuffer) // Replace 'ThisClassName' with the actual class name
                    .GetMethod(nameof(ReadCustom))
                    .MakeGenericMethod(valueType);
                object value = readValueMethod.Invoke(this, null);

                list.Add(value);
            }
            return list;

        }
#if UNITY_ENGINE
        else if (typeof(ISerializable).IsAssignableFrom(type))
		{/*
			ISerializable data;

			MethodInfo getNewInstanceMethod = typeof(T).GetMethod("GetNewInstance", BindingFlags.Static | BindingFlags.Public);
			if (getNewInstanceMethod != null)
			{
				object[] parameters = { this };
				data = (ISerializable)getNewInstanceMethod.Invoke(null, parameters);
			}
			else
			{
				// 정적 메서드가 없으면 기본 생성자로 객체 생성
				data = (ISerializable)Activator.CreateInstance(typeof(T));
				data.Deserialize(this);
			}

			return (T)data;
			*/
			ISerializable data;
			data = (ISerializable)Activator.CreateInstance(typeof(T));
			data.Deserialize(this);
			return (T)data;
		}
		else if (typeof(ISerializableBase).IsAssignableFrom(type))
		{
			MethodInfo createMethod;
			if (!createMethodCache.TryGetValue(typeof(T), out createMethod))
			{
				createMethod = typeof(T).GetMethod("CreateInstance", BindingFlags.Public | BindingFlags.Static);
				createMethodCache[typeof(T)] = createMethod;
			}

			ISerializableBase data = (ISerializableBase)createMethod.Invoke(null, new object[] { this });
			data.Deserialize(this);
			return (T)data;
		}
#endif
		else
        {
#if UNITY_ENGINE
            Debug.LogError($"invalid type {typeof(T)}");
#endif
            return null;
		}
	}
#endregion

    private bool disposedValue = false;

	//IDisposable
	protected virtual void Dispose(bool disposing)
	{
		if (!this.disposedValue)
		{
			if (disposing)
			{
				Buff.Clear();
			}

			readpos = 0;
		}
		this.disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	public void Init()
	{
		Clear();
	}
}