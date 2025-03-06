using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    public Camera m_cam;
    public PuzzleElement current { get; private set; }

    public List<PuzzleElement> collection = new();
    public List<Puzzle> puzzles = new();
    public int completedPuzzles = 0;
    void CheckSelection(Ray ray)
    {
        foreach (PuzzleElement element in collection)
        {
            if(element.isSolved || element.isBlocked) continue;
            Debug.Log("ray check " + current?.name);
            RaycastHit hit;
            if (element.coll.Raycast(ray, out hit, float.MaxValue))
            {
                if (current != null)
                {
                    DeselectCurrent();
                    if (element == current) continue;
                }
                current = element;
                element.Select();

                return;
            }
        }
        if (current == null) return;
        DeselectCurrent();
    }
    private void OnEnable()
    {
        if (m_cam == null)
            m_cam = Camera.main;
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }
    private void Update()
    {
        if (SensorInput.DeviceFound(SensorInput.touchscreenLayout))
        {
            Debug.Log("Topuch: " + SensorInput.touchscreen.position.ReadValue());
            if (SensorInput.touchscreen.press.wasPressedThisFrame)
            {
                Debug.Log("tap");
                Ray ray = m_cam.ScreenPointToRay(SensorInput.GetControlValue(SensorInput.touchscreen.position));
                CheckSelection(ray);
            }
        }
        foreach (var puzzle in puzzles)
        {
            if (!puzzle.isSolved)
                puzzle.Check();
        }
    }

    public static void Add(PuzzleElement element)
    {
        instance.collection.Add(element);
    }
    public static void Remove(PuzzleElement element)
    {
        instance.collection.Remove(element);
    }
    public static void Add(Puzzle puzzle)
    {
        instance.puzzles.Add(puzzle);
        puzzle.onSolved += instance.OnPuzzleComplete;
    }
    public static void Remove(Puzzle puzzle)
    {
        instance.puzzles.Remove(puzzle);
        puzzle.onSolved -= instance.OnPuzzleComplete;
    }

    public static void DeselectCurrent()
    {
        instance.current.Deselect();
        instance.current = null;
    }

    public void OnPuzzleComplete(Puzzle puzzle)
    {
        completedPuzzles++;
        if (completedPuzzles == puzzles.Count)
        {
            GameManager.instance.PuzzlesCompleted();
        }
    }
}
