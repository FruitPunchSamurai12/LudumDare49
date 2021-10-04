using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator _animator;
    [SerializeField] bool _startOpen;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_startOpen)
            Open();
    }

    public void Close()
    {
        _animator.SetBool("Open", false);
    }

    public void Open()
    {
        _animator.SetBool("Open", true);
    }
}
