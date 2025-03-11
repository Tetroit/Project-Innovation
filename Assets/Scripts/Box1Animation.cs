using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Box1Animation : MonoBehaviour
{
    [SerializeField]
    Animator openAnim;
    [SerializeField]
    Animator ghostAnim;
    [SerializeField]
    Animator mouthAnim;

    [SerializeField]
    float boxDelay = 0.0f;
    [SerializeField]
    float ghostDelay = 2.0f;
    private void OnEnable()
    {
        GameManager.instance.onGhostSuccess += Trigger;
    }
    private void OnDisable()
    {
        GameManager.instance.onGhostSuccess -= Trigger;
    }
    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Trigger();
        }
    }
    void Trigger()
    {
        Invoke(nameof(TriggerBox), boxDelay);
        Invoke(nameof(TriggerGhost), ghostDelay);
    }
    void TriggerGhost()
    {
        ghostAnim.SetTrigger("ghost_animation_start");
        mouthAnim.SetTrigger("ghost_animation_local");
    }

    void TriggerBox()
    {
        openAnim.SetTrigger("box_open");
    }
}
