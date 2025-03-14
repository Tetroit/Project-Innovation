using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Box1Animation : MonoBehaviour
{
    private FMOD.Studio.EventInstance animationSound;
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

    private Vector3 gScale;

    private void OnEnable()
    {
        GameManager.instance.onGhostSuccess += Trigger;
    }
    private void OnDisable()
    {
        GameManager.instance.onGhostSuccess -= Trigger;
    }

    private void Awake()
    {
        gScale = ghostAnim.transform.localScale;
        ghostAnim.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
    }

    private void Update()
    {
        Debug.Log(gScale);
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            ghostAnim.transform.localScale = gScale;
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
        if (!animationSound.isValid())
        {
            animationSound = FMODUnity.RuntimeManager.CreateInstance("event:/Gost");
            animationSound.start();

            Debug.Log("play pls");
        }
    }

    void TriggerBox()
    {
        Debug.Log("triggerbox");
        openAnim.SetTrigger("box_open");
    }
}
