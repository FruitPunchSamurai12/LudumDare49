using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] Transform _carryPoint;
    [SerializeField] Transform _rayOrigin;
    [SerializeField] float _depth = -2;
    [SerializeField] float _range = 2f;
    [SerializeField] float _thickness = 2f;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] float _throwForce = 50f;
    public bool Picking { get; private set; }

    Camera _cam;
    ShrinkGun _shrinkGun;

    Pickable _currentPickable = null;

    Pickable _hoverPickable = null;
    Interactable _hoverInteractible = null;

    private void Awake()
    {
        _cam = Camera.main;
        _shrinkGun = GetComponent<ShrinkGun>();
    }

    private void Update()
    {
        Ray screenCenterRay = _cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, _depth));
        var hits = Physics.OverlapSphere(transform.position + transform.forward*_range, _thickness,_layerMask);

        // Find things for hovering
        Pickable newHoverPickable = null;
        Interactable newHoverInteractible = null;

        if(!Picking && !_shrinkGun.Firing) {
            foreach (var info in hits)
            {
                var interactable = info.GetComponent<Interactable>();
                if(interactable!=null)
                {
                    newHoverInteractible = interactable;
                    break;
                }
                var pickable = info.GetComponent<Pickable>();
                if (pickable != null && pickable.IsPickable)
                {
                    newHoverPickable = pickable;
                    break;
                }
            }
        }

        // Update hover indication
        if(newHoverInteractible != _hoverInteractible) {
            if(_hoverInteractible != null) {
                _hoverInteractible.Highlight(false);
            }
            if(newHoverInteractible!= null) {
                newHoverInteractible.Highlight(true);
            }
            _hoverInteractible = newHoverInteractible;
        }
        if(newHoverPickable != _hoverPickable) {
            if(_hoverPickable != null) {
                _hoverPickable.Highlight(false);
            }
            if(newHoverPickable != null) {
                newHoverPickable.Highlight(true);
            }
            _hoverPickable = newHoverPickable;
        }
        

        //
        // Actual picking
        //
        if (_shrinkGun.Firing)
            return;
        if(!Picking && PlayerInput.Instance.Pick)
        {
            if(newHoverInteractible != null) {
                newHoverInteractible.Interact();
            }
            if (newHoverPickable != null)
            {
                _currentPickable = newHoverPickable;
                _currentPickable.Pick(_carryPoint);
                AudioManager.Instance.PlaySoundEffect("Pick", transform.position);
                _currentPickable.onTooBigToCarry += Drop;
                Picking = true;
            }
        }
        if(Picking && PlayerInput.Instance.LetDown)
        {
            if(_currentPickable!=null)
            {
                _currentPickable.Throw(_cam.transform.forward*_throwForce);
                AudioManager.Instance.PlaySoundEffect("Throw", transform.position);
            }
            _currentPickable.onTooBigToCarry -= Drop;
            _currentPickable = null;
            Picking = false;
        }
    }

    void Drop()
    {
        _currentPickable.LetDown();
        _currentPickable.onTooBigToCarry -= Drop;
        _currentPickable = null;
        Picking = false;
    }
}
