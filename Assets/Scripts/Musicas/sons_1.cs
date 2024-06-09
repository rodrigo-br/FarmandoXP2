using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class sons_1 : SingletonBase<sons_1>
{
    [SerializeField] AudioClip enemyShootClip;
    [SerializeField] AudioClip playerShootClip;
    [SerializeField] AudioClip[] enemySounds;
    [SerializeField] AudioClip[] enemyDieClips;
    [SerializeField] AudioClip[] playerDieClips;
    [SerializeField] AudioClip correctCombo;
    [SerializeField] AudioClip incorrectCombo;
    private float sfxVolume;
    AudioSource myAudioSource;


    private void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
        sfxVolume = 1;
    }

    private void Start()
    {
        myAudioSource.Play();
    }

    public void ChangeMusicVolume(float amount)
    {
        myAudioSource.volume += amount;
    }

    public void ChangeSFXVolume(float amount)
    {
        sfxVolume = Mathf.Clamp(sfxVolume + amount, 0, 1f);
    }

    private void PlayClip(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, sfxVolume);
    }

    public void PlayEnemySoundClip()
    {
        PlayClip(enemySounds[Random.Range(0, enemySounds.Length)]);
    }

    public void PlayPlayerDieClip()
    {
        PlayClip(playerDieClips[Random.Range(0, playerDieClips.Length)]);
    }

    public void PlayEnemyDieClip()
    {
        PlayClip(enemyDieClips[Random.Range(0, enemyDieClips.Length)]);
    }

    public void PlayPlayerShootClip()
    {
        PlayClip(playerShootClip);
    }

    public void PlayEnemyShootClip()
    {
        PlayClip(enemyShootClip);
    }

    public void PlayCorrectComboClip()
    {
        PlayClip(correctCombo);
    }

    public void PlayIncorrectComboClip()
    {
        PlayClip(incorrectCombo);
    }

    public void OnMusicVolumeUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ChangeMusicVolume(0.05f);
            Debug.Log(myAudioSource.volume);
        }
    }

    public void OnMusicVolumeDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ChangeMusicVolume(-0.05f);
            Debug.Log(myAudioSource.volume);
        }
    }

    public void OnSFXVolumeUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ChangeSFXVolume(0.1f);
            Debug.Log(sfxVolume);
        }
    }

    public void OnSFXVolumeDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ChangeSFXVolume(-0.1f);
            Debug.Log(sfxVolume);
        }
    }

}
