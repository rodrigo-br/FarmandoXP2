using UnityEngine;

public class SubMusic : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
