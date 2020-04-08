using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] introTracks;
    public AudioClip[] lowTracks;
    public AudioClip[] midTracks;
    public AudioClip[] transitionTracks;
    public AudioClip[] highTracks;

    private Dictionary<MusicState, AudioClip[]> stateToTrackMap;
    private AudioClip[] activeTracks;
    private int trackIndex = -1;

    [HideInInspector]
    public MusicState PlayerState { get; private set; } = MusicState.None;

    public static MusicPlayer Instance { get; private set; } = null;
    public AudioSource MusicSource { get; private set; } = null;

    private void Awake()
    {
        Instance = this;
        MusicSource = GetComponent<AudioSource>();
        stateToTrackMap = new Dictionary<MusicState, AudioClip[]>()
        {
            {MusicState.Intro, introTracks },
            {MusicState.Low, lowTracks },
            {MusicState.Mid, midTracks },
            {MusicState.High, highTracks },
            {MusicState.Transition, transitionTracks },
            {MusicState.None, new AudioClip[0] }
        };
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameplayTest")
        {
            Debug.Log("Loaded gameplay test, setting music");
            activeTracks = introTracks;
            trackIndex = -1;
            PlayerState = MusicState.Intro;
        }
    }

    private void FixedUpdate()
    {
        if (PlayerState != MusicState.None && !MusicSource.isPlaying) {
            if (trackIndex < activeTracks.Length - 1)
            {
                trackIndex++;
            }

            else
            {
                switch (PlayerState)
                {
                    case MusicState.Intro:
                        PlayerState = MusicState.Low;
                        activeTracks = lowTracks;
                        trackIndex = 0;
                        break;
                    case MusicState.Transition:
                        PlayerState = MusicState.High;
                        activeTracks = highTracks;
                        trackIndex = 0;
                        break;
                }
            }

            MusicSource.clip = activeTracks[trackIndex];
            MusicSource.Play();
        }
    }

    public void ForceSetPlayerState(MusicState state) {
        PlayerState = state;
        activeTracks = stateToTrackMap[state];
        trackIndex = 0;

        if (state == MusicState.None)
        {
            MusicSource.clip = null;
            MusicSource.Stop();
        }
        else {
            MusicSource.clip = activeTracks[0];
            MusicSource.Play();
        }
    }

    public void FadeOut(float fadeTime) {
        StartCoroutine(FadeOutCoroutine(fadeTime));
    }

    private IEnumerator FadeOutCoroutine(float fadeTime)
    {
        float startVolume = MusicSource.volume;

        while (MusicSource.volume > 0)
        {
            MusicSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        MusicSource.Stop();
        MusicSource.volume = startVolume;
    }

    public void FadeToNewState(float fadeTime, MusicState newState) {
        StartCoroutine(FadeToNewStateCoroutine(fadeTime, newState));
    }

    private IEnumerator FadeToNewStateCoroutine(float fadeTime, MusicState newState) {
        float startVolume = MusicSource.volume;

        while (MusicSource.volume > 0) {
            MusicSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        MusicSource.Stop();
        MusicSource.volume = startVolume;
        ForceSetPlayerState(newState);
    }

    public enum MusicState {
        Intro,
        Low,
        Mid,
        High,
        Transition,
        None
    }

}
