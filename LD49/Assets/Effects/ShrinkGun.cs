using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkGun : MonoBehaviour
{
    Camera _cam;
    [SerializeField] GameObject _rayObj;
    LineRenderer _lineRenderer;
    [SerializeField] float _depth = -100;
    [SerializeField] float _range = 5f;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] float _shrinkRate;
    [SerializeField] Material _shrinkMat;
    public bool Firing { get; private set; }
    Shrinkable _currentShrinkable = null;
    PickUp _pickUp;

    private void Awake()
    {
        _cam = Camera.main;
        _lineRenderer = _rayObj.GetComponent<LineRenderer>();
        _pickUp = GetComponent<PickUp>();
    }

    private void Update()
    {
        if (_pickUp.Picking)
            return;
        if(PlayerInput.Instance.Fire)
        {
            _rayObj.SetActive(true);
            Firing = true;
        }
        if(Firing)
        {
            Vector3 target = _cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, -_depth));
            Vector3 direction = (target - _rayObj.transform.position).normalized;
            if(Physics.Raycast(_rayObj.transform.position,direction,out RaycastHit info, _range,_layerMask))
            {
                _lineRenderer.SetPosition(0, Vector3.zero);
                _lineRenderer.SetPosition(1, _rayObj.transform.InverseTransformPoint(info.point));
                var shrinkable = info.collider.GetComponent<Shrinkable>();
                if(shrinkable!=null)
                {
                    if(_currentShrinkable!=null && _currentShrinkable !=shrinkable)
                    {
                        _currentShrinkable.StopShrink();
                    }
                    _currentShrinkable = shrinkable;
                    shrinkable.Shrink(_shrinkMat, _shrinkRate);
                }
            }
            else
            {
                _lineRenderer.SetPosition(0, Vector3.zero);
                _lineRenderer.SetPosition(1, _rayObj.transform.InverseTransformPoint(_rayObj.transform.position +direction *_range));
            }
            
            if (PlayerInput.Instance.StopFire)
            {
                if(_currentShrinkable!=null)
                {
                    _currentShrinkable.StopShrink();
                    _currentShrinkable = null;
                }
                Firing = false;
                _rayObj.SetActive(false);
            }
        }
    }
}
