using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct OrbitSatellites : IComponentData
{
    public BlobAssetReference<OrbitSatellitesBlob> mOrbitSatellitesBlob;
    public int mOrbitSatellitesCount;
}

public struct OrbitSatellitesBlob
{
    public BlobArray<Entity> mOrbitSatellites;
}