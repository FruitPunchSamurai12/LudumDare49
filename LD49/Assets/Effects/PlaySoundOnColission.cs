using UnityEngine;

public class PlaySoundOnColission:MonoBehaviour
{
    Rigidbody _rb;
    [SerializeField] string[] sounds;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_rb.velocity.magnitude>1)
        {
            int index = Random.Range(0, sounds.Length);
            AudioManager.Instance.PlaySoundEffect(sounds[index], transform.position);
        }

    }
}