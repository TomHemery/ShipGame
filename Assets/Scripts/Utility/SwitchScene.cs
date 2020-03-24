using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public string targetSceneName;
    public string targetSceneEntryPoint;
    public bool animateExit;

    public void LoadTargetScene() {
        PlayerSpawnController transitionBehaviour = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<PlayerSpawnController>();
        transitionBehaviour.sceneEntryPoint = targetSceneEntryPoint;
        transitionBehaviour.animateExit = animateExit;

        SceneManager.LoadScene(targetSceneName);
    }
}
