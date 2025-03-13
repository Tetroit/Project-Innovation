using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box1Puzzle2 : Puzzle
{
    private FMOD.Studio.EventInstance boxSolvedSound;

    enum symbols
    {
        ALPHA = 0,
        DELTA = 1,
        LAMBDA = 2,
        PI = 3,
        THETA = 4
    }
    ImageButton button(int n) => elements[n] as ImageButton;
    public override bool CheckCondition()
    {
        if (button(0).currentIcon == (int)symbols.LAMBDA &&
            button(1).currentIcon == (int)symbols.THETA &&
            button(2).currentIcon == (int)symbols.ALPHA &&
            button(3).currentIcon == (int)symbols.PI)
            return true;
        return false;
    }

    protected override void Setup()
    {
        onSolved += PlayBoxSolvedSound;
        for (int i = 0; i<4; i++)
            button(i).SetIcon(Random.Range(0, 5));
    }

    void PlayBoxSolvedSound(Puzzle puzzle)
    {
        if (!boxSolvedSound.isValid())
        {
            boxSolvedSound = FMODUnity.RuntimeManager.CreateInstance("event:/Puzzle_box_solved");
            boxSolvedSound.start();
        }
    }
}
