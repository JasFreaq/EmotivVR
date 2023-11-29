using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct ParticlesCache : IComponentData
{
    public Entity mTinyExplosionParticle;

    public float mTinyExplosionLifetime;
}
