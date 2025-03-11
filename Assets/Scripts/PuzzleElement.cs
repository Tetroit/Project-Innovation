using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public abstract  class PuzzleElement : MonoBehaviour
{
    public Collider coll;
    public bool isSolved { get; private set; } = false;
    public bool isBlocked { get; private set; } = false;

    protected void OnEnable()
    {
        coll = GetComponent<Collider>();
        PuzzleManager.Add(this);
    }
    protected void OnDisable()
    {
        PuzzleManager.Remove(this);
    }
    private void LateUpdate()
    {
        if (PuzzleManager.instance.current == this)
            Move();
    }
    public virtual void Select()
    {
        //Move();
    }
    public virtual void Deselect()
    {
    }
    public void MarkSolved() { isSolved = true; }
    public abstract void Move();
    public abstract void OnSolved();
}
