using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrinkable : MonoBehaviour
{
    // Copy of the object with the ghost shader on it!
    public GameObject _expandGraphics;
    [SerializeField] Transform _parentPivot;
    [SerializeField] float _revertingThreshold = 2f;
    [SerializeField] float _revertingDuration = 2f;
    [SerializeField] Vector3 _minimumScale = new Vector3(.3f, .3f, .3f);
    [SerializeField] Vector3 _maxScaleToBePickable = new Vector3(.4f, .4f, .4f);
    bool _reverting = false;
    float _revertingTime = 0;
    MeshRenderer _rend;
    Material _defaultMat;
    Vector3 _startRevertingScale;
    Pickable _pickable;

    private void Awake()
    {
        _expandGraphics.SetActive(false);
        _rend = GetComponent<MeshRenderer>();
        _defaultMat = _rend.material;
        _pickable = GetComponent<Pickable>();
    }

    void Update()
    {
        if(_reverting)
        {
            _revertingTime += Time.deltaTime;
            if(_revertingTime>=_revertingThreshold)
            {
                float percentage = (_revertingTime - _revertingThreshold) / _revertingDuration;
                _parentPivot.localScale = Vector3.Lerp(_minimumScale, Vector3.one, percentage);
                if (_pickable != null && _parentPivot.transform.localScale.sqrMagnitude > _maxScaleToBePickable.sqrMagnitude)
                    _pickable.TooBigToCarry();
                if (_revertingTime>=(_revertingThreshold+_revertingDuration))
                {
                    _reverting = false;
                    _rend.material = _defaultMat;
                }
            }
        }
    }

    public void Shrink(Material shrinkMat,float shrinkRate)
    {
        if (_reverting)
             _reverting = false;
        _expandGraphics.SetActive(true);
        _rend.material = shrinkMat;
        _parentPivot.transform.localScale = Vector3.MoveTowards(_parentPivot.transform.localScale, _minimumScale, shrinkRate);
        if (_pickable != null && _parentPivot.transform.localScale.sqrMagnitude < _maxScaleToBePickable.sqrMagnitude)
            _pickable.SmallEnoughToPick();
    }

    public void StopShrink()
    {
        _reverting = true;
        _revertingTime = 0;
    }
}
