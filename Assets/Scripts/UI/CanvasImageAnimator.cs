using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasImageAnimator : MonoBehaviour
{
    public Image imageTarget;
    public SpriteRenderer spriteRendererSource;
    public Animator animator;

    void Start()
    {
        // prevent the sprite renderer from being used
        spriteRendererSource.enabled = false;
    }

    void Update()
    {
        // if a controller is running, set the sprite
        if (animator.runtimeAnimatorController)
        {
            imageTarget.sprite = spriteRendererSource.sprite;
        }
    }
}
