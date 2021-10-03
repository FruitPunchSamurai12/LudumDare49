using UnityEngine;
using System;

public class Pickable:MonoBehaviour
{
    public bool IsPickable { get; private set; }
    [SerializeField] Rigidbody _rb;
    [SerializeField] Transform _rootObject;
    public event Action onTooBigToCarry;

    public void Pick(Transform parent)
    {
        if (IsPickable)
        {
            _rootObject.parent = parent;
            _rootObject.localPosition = Vector3.zero;
            _rb.isKinematic = true;
        }
    }


    public void LetDown()
    {
        _rb.isKinematic = false;
        _rootObject.parent = null;
    }

    public void Throw(Vector3 force)
    {
        LetDown();
        _rb.AddForce(force, ForceMode.Impulse);
    }

    public void SmallEnoughToPick()
    {
        IsPickable = true;
    }

    public void TooBigToCarry()
    {
        IsPickable = false;
        onTooBigToCarry?.Invoke();
    }

}
