using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator _animator;
    [SerializeField] bool _startOpen;
    [SerializeField] float _waitForSecondsToPlayCloseSound = 0.3f;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_startOpen)
            Open();
    }

    public void Close()
    {
        _animator.SetBool("Open", false);
        StopCoroutine(PlayCloseSound());
        StartCoroutine(PlayCloseSound());
        
    }

    public void Open()
    {
        _animator.SetBool("Open", true);
        StopCoroutine(PlayCloseSound());
        AudioManager.Instance.PlaySoundEffect("DoorOpen", transform.position);
    }

    IEnumerator PlayCloseSound()
    {
        yield return new WaitForSeconds(_waitForSecondsToPlayCloseSound);
        AudioManager.Instance.PlaySoundEffect("DoorClose", transform.position);
    }
}
