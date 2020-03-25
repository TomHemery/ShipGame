using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    public static SoundEffectPlayer Instance { get; private set; } = null;
    public static AudioSource SoundEffectSource { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        SoundEffectSource = GetComponent<AudioSource>();
    }

    public static IEnumerator FadeOut (AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
