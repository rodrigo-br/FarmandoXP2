using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioManager : SingletonBase<AudioManager>
{
    private AudioSource audioSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource cheerSource;
    [SerializeField] private AudioClip cheerLowClip;
    [SerializeField] private AudioClip bombAdjacent;
    [SerializeField] private AudioClip bombThunder;
    [SerializeField] private AudioClip mouseClick; // ???
    [SerializeField] private AudioClip hoverMouseGem;
    [SerializeField] private AudioClip victoryScreen;
    [SerializeField] private AudioClip bombSimple;
    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider sfxVolume;
    [SerializeField] private GameObject canvasModal;
    private EventTrigger eventTrigger;

    public override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        musicVolume.value = audioSource.volume;
        audioSource.ignoreListenerPause = true;
        sfxSource.ignoreListenerPause = true;
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
    }

    public void Start()
    {
        musicVolume.onValueChanged.AddListener(_ => 
        {
            audioSource.volume = musicVolume.value;
            cheerSource.volume = musicVolume.value;
        });
        eventTrigger = sfxVolume.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => { OnSfxSliderPointerUp(); });
        eventTrigger.triggers.Add(entry);
    }

    void OnSfxSliderPointerUp()
    {
        PlaySFX(hoverMouseGem);
    }

    public void PlaySFX(AudioClip sfx)
    {
        sfxSource.PlayOneShot(sfx, sfxVolume.value);
    }

    private void PlayMusic(AudioClip music)
    {
        audioSource.Stop();
        audioSource.clip = music;
        audioSource.Play();
    }

    public void PlayBombAdjacent()
    {
        PlaySFX(bombAdjacent);
    }

    public void PlayBombThunder()
    {
        PlaySFX(bombThunder);
    }

    public void PlayMouseClick()
    {
        PlaySFX(mouseClick);
    }

    public void PlayHoverMouseGem()
    {
        PlaySFX(hoverMouseGem);
    }

    public void PlayBombSimple()
    {
        PlaySFX(bombSimple);
    }

    public void PlayVictoryScreenMusic()
    {
        PlayMusic(victoryScreen);
    }

    public void PlayMainMusic()
    {
        PlayMusic(mainMusic);
    }

    public void CloseVolumeModal()
    {
        canvasModal.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenVolumeModal()
    {
        Time.timeScale = 0;
        canvasModal.SetActive(true);
    }

    public void PlayCheerSound()
    {
        cheerSource.Play();
    }

    public void StopCheerSound()
    {
        cheerSource.Stop();
    }
}
