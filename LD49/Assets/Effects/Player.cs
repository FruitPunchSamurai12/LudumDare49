using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    [SerializeField] CharacterDisplay _characterDisplay;
    CharacterController _characterController;
    CharacterGrounding _characterGrounding;
    [SerializeField] ShrinkAnimation _shrinkAnimation;
    [SerializeField] Transform _carrier;

    [SerializeField] float _defaultHeight = 1;
    [SerializeField] float _defaultRadius = .25f;
    [SerializeField] float _defaultCenterY = .5f;
    [SerializeField] float _defaultSpeed;
    [SerializeField] float _defaultJumpPower;
    [SerializeField] float _shrankHeight = 1f/3f;
    [SerializeField] float _shrankRadius = .25f/3f;
    [SerializeField] float _shrankCenterY = .15f;
    [SerializeField] float _shrankSpeed;
    [SerializeField] float _shrankJumpPower;

    [SerializeField] float _defaultCarrierY = 0.85f;
    [SerializeField] float _shrankCarrierY = 0.4f;

    public float _speed;
    public float _jumpPower = 20;
    bool _groundedPlayer = true;
    Vector3 velocity;
    bool leftStep = true;
    AudioSource _stepsAudioSource;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _characterGrounding = GetComponent<CharacterGrounding>();
        _stepsAudioSource = GetComponent<AudioSource>();
        _shrinkAnimation.onShrinkComplete += HandlePlayerShrink;
        _shrinkAnimation.onShrinkRevert += HandlePlayerUnshrink;
        _speed = _defaultSpeed;
        _jumpPower = _defaultJumpPower;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    // Update is called once per frame
    void Update()
    {
        _groundedPlayer = _characterGrounding.IsGrounded;
        if (_groundedPlayer && velocity.y < 0)
        {
            velocity.y = 0f;
        }


        Vector3 moveInput = new Vector3(PlayerInput.Instance.Horizontal, 0, PlayerInput.Instance.Vertical);
        _characterDisplay.runDirection = new Vector2(moveInput.z, moveInput.x);
        _characterDisplay.running = moveInput != Vector3.zero;
        Vector3 move = transform.rotation * moveInput;
        if (move != Vector3.zero && _groundedPlayer)
            PlaySteps();
        _characterController.Move(move * Time.deltaTime * _speed);


        // Changes the height position of the player..
        if (PlayerInput.Instance.Jump && _groundedPlayer)
        {
            Jump();
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        _characterController.Move(velocity * Time.deltaTime);

        _characterDisplay.inair = !_groundedPlayer;
    }

    void PlaySteps()
    {
        if (_stepsAudioSource.isPlaying)
            return;
        string sound = "Step";
        if (leftStep)
            sound += "Left";
        else
            sound += "Right";
        sound += UnityEngine.Random.Range(1, 4).ToString();
        leftStep = !leftStep;
        _stepsAudioSource.clip = AudioManager.Instance.GetSoundEffect(sound);
        _stepsAudioSource.Play();
    }

    private void Jump()
    {
        velocity.y += Mathf.Sqrt(_jumpPower * -3.0f * Physics.gravity.y);
        AudioManager.Instance.PlaySoundEffect("Jump", transform.position);
    }

    void HandlePlayerShrink()
    {
        _characterController.center = new Vector3(0, _shrankCenterY, 0);
        _characterController.radius = _shrankRadius;
        _characterController.height = _shrankHeight;
        _speed = _shrankSpeed;
        _jumpPower = _shrankJumpPower;
        _carrier.transform.localPosition = new Vector3(_carrier.transform.localPosition.x, _shrankCarrierY, _carrier.transform.localPosition.z);
    }

    void HandlePlayerUnshrink()
    {
        _characterController.center = new Vector3(0, _defaultCenterY, 0);
        _characterController.radius = _defaultRadius;
        _characterController.height = _defaultHeight;
        _speed = _defaultSpeed;
        _jumpPower = _defaultJumpPower;
        _carrier.transform.localPosition = new Vector3(_carrier.transform.localPosition.x, _defaultCarrierY, _carrier.transform.localPosition.z);
    }

}
