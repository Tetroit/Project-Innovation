using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box1Puzzle1 : Puzzle
{
    private FMOD.Studio.EventInstance puzzleSolvedSound;

    protected override void Setup()
    {
        onSolved += PlayPuzzleSolvedSound;
    }
    public override bool CheckCondition()
    {
        float rot1 = elements[0].transform.rotation.eulerAngles.z;
        float rot2= elements[1].transform.rotation.eulerAngles.z;
        float rot3= elements[2].transform.rotation.eulerAngles.z;
        if (Mathf.Abs(rot1 - rot2) < 10f && Mathf.Abs(rot3 - rot2) < 10f)
            return true;
        return false;
    }

    void PlayPuzzleSolvedSound(Puzzle puzzle)
    {
        if (!puzzleSolvedSound.isValid())
        {
            puzzleSolvedSound = FMODUnity.RuntimeManager.CreateInstance("event:/Puzzle_solved");
            puzzleSolvedSound.start();
        }
    }
}
