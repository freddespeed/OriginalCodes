using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Hits
{
    public readonly WORLDOBJECTID firstHit;
    public readonly WORLDOBJECTID firstHit2;
    public readonly WORLDOBJECTID secondHit;

    public Hits(WORLDOBJECTID firstHit, WORLDOBJECTID firstHit2, WORLDOBJECTID secondHit)
    {
        this.firstHit = firstHit;
        this.firstHit2 = firstHit2;
        this.secondHit = secondHit;
    }

    public bool Contains(WORLDOBJECTID hit)
    {
        return firstHit == hit || firstHit2 == hit || secondHit == hit;
    }
}
