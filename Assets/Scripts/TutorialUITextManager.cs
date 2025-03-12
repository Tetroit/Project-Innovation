using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;
//using static UnityEngine.InputManagerEntry;
using UnityEngine.InputSystem;

public class TutorialUITextManager : MonoBehaviour
{
    public enum TutorialStage //TT == Tutorial Text
    {
        TT0 = 0,
        TT1 = 1,
        TT2 = 2,
        TT3 = 3,
        TT4 = 4,
        TT5 = 5,
    }

    public TutorialStage currentStage = TutorialStage.TT1;
    public TMP_Text tutorialText;
    [SerializeField]
    Box1Puzzle1 puzzle1;
    [SerializeField]
    Box1Puzzle2 puzzle2;

    public static int TTCount = 0;     
    public int timer = 0;
    private bool hasUpdatedTTCount = false;

    void Start()
    {
        UpdateTutorialText();
    }
    private void OnEnable()
    {
        RotatingElement.onFirstElementClicked += SetStageClickOnGear;
        RotatingElement.onFirstElementRotated += SetStageRotateGear;
        GameManager.instance.onFirstSwitchDark += SetStageTurnOffLight;
        puzzle1.onSolved += SetStageRotatingPuzzleSolved;
        puzzle2.onSolved += SetStageSignPuzzleSolved;
    }

 
    void Update()
    {
      
        if(Keyboard.current.qKey.wasPressedThisFrame)
        {
            currentStage++;
        }

        UpdateTutorialText();

    }

    void SetStage(TutorialStage stage)
    {
        currentStage = stage;
        UpdateTutorialText();
    }

    void SetStageClickOnGear() => SetStage(TutorialStage.TT1);
    void SetStageRotateGear()
    {
        GameManager.instance.onSwitchDark += SetStageTurnOffLight;
        SetStage(TutorialStage.TT2);
    }
    void SetStageTurnOffLight()
    {
        GameManager.instance.onSwitchDark -= SetStageTurnOffLight;
        SetStage(TutorialStage.TT3);
    }
    void SetStageRotatingPuzzleSolved(Puzzle puzzle) => SetStage(TutorialStage.TT4);
    void SetStageSignPuzzleSolved(Puzzle puzzle) => SetStage(TutorialStage.TT5);
 
    void UpdateTutorialText()
    {
        switch (currentStage)
        {
            case TutorialStage.TT0:
                tutorialText.text = "Solve the puzzle on the box to release the Spirit"; //default text (start)
                break;
            case TutorialStage.TT1:
                tutorialText.text = "To solve the puzzle you need to first press, align and connect the lines by rotating your phone"; //click on gear
                break;
            case TutorialStage.TT2:
                tutorialText.text = "If you are strugeling to align the lines turn the light off, it might help you"; //rotate gear
                break;
            case TutorialStage.TT3:
                tutorialText.text = "But be careful in the dark, if you spend too much time in here you will lose"; //turn light off
                break;
            case TutorialStage.TT4:
                tutorialText.text = "Now you should be able to input the Constellation Signs in the Stars by pressing on the 4 on the same face"; //complete turning part
                break;
            case TutorialStage.TT5:
                tutorialText.text = "You have completed the Tutorial!"; //complete sign input
                break;
            default:
                currentStage = 0;
                break;
        }
    }


}