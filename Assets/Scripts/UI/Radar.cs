using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public static Radar Instance { get; private set; } = null;

    private readonly List<Transform> targets = new List<Transform>();
    private readonly List<GameObject> radarPings = new List<GameObject>();
    private float radarRange = 4000;

    private Transform playerShipTransform;
    public GameObject RadarPingObject;

    public GameObject ObjectiveArrow;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        playerShipTransform = GameObject.FindGameObjectWithTag("PlayerShip").transform;
    }

    private void Start()
    {
        ObjectiveArrow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Transform t;
        for (int i = 0; i < targets.Count; i++)
        {
            t = targets[i];
            GameObject ping = radarPings[i];
            Vector2 between = t.position - playerShipTransform.position;
            float distSquared = between.sqrMagnitude;
            if (distSquared < radarRange) { 
                ping.SetActive(true);
                ping.GetComponent<RectTransform>().anchoredPosition = between * ((radarRange + (radarRange / 4)) / radarRange);
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

    public void RemoveTarget(Transform t)
    {
        GameObject ping = radarPings[targets.IndexOf(t)];
        if (ping != null)
        {
            ping.SetActive(false);
            radarPings.Remove(ping);
        }
        targets.Remove(t);
    }
}
