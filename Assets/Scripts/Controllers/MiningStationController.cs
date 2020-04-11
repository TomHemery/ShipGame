using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningStationController : MonoBehaviour
{
    public static MiningStationController Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
    }

    public HullRepairer m_HullRepairer;
    public ResourceFiller m_O2Gen;
    public ResourceFiller m_JumpDriveFueler;
}