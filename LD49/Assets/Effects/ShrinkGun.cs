using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkGun : MonoBehaviour
{
    Camera _cam;
    [SerializeField] GameObject rayObj;
    [SerializeField] float depth = -100;
    bool firing = false;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        if(PlayerInput.Instance.Fire)
        {
            Vector3 target = _cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, -depth));
            rayObj.transform.LookAt(target);
            rayObj.SetActive(true);
            firing = true;
        }
        if(firing)
        {
            Vector3 target = _cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, -depth));
            rayObj.transform.LookAt(target);
            if (PlayerInput.Instance.StopFire)
            {
                firing = false;
                rayObj.SetActive(false);
            }
        }
    }
}
