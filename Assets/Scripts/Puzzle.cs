using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Puzzle : MonoBehaviour
{
    protected List<PuzzleElement> elements;

    public bool isSolved;
    public abstract bool Check();
    protected void OnEnable()
    {
        PuzzleManager.Add(this);
    }
    protected void OnDisable()
    {
        PuzzleManager.Remove(this);
    }
    protected void OnSolved() 
    {
        foreach (var element in elements)
        {
            element.isSolved = true;
        }
    }
}
