using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    public static Camera m_cam;
    public static PuzzleElement current { get; private set; }

    public static List<PuzzleElement> collection = new List<PuzzleElement>();
    public static List<Puzzle> puzzles;
    void CheckSelection(Ray ray)
    {
        foreach (PuzzleElement element in collection)
        {
            Debug.Log("ray check " + current?.name);
            RaycastHit hit;
            if (element.coll.Raycast(ray, out hit, float.MaxValue))
            {
                if (current != null)
                {
                    if (element == current) continue;
                    current.Deselect();
                }
                current = element;
                element.Select();

                return;
            }
        }
        if (current == null) return;

        current.Deselect();
        current = null;
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
        if (SensorInput.DeviceFound(SensorInput.touchscreenLayout) && SensorInput.touchscreen.primaryTouch.tap.wasPressedThisFrame)
        {
            Ray ray = m_cam.ScreenPointToRay(SensorInput.GetControlValue(SensorInput.touchscreen.position));
            CheckSelection(ray);
        }
    }

    public static void Add(PuzzleElement element)
    {
        collection.Add(element);
    }
    public static void Remove(PuzzleElement element)
    {
        collection.Remove(element);
    }
    public static void Add(Puzzle puzzle)
    {
        puzzles.Add(puzzle);
    }
    public static void Remove(Puzzle puzzle)
    {
        puzzles.Remove(puzzle);
    }
}
