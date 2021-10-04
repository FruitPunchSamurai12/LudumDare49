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
    [SerializeField] LayerMask _triggerBoxLayer;



    private void Awake()
    {
        _pickable = GetComponent<Pickable>();
        _shrinkAnimation.onShrinkComplete += HandleShrinkComplete;
        _shrinkAnimation.onShrinkRevert += HandleShrinkRevert;
    }

    private void FixedUpdate()
    {
        if(_isSmall)
        {
            var cols = Physics.OverlapSphere(transform.position, 1,_triggerBoxLayer,QueryTriggerInteraction.Collide);
            if (cols.Length > 0)
                InSmallSpace();
            else
                InBigSpace();
        }
    }

    public void InSmallSpace()
    {
        _shrinkAnimation.InSmallSpace();
    }

    public void InBigSpace()
    {
        _shrinkAnimation.LeftSmallSpace();
    }

    public virtual void Shrink(Vector3 hitPoint)
    {
        _shrinkAnimation.chargeHitPosition = hitPoint;
        _shrinkAnimation.charging = true;
    }
    public virtual void StopShrink()
    {
        _shrinkAnimation.charging = false;
    }

    protected virtual void HandleShrinkComplete()
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
