using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
