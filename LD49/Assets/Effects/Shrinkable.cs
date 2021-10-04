using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrinkable : MonoBehaviour
{
    // Copy of the object with the ghost shader on it!
    [SerializeField] ShrinkAnimation _shrinkAnimation;
    Pickable _pickable;
    bool _isSmall = false;
    [SerializeField] bool _alwaysWorkOnPressurePlates = false;


    private void Awake()
    {
        _pickable = GetComponent<Pickable>();
        _shrinkAnimation.onShrinkComplete += HandleShrinkComplete;
        _shrinkAnimation.onShrinkRevert += HandleShrinkRevert;
    }

    public void Shrink(Vector3 hitPoint)
    {
        _shrinkAnimation.chargeHitPosition = hitPoint;
        _shrinkAnimation.charging = true;
    }
    public void StopShrink()
    {
        _shrinkAnimation.charging = false;
    }

    void HandleShrinkComplete()
    {
        _isSmall = true;
        if (_pickable != null)
            _pickable.SmallEnoughToPick();
    }

    void HandleShrinkRevert()
    {
        _isSmall = false;
        if (_pickable != null)
            _pickable.TooBigToCarry();
    }

    public bool ApplyPressure()
    {
        if (_alwaysWorkOnPressurePlates) return true;
        return !_isSmall;
    }
}
