using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class Temp : MonoBehaviour
{
    public Entity managedEntity;

    void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(typeof(LocalTransform));
        Entity entity = entityManager.CreateEntity(entityArchetype);
        managedEntity = entityManager.Instantiate(entity);
    }
}

//public class TempBaker : Baker<Temp>
//{
//    public override void Bake(Temp authoring)
//    {
//        authoring.entityPrefab = GetEntity(authoring.testPrefab, TransformUsageFlags.Dynamic);
//    }
//}

public struct TempData : IComponentData
{

}
