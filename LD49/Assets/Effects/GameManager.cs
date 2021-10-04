using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    Player _player;
    Rotator _rotator;
    Menu _menu;
    bool _paused;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
            _player = FindObjectOfType<Player>();
            _rotator = _player.GetComponent<Rotator>();
            _player.enabled = false;
            _rotator.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    public void Play()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _player.enabled = true;
        _rotator.enabled = true;
    }

    private void Update()
    {
        if (_menu == null)
                _menu = FindObjectOfType<Menu>();
        if(PlayerInput.Instance.PausePressed)
        {           
            if(_paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        if (_menu != null)
            _menu.SetCanvasGroupAlpha(1,true);
        _paused = true;
        _player.enabled = false;
        _rotator.enabled = false;
    }

    public void Resume()
    {
        if (_menu != null)
            _menu.SetCanvasGroupAlpha(0,false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        _paused = false;
        _player.enabled = true;
        _rotator.enabled = true;
    }
}