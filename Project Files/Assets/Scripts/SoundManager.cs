using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSrc;
    [SerializeField] float transitionSpeed;
    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sound)
    {
        if (audioSrc.isPlaying)
        {
            audioSrc.Stop();
        }

        if (sound)
        {
            audioSrc.clip = sound;
            audioSrc.Play();
        }
    }

    public void TransitionMusic(AudioClip music)
    {
        StartCoroutine(FadeOutIn(audioSrc, transitionSpeed, music));
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOut(audioSrc, transitionSpeed));
    }

    public void PlayMusic(AudioClip music)
    {
        StartCoroutine(FadeIn(audioSrc, transitionSpeed, music));
    }

    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.Stop();
    }

    public IEnumerator FadeIn(AudioSource audioSource, float FadeTime, AudioClip music)
    {
        audioSource.Play();
        PlaySound(music);

        audioSource.volume = 0f;
        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime / FadeTime;
            yield return null;
        }
    }

    public IEnumerator FadeOutIn(AudioSource audioSource, float FadeTime, AudioClip music)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.Play();
        PlaySound(music);

        audioSource.volume = 0f;
        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime / FadeTime;
            yield return null;
        }
    }
}
