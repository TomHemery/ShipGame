using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private MusicTrack [] allMusicTracks;
    public Dictionary<string, MusicTrack> TrackDictionary { get; private set; } = new Dictionary<string, MusicTrack>();

    private MusicTrack activeTrack;

    public static MusicPlayer Instance { get; private set; } = null;
    public AudioSource MusicSource { get; private set; } = null;

    public const string NO_TRACK = "None";

    private float startVolume;

    private void Awake()
    {
        Instance = this;
        MusicSource = GetComponent<AudioSource>();

        foreach (MusicTrack track in allMusicTracks) {
            TrackDictionary.Add(track.systemName, track);
        }
        startVolume = MusicSource.volume;
    }

    private void FixedUpdate()
    {
        if (!MusicSource.isPlaying && activeTrack != null) {
            MusicSource.clip = activeTrack.GetNextClip();
            MusicSource.Play();
        } 
    }


    /// <summary>
    /// Sets the current track based on the system name passed in, starts playing immediately
    /// Pass in NO_TRACK or "" to stop music
    /// </summary>
    /// <param name="trackName"></param>
    public void SetTrackImmediate(string trackName) {
        //reset the current active track to it's beginning 
        if(activeTrack != null) activeTrack.ResetTrack();
        //load the new track (either nothing, or the associated track in the dictionary based on the passed in name)
        if (trackName == "" || trackName == NO_TRACK)
        {
            activeTrack = null;
            MusicSource.clip = null;
            MusicSource.Stop();
        }
        else {
            activeTrack = TrackDictionary[trackName];
            MusicSource.clip = activeTrack.GetNextClip();
            MusicSource.Play();
        }
        
    }

    public void FadeOut(float fadeTime) {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(fadeTime));
    }

    private IEnumerator FadeOutCoroutine(float fadeTime)
    {
        while (MusicSource.volume > 0)
        {
            MusicSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        MusicSource.volume = startVolume;
        SetTrackImmediate(NO_TRACK);
    }

    public void FadeToNewTrack(float fadeTime, string newTrack) {
        StopAllCoroutines();
        StartCoroutine(FadeToNewTrackCoroutine(fadeTime, newTrack));
    }

    private IEnumerator FadeToNewTrackCoroutine(float fadeTime, string newTrack) {
        float t = 0;
        while (MusicSource.volume > 0) {
            MusicSource.volume = startVolume * (1 - t / fadeTime);
            t += Time.deltaTime;
            yield return null;
        }

        MusicSource.Stop();
        MusicSource.volume = startVolume;
        SetTrackImmediate(newTrack);
    }

    public bool IsPlayingTrack(string trackName) {
        return activeTrack == null ? false : activeTrack.systemName == trackName;
    }

}
