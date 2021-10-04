using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHitPlanet : Shrinkable
{
    

    public override void Shrink(Vector3 hitPoint)
    {
        base.Shrink(hitPoint);
    }
    
    public override void StopShrink()
    {
        base.StopShrink();
    }

    protected override void HandleShrinkComplete()
    {
        base.HandleShrinkComplete();
    }
}
