using Cinemachine;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private Player _player;
    [SerializeField] Transform _aimer;
    [SerializeField] float _rotationSpeed = 300f;
    [SerializeField] CharacterDisplay _characterDisplay;
    private void Start()
    {
        _player = GetComponent<Player>();
    }


    void LateUpdate()
    {
            var bodyRotation = new Vector3(0, PlayerInput.Instance.MouseX * _rotationSpeed * Time.deltaTime, 0);
            _player.transform.Rotate(bodyRotation);
            var headRotation = new Vector3(-PlayerInput.Instance.MouseY * _rotationSpeed * Time.deltaTime, 0, 0);
            _aimer.transform.Rotate(headRotation);
            Vector3 aimerEuler = _aimer.localEulerAngles;
            float angle = aimerEuler.x;
            if (angle > 180 && angle < 345)
                aimerEuler.x = 345;
            else if (angle < 180 && angle > 40)
                aimerEuler.x = 40;
            _aimer.localEulerAngles = aimerEuler;
        _characterDisplay.angleLookUp = -aimerEuler.x;
        if(Input.GetKeyDown(KeyCode.Q))
        {
            _aimer.rotation = _player.transform.rotation;
        }
    }
}