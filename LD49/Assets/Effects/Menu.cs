using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    CanvasGroup _canvasGroup;
    [SerializeField] Slider _musicSider;
    [SerializeField] Slider _soundSlider;
    [SerializeField] TextMeshProUGUI _playText;
    [SerializeField] Button _playButton;


    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _playButton.onClick.AddListener(OnClickPlay);
    }


    void Start()
    {
        _musicSider.value = AudioManager.Instance.BGVolume;
        _soundSlider.value = AudioManager.Instance.SFXVolume;
    }

    public void OnClickPlay()
    {
        _canvasGroup.alpha = 0;
        _playText.SetText("Resume");
        _playButton.onClick.RemoveListener(OnClickPlay);
        _playButton.onClick.AddListener(Resume);
        GameManager.Instance.Play();
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnMusicVolumeChange(float value)
    {
        AudioManager.Instance.ChangeBGVolume(value);
    }

    public void OnSoundVolumeChange(float value)
    {
        AudioManager.Instance.ChangeSFXVolume(value);
    }

    public void Pause()
    {
        GameManager.Instance.Pause();
    }

    public void Resume()
    {
        GameManager.Instance.Resume();
    }

    public void SetCanvasGroupAlpha(float value,bool blockRaycasts)
    {
        _canvasGroup.alpha = value;
        _canvasGroup.blocksRaycasts = blockRaycasts;
    }

}
