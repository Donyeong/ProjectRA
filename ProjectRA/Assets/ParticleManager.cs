using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum eParticleType
{
	PropDamage,
	PropBreak,
}

public class ParticleManager : SingletonMono<ParticleManager>
{

	GameObject[] mParticles;
	string mResourcePath = "ParticlePrefab/";

	public GameObject PlayParticle(eParticleType playType, Vector3 position, Quaternion rotation)
	{
		return Instantiate(mParticles[(int)playType], position, rotation);
	}

	public GameObject PlayParticle(eParticleType playType, Transform transform)
	{
		return Instantiate(mParticles[(int)playType], transform);
	}

	protected override void Awake()
	{
		base.Awake();
		LoadParticleResources();
	}


	void LoadParticleResources()
	{
		string[] particleResourcesNames = Enum.GetNames(typeof(eParticleType));
		mParticles = new GameObject[particleResourcesNames.Length];

		for (int i = 0; i < particleResourcesNames.Length; i++)
		{
			string path = mResourcePath + particleResourcesNames[i];
			mParticles[i] = Resources.Load(path, typeof(GameObject)) as GameObject;

			//eParticleType 열거형의 멤버와 동일한 이름의 프리팹이 ParticlePrefab폴더에 있어야 합니다.
			Debug.Assert(mParticles[i] != null, "파티클 리소스를 찾을 수 없습니다");
		}
	}
}

