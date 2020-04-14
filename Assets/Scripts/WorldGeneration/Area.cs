using UnityEngine;

[System.Serializable]
public struct Area {
    [SerializeField]
    public string systemName;
    [SerializeField]
    public string prettyName;
    [SerializeField]
    public MusicPlayer.MusicState musicState;
}