using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Puzzle : MonoBehaviour
{
    [SerializeField]
    protected List<PuzzleElement> elements;
    public Action<Puzzle> onSolved;

    public bool isSolved;
    public abstract bool CheckCondition();
    public void Check()
    {
        if (CheckCondition())
        {
            OnSolved();
        }
    }
    protected abstract void Setup();
    protected void OnEnable()
    {
        PuzzleManager.Add(this);
        Setup();
    }
    protected void OnDisable()
    {
        PuzzleManager.Remove(this);
    }
    
    protected void OnSolved()
    {
        isSolved = true;
        PuzzleManager.DeselectCurrent();
        foreach (var element in elements)
        {
            if (PuzzleManager.instance.current == element)
            {
                element.Deselect();
            }
            element.isSolved = true;
            element.OnSolved();
        }
        onSolved?.Invoke(this);
    }
}
