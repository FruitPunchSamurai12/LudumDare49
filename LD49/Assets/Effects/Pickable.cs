using UnityEngine;
using System;

public class Pickable:MonoBehaviour
{
    [SerializeField] bool _alwaysPickable;
    public bool _alwaysWorkOnPressurePlates;
    public bool IsPickable { get; private set; }
    [SerializeField] Rigidbody _rb;
    [SerializeField] Transform _rootObject;
    public event Action onTooBigToCarry;

    public Renderer rendererForHighlight;

    RigidbodyConstraints _defaultConstraints;

    private void Awake()
    {
        if (_alwaysPickable)
            IsPickable = true;

        _defaultConstraints = _rb.constraints;
    }

    public void Pick(Transform parent)
    {
        if (IsPickable)
        {
            _rootObject.parent = parent;
            _rootObject.localPosition = Vector3.zero;
            _rb.useGravity = false;
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }


    public void LetDown()
    {
        _rb.useGravity = true;
        _rootObject.parent = null;
        _rb.constraints = _defaultConstraints;
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
        if (_alwaysPickable)
            return;
        IsPickable = false;
        onTooBigToCarry?.Invoke();
    }


    public void Highlight(bool highlight) {
        if(rendererForHighlight != null) {
            rendererForHighlight.material.SetFloat("Highlight", highlight?1:0);
        }
    }
}
