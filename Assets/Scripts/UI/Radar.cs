using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public static Radar Instance { get; private set; } = null;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> radarPings = new List<GameObject>();

    public Transform PlayerShip;
    public float RadarRange = 100;
    public GameObject RadarPingObject;

    private void Awake()
    {
        if (Instance != null) Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Transform t;
        for (int i = 0; i < targets.Count; i++)
        {
            t = targets[i];
            GameObject ping = radarPings[i];
            if (Vector2.Distance(t.position, PlayerShip.position) < RadarRange)
            {

                ping.SetActive(true);
                
            }
            else ping.SetActive(false);
        }
    }

    public void AddTarget(Transform t)
    {
        targets.Add(t);
        GameObject ping = Instantiate(RadarPingObject, transform);
        radarPings.Add(ping);
        ping.SetActive(false);
    }
}
