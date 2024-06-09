using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private float lifeTime = 1.0f;

    private void Start()
    {
        particles = GetComponentsInChildren<ParticleSystem>();

        Destroy(gameObject, lifeTime);
    }

    public void Play()
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop();
            particle.Play();
        }
    }
}
