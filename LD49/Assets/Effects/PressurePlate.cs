using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public UnityEvent onApplyPressure;
    public UnityEvent onReleasePressure;
    Animator _animator;
    bool press = false;
    [SerializeField] float secondsToCallEvents = 0.4f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Shrinkable p = other.GetComponent<Shrinkable>();
        if(p!=null)
        {
            if (!p.ApplyPressure()) return;
        }
        Press();
    }

    private void OnTriggerStay(Collider other)
    {
        Shrinkable p = other.GetComponent<Shrinkable>();
        if (p != null)
        {
            if (!p.ApplyPressure())
            {
                UnPress();
                return;
            }
        }
        Press();
    }

    private void OnTriggerExit(Collider other)
    {
        Shrinkable p = other.GetComponent<Shrinkable>();
        if (p != null)
        {
            if (!p.ApplyPressure()) return;
        }
        UnPress();
    }

    void Press()
    {
        if (press) return;
        press = true;
        StopAllCoroutines();
        StartCoroutine(CallEvent(onApplyPressure));
        _animator.SetBool("Press", true);
    }

    void UnPress()
    {
        if (!press) return;
        press = false;
        StopAllCoroutines();
        StartCoroutine(CallEvent(onReleasePressure));
        _animator.SetBool("Press", false);
    }

    public void PlayerOnTop()
    {
        Press();
    }

    public void PlayerLeft()
    {
        UnPress();
    }

    IEnumerator CallEvent(UnityEvent unityEvent)
    {
        yield return new WaitForSeconds(secondsToCallEvents);
        unityEvent?.Invoke();
    }

}
