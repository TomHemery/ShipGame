using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MusicTrack
{
    public string systemName;
    [SerializeField]
    public AudioClip[] audioClips;
    private int index = 0;

    public AudioClip GetNextClip() {
        AudioClip track = audioClips[index];
        if (index < audioClips.Length - 1) index++;
        return track;
    }

    public void ResetTrack() {
        index = 0;
    }
}
