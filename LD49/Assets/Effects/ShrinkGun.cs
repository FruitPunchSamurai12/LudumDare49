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
    bool _firing = false;
    Shrinkable _currentShrinkable = null;


    private void Awake()
    {
        _cam = Camera.main;
        _lineRenderer = _rayObj.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(PlayerInput.Instance.Fire)
        {
            _rayObj.SetActive(true);
            _firing = true;
        }
        if(_firing)
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
                Debug.Log(info.collider.name);
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
                _firing = false;
                _rayObj.SetActive(false);
            }
        }
    }
}
