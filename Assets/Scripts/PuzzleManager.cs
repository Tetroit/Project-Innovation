using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngineInternal;

public class PuzzleManager : MonoBehaviour
{
    TutorialUITextManager tutorialUiTextManager;
    public static PuzzleManager instance;

    public Camera m_cam;
    public PuzzleElement current { get; private set; }

    public List<PuzzleElement> collection = new();
    public List<Puzzle> puzzles = new();
    public int completedPuzzles = 0;

    public GameObject functionParticle;
    private float destroyParticleSystemTime = 1f;


    void CheckSelection(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue))
        {
            PuzzleElement element = hit.transform.GetComponent<PuzzleElement>();

            if (element != null && !element.isSolved && !element.isBlocked && collection.Contains(element))
            {
                Debug.Log("Raycast hit: " + hit.transform.name);

                /* if (TutorialUITextManager.TTCount != 3) //
                 {                                       //
                     TutorialUITextManager.TTCount = 3;  // Line from designer Lluis, if it breaks anything delete
                     return;                             //
                 }                                       //*/

                if (hit.collider.CompareTag("star"))
                {
                    Debug.Log("no raycast Hit");
                    GameObject instantiatedParticle = Instantiate(functionParticle, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(instantiatedParticle, destroyParticleSystemTime);
                }

                DeselectCurrent();
                current = element;
                element.Select();
                return; // Stops processing after the first valid hit
            }

        }

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
            if (SensorInput.touchscreen.press.wasPressedThisFrame)
            {
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
        if (instance != null)
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

        /* if (TutorialUITextManager.TTCount != 4) //
         {                                       //
             TutorialUITextManager.TTCount = 4;  // Line from designer Lluis, if it breaks anything delete
             return;                             //
         }                                       //*/
    }

    public static void Remove(Puzzle puzzle)
    {
        instance.puzzles.Remove(puzzle);
        puzzle.onSolved -= instance.OnPuzzleComplete;
    }

    public static void DeselectCurrent()
    {
        if (instance.current == null) return;
        instance.current.Deselect();
        instance.current = null;
    }

    public void OnPuzzleComplete(Puzzle puzzle)
    {
        completedPuzzles++;

        /* if (TutorialUITextManager.TTCount != 5) //
         {                                       //
             TutorialUITextManager.TTCount = 5;  // Line from designer Lluis, if it breaks anything delete
             return;                             //
         }                                       //*/

        if (completedPuzzles == puzzles.Count)
        {
            GameManager.instance.PuzzlesCompleted();
        }
    }
}
