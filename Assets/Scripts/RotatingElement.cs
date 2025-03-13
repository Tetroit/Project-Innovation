using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotatingElement : PuzzleElement
{
    public float speed;
    public Vector3 axis;

    //for tutorial
    public static Action onFirstElementClicked;
    public static Action onFirstElementRotated;
    static bool firstElementClicked = false;
    static bool firstElementRotated = false;
    private FMOD.Studio.EventInstance turningWheelSound;
    bool sfxPlaying = false;
    public override void Move()
    {
        float angle = 0;
        if (SensorInput.DeviceFound(SensorInput.gravitySensorLayout))
        {
            angle = SensorInput.GetControlValue(SensorInput.gravitySensor.gravity).x * -speed;
        }
        else
        {
            angle += Keyboard.current.dKey.isPressed ? speed / 2 : 0;
            angle -= Keyboard.current.aKey.isPressed ? speed/2 : 0;
        }
        transform.Rotate(axis, angle * Time.deltaTime);

        if (!firstElementRotated && Mathf.Abs(angle/speed) > 0.3f)
        {
            firstElementRotated = true;
            onFirstElementRotated?.Invoke();
        }
        if (Mathf.Abs(angle / speed) > 0.3f)
        {
            if (!sfxPlaying)
            {
                StartSFX();
                sfxPlaying = true;
            }
        }
        else
        {
            if (sfxPlaying)
            {
                StopSFX();
                sfxPlaying = false;
            }
        }
    }

    private void StartSFX()
    {
        if (!turningWheelSound.isValid())
        {
            turningWheelSound = FMODUnity.RuntimeManager.CreateInstance("event:/Wheels_turning");
            turningWheelSound.start();
        }
    }
    private void StopSFX()
    {

        if (!turningWheelSound.isValid())
        {
            turningWheelSound = FMODUnity.RuntimeManager.CreateInstance("event:/Wheels_turning");
        }
        turningWheelSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        turningWheelSound.release();
    }
    public override void OnSolved()
    {
        var mr = GetComponent<MeshRenderer>();
        mr.material.color = Color.green;
    }

    public override void Select()
    {
        var mr = GetComponent<MeshRenderer>();
        mr.material.color = Color.yellow;

        if (!firstElementClicked)
        {
            firstElementClicked = true;
            onFirstElementClicked?.Invoke();
        }
    }
    public override void Deselect()
    {
        var mr = GetComponent<MeshRenderer>();
        mr.material.color = Color.gray;
    }
}
