using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public readonly partial struct ParticlesCacheAspect : IAspect
{
    public readonly Entity mEntity;
    
    private readonly RefRO<ParticlesCache> m_particlesCache;

    public Entity TinyExplosion => m_particlesCache.ValueRO.mTinyExplosionParticle;

    public float TinyExplosionLifetime => m_particlesCache.ValueRO.mTinyExplosionLifetime;
}
