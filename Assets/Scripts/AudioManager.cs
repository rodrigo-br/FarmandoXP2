using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private AudioClip[] tutorialPageVoice;
    [SerializeField] private GameObject cardCanvas;
    [SerializeField] private Image cardBackground;
    [SerializeField] private TextMeshProUGUI cardTitle;
    [SerializeField]private TextMeshProUGUI cardSubtitle;
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

    public void PlayTutorialPageVoice(int page)
    {
        sfxSource.Stop();
        sfxSource.clip = tutorialPageVoice[page - 1];
        sfxSource.Play();
    }

    public void OpenCardCanvas(Sprite runnerSprite, int index)
    {
        if (Time.timeScale == 0) { return; }
        Time.timeScale = 0;
        string[,] texts = new string[,]
        {
            { "Martina Sandlers", "Habilidade: Explode uma coluna" },
            {"Luna Swiftwind", "Habilidade: Explode uma linha"},
            {"Aria Veloce", "Habilidade: Ganha mais tempo"},
            {"Nova Fleetfoot", "Habilidade: Planta duas bombas"},
            {"Zara Blitz", "Habilidade: Encoraja suas aliadas"}
        };
        canvasModal.SetActive(true);
        cardCanvas.SetActive(true);
        cardBackground.sprite = runnerSprite;
        cardTitle.text = texts[index, 0];
        cardSubtitle.text = texts[index, 1];
    }

    public void CloseCardCanvas()
    {
        if (Time.timeScale == 1) { return; }
        cardCanvas.SetActive(false);
        canvasModal.SetActive(false);
        Time.timeScale = 1;
    }
}
