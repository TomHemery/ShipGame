using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiningStationController : MonoBehaviour
{
    public static MiningStationController Instance { get; private set; } = null;
    public MiningStationUIToggle uiToggle;

    public GameObject jumpParticles;
    public SpriteRenderer mRenderer;
    private GameObject playerShip;
    public GameObject[] entryLights;
    public AudioSource jumpAudioSource;

    public HullRepairer hullRepairer;
    public ResourceFiller o2Gen;
    public ResourceFiller jumpDriveFueler;
    public Resource jumpResource;

    public UnityEvent OnJumpCompleted = new UnityEvent();

    private const float JUMP_DURATION = 4.5f;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        jumpParticles.SetActive(false);
        playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
    }

    public void JumpToArea(Area area){
        StartCoroutine(JumpCoroutine(area));
    }

    private IEnumerator JumpCoroutine(Area area) {
        //hide station UI
        uiToggle.SilentUnPauseAndHide();

        //pause the game, hide player ship and lights, show particles
        GameManager.PauseSim();
        playerShip.SetActive(false);
        jumpParticles.SetActive(true);
        foreach (GameObject light in entryLights) light.SetActive(false);

        jumpAudioSource.Play();

        //fade out ship over time
        float t = 0.0f;
        Color c = mRenderer.color;
        while (t < JUMP_DURATION)
        {
            t += Time.deltaTime;
            c.a = 1 - t / 5.0f;
            mRenderer.color = c;
            yield return null;
        }

        //show ship, show player, show lights, hide particles
        c.a = 1.0f;
        mRenderer.color = c;
        playerShip.SetActive(true);
        foreach (GameObject light in entryLights) light.SetActive(true);
        jumpParticles.SetActive(false);

        //empty jump drive
        jumpResource.EmptyResource();

        //load destination and un pause
        GameManager.LoadArea(area);
        GameManager.UnPauseSim();
        
        //alert listeners
        OnJumpCompleted.Invoke();
    }

    
}