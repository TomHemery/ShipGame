using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public static Radar Instance { get; private set; } = null;

    private readonly List<Transform> targets = new List<Transform>();
    private readonly List<GameObject> radarPings = new List<GameObject>();

    private float radarRange = 8000;
    private float pointAtStationDist = 100;
    private float radarRadius = 64f;

    private Transform playerShipTransform;
    private Transform playerStationTransform;
    public GameObject RadarPingObject;

    public GameObject StationArrow;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        playerShipTransform = GameObject.FindGameObjectWithTag("PlayerShip").transform;
        playerStationTransform = GameObject.FindGameObjectWithTag("MiningStation").transform;
    }

    private void Start()
    {
        StationArrow.SetActive(false);
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
            if (between.sqrMagnitude < radarRange) { 
                ping.SetActive(true);
                ping.GetComponent<RectTransform>().anchoredPosition = between * 0.8f;
            }
            else ping.SetActive(false);
        }

        Vector2 playerToStation = playerStationTransform.position - playerShipTransform.position;
        if (playerToStation.sqrMagnitude > pointAtStationDist)
        {
            StationArrow.SetActive(true);
            playerToStation *= 1.1f;
            playerToStation = Vector2.ClampMagnitude(playerToStation, radarRadius);
            StationArrow.GetComponent<RectTransform>().anchoredPosition = playerToStation;
            StationArrow.transform.up = playerToStation;
        }
        else StationArrow.SetActive(false);
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
