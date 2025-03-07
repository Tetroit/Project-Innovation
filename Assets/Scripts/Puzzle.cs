using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Puzzle : MonoBehaviour
{
    [SerializeField]
    protected List<PuzzleElement> elements;
    public Action<Puzzle> onSolved;

    /// <summary>
    /// true if the puzzle is solved
    /// </summary>
    public bool isSolved;
    /// <summary>
    /// Return 
    /// </summary>
    /// <returns></returns>
    public abstract bool CheckCondition();
    public void Check()
    {
        if (CheckCondition())
        {
            OnSolved();
        }
    }
    /// <summary>
    /// Initial setup for the puzzle
    /// </summary>
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
    /// <summary>
    /// Is called when the puzzle is solved
    /// </summary>
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
            element.MarkSolved();
            element.OnSolved();
        }
        onSolved?.Invoke(this);
    }
}
