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

    private void Awake()
    {
        _cam = Camera.main;
        _shrinkGun = GetComponent<ShrinkGun>();
    }

    private void Update()
    {
        if (_shrinkGun.Firing)
            return;
        if(!Picking && PlayerInput.Instance.Pick)
        {
            Vector3 target = _cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, -_depth));
            Vector3 direction = (target - _rayOrigin.position).normalized;
            var hits = Physics.SphereCastAll(_rayOrigin.position, _thickness, direction, _range, _layerMask);
            foreach (var info in hits)
            {
                var pickable = info.collider.GetComponent<Pickable>();
                if (pickable != null && pickable.IsPickable)
                {
                    _currentPickable = pickable;
                    _currentPickable.Pick(_carryPoint);
                    AudioManager.Instance.PlaySoundEffect("Pick", transform.position);
                    _currentPickable.onTooBigToCarry += Drop;
                    Picking = true;
                    break;
                }
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
