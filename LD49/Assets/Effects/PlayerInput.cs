using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public float Vertical  => Input.GetAxisRaw("Vertical");
    public float Horizontal => Input.GetAxisRaw("Horizontal");

    public float MouseX => Input.GetAxis("Mouse X");
    public float MouseY=> Input.GetAxis("Mouse Y");

    public bool Jump => Input.GetButtonDown("Jump");
    public bool PausePressed => Input.GetKeyDown(KeyCode.Escape);

    public Vector2 MousePosition => Input.mousePosition;

    public bool Fire => Input.GetButtonDown("Fire1");
    public bool StopFire => Input.GetButtonUp("Fire1");
}
