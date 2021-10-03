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
    [SerializeField] AudioSource _shrinkGunAudioSource;
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
            _shrinkGunAudioSource.time = 0;
            _shrinkGunAudioSource.Play();
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
                _lineRenderer.SetPosition(2, _rayObj.transform.InverseTransformPoint(info.point));
                var shrinkable = info.collider.GetComponent<Shrinkable>();
                if(info.collider.CompareTag("Mirror"))
                {
                    Vector3 reflect = Vector3.Reflect(direction, info.normal);
                    if (Physics.Raycast(info.point, reflect, out RaycastHit info2, _range, _layerMask))
                    {
                        shrinkable = info2.collider.GetComponent<Shrinkable>();
                        _lineRenderer.SetPosition(2, _rayObj.transform.InverseTransformPoint(info2.point));
                    }
                    else
                        _lineRenderer.SetPosition(2, _rayObj.transform.InverseTransformPoint(info.point + reflect * _range));
                }
                if(shrinkable!=null)
                {
                    if(_currentShrinkable!=null && _currentShrinkable !=shrinkable)
                    {
                        _currentShrinkable.StopShrink();
                    }
                    _currentShrinkable = shrinkable;
                    //shrinkable.Shrink(_shrinkMat, _shrinkRate);
                    shrinkable.Shrink(info.point);
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
                _shrinkGunAudioSource.Stop();
                _rayObj.SetActive(false);
            }
        }
    }

}

