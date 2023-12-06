using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct PlayerStateData : IComponentData
{
    public bool mIsGamePaused;

    public int mPlayerHealth;
}
