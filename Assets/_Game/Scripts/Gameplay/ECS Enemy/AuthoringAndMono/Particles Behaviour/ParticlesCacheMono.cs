using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ParticlesCacheMono : MonoBehaviour
{
    [SerializeField] public ParticleSystem m_tinyExplosionParticle;

    [SerializeField] public float m_tinyExplosionLifetime = 1f;

    public GameObject TinyExplosionParticle => m_tinyExplosionParticle.gameObject;

    public float TinyExplosionLifetime => m_tinyExplosionLifetime;
}

public class ParticlesCacheBaker : Baker<ParticlesCacheMono>
{
    public override void Bake(ParticlesCacheMono authoring)
    {
        Entity particlesCacheEntity = GetEntity(TransformUsageFlags.None);

        AddComponent(particlesCacheEntity, new ParticlesCache
        {
            mTinyExplosionParticle = GetEntity(authoring.TinyExplosionParticle, TransformUsageFlags.Dynamic),
            mTinyExplosionLifetime = authoring.TinyExplosionLifetime
        });
    }
}
