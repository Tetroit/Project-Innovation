using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public abstract  class PuzzleElement : MonoBehaviour
{
    public Collider coll;
    public bool isSolved = false;
    public bool isBlocked = false;
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
    public void Select()
    {
        var mr = GetComponent<MeshRenderer>();
        mr.material.color = Color.yellow;
        //Move();
    }
    public void Deselect()
    {
        var mr = GetComponent<MeshRenderer>();
        mr.material.color = Color.gray;
    }

    public abstract void Move();
    public abstract void OnSolved();
}
