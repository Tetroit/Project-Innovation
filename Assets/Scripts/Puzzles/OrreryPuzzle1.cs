using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrreryPuzzle1 : Puzzle
{
    private readonly float[] targetRotations = { -149.074f, -90.358f, -31.545f };
    private const float tolerance = 10f;

    protected override void Setup()
    {

    }

    public override bool CheckCondition()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            float rotY = NormalizeAngle(elements[i].transform.rotation.eulerAngles.y);

            if (Mathf.Abs(rotY - targetRotations[i]) >= tolerance)
            {
                return false;
            }
        }
        return true;
    }

    private float NormalizeAngle(float angle)
    {
        return (angle > 180f) ? angle - 360f : angle;
    }
}
