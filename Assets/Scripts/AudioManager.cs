using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonBase<AudioManager>
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip cheerLowClip;
    [SerializeField] private AudioClip bombAdjacent;
    [SerializeField] private AudioClip bombThunder;
    [SerializeField] private AudioClip mouseClick; // ???
    [SerializeField] private AudioClip hoverMouseGem;
    [SerializeField] private AudioClip victoryScreen;
    [SerializeField] private AudioClip bombSimple;
    [SerializeField] private AudioClip mainMusic;
    private float sfxVolume = 1.0f;

    public override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCheerLow()
    {
        PlayMusic(cheerLowClip);
    }

    private void PlaySFX(AudioClip sfx)
    {
        AudioSource.PlayClipAtPoint(sfx, Camera.main.transform.position, sfxVolume);
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
}
