using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[InternalBufferCapacity(16)]
public struct ScoreDataElement : IBufferElementData
{
    public static implicit operator ScoreDataElement(int score)
    {
        return new ScoreDataElement { mScoreIncrement = score };
    }

    public int mScoreIncrement;
}
