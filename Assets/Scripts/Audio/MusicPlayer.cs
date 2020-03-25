using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] introTracks;
    public AudioClip[] lowTracks;
    public AudioClip[] midTracks;
    public AudioClip[] highTracks;

    private AudioClip[] activeTracks;
    private int trackIndex = -1;

    [HideInInspector]
    public MusicState playerState = MusicState.None;

    public MusicPlayer Instance { get; private set; } = null;
    public AudioSource MusicSource { get; private set; } = null;

    private void Awake()
    {
        Instance = this;
        MusicSource = GetComponent<AudioSource>();
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
            playerState = MusicState.Intro;
        }
    }

    private void FixedUpdate()
    {
        if (playerState != MusicState.None && !MusicSource.isPlaying) {
            if (trackIndex < activeTracks.Length - 1)
            {
                trackIndex++;
            }

            else
            {
                switch (playerState)
                {
                    case MusicState.Intro:
                        playerState = MusicState.Low;
                        activeTracks = lowTracks;
                        trackIndex = 0;
                        break;
                }
            }

            MusicSource.clip = activeTracks[trackIndex];
            MusicSource.Play();
        }

    }

    public enum MusicState {
        Intro,
        Low,
        Mid,
        High,
        None
    }

}
