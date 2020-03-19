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
        PlayerSceneTransitionBehaviour transitionBehaviour = GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<PlayerSceneTransitionBehaviour>();
        transitionBehaviour.sceneEntryPoint = targetSceneEntryPoint;
        transitionBehaviour.animateExit = animateExit;

        SceneManager.LoadScene(targetSceneName);
    }
}
