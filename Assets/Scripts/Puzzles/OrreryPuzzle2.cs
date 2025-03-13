using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrreryPuzzle2 : Puzzle
{
    private readonly float[] targetRotations = { -51.644f, 108.605f, -136.443f, 13.939f, -14.305f, 165.561f, -105.264f, 75.796f };
    private const float tolerance = 10f;
    private FMOD.Studio.EventInstance puzzleSolvedSound;
    private FMOD.Studio.EventInstance boxSolvedSound;

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
        if (!boxSolvedSound.isValid())
        {
            boxSolvedSound = FMODUnity.RuntimeManager.CreateInstance("event:/Puzzle_box_solved");
            boxSolvedSound.start();
        }
        return true;
    }

    private float NormalizeAngle(float angle)
    {
        return (angle > 180f) ? angle - 360f : angle;
    }
}
