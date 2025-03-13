using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private FMOD.Studio.EventInstance animationSound;

    public void PlayAnimationSound()
    {
        if (!animationSound.isValid())
        {
            animationSound = FMODUnity.RuntimeManager.CreateInstance("event:/Gost");
            animationSound.start();

            Debug.Log("play pls");
        }
    }
}
