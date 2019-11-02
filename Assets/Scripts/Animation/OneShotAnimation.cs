using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotAnimation : MonoBehaviour
{

    public Animator animator;
    public string animName;

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).length <
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime) {
            Destroy(gameObject);
        }
    }
}
