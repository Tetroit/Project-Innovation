using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotatingElement : PuzzleElement
{
    public float speed;
    public Vector3 axis;
    public override void Move()
    {
        float angle = 0;
        if (SensorInput.DeviceFound(SensorInput.gravitySensorLayout))
        {
            angle = SensorInput.GetControlValue(SensorInput.gravitySensor.gravity).x * -speed;
        }
        else
        {
            angle += Keyboard.current.dKey.isPressed ? speed/2 : 0;
            angle -= Keyboard.current.aKey.isPressed ? speed/2 : 0;
        }

        transform.Rotate(axis, angle * Time.deltaTime);
    }

    public override void OnSolved()
    {
        var mr = GetComponent<MeshRenderer>();
        mr.material.color = Color.green;
    }
}
