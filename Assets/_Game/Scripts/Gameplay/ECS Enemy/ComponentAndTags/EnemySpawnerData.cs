using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct EnemySpawnerData : IComponentData
{
    public Entity mSpawnedEntity;

    public Random mRandom;
}
