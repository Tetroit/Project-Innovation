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
    private bool hasSeenTT0 = false;
    private bool hasSeenTT1 = false;
    private bool hasSeenTT2 = false;
    private bool hasSeenTT3 = false;
    private bool hasSeenTT4 = false;
    private bool hasSeenTT5 = false;


    void Disable()
    {
        StartCoroutine(DisableAnim(1));
    }
    IEnumerator DisableAnim(float time = 1)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            float fac = 1 - t / time;
            tutorialText.alpha = fac;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    void Start()
    {
        UpdateTutorialText();
    }
    private void OnEnable()
    {
        RotatingElement.onFirstElementClicked += SetStageClickOnGear;
        RotatingElement.onFirstElementRotated += SetStageRotateGear;
        //GameManager.instance.onFirstSwitchDark += SetStageTurnOffLight;
        puzzle1.onSolved += SetStageRotatingPuzzleSolved;
        puzzle2.onSolved += SetStageSignPuzzleSolved;

        //turn yellow whenever player switches off the light
        GameManager.instance.onSwitchDark += SetTextToDark;
        //turn black whenever player switches on the light
        GameManager.instance.onSwitchLight += SetTextToLight;
        GameManager.instance.onGhostFail += Disable;
    }
    private void OnDisable()
    {
        GameManager.instance.onSwitchDark -= SetTextToDark;
        GameManager.instance.onSwitchLight -= SetTextToLight;

        RotatingElement.onFirstElementClicked -= SetStageClickOnGear;
        RotatingElement.onFirstElementRotated -= SetStageRotateGear;

        puzzle1.onSolved -= SetStageRotatingPuzzleSolved;
        puzzle2.onSolved -= SetStageSignPuzzleSolved;
        GameManager.instance.onGhostFail -= Disable;
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
 
    void SetTextToDark()
    {
        var txt = GetComponent<TMP_Text>();
        if (txt != null)
            txt.color = Color.white;
    }
    void SetTextToLight()
    {
        var txt = GetComponent<TMP_Text>();
        if (txt != null)
            txt.color = Color.white;
    }
    void UpdateTutorialText()
    {
        switch (currentStage)
        {
            case TutorialStage.TT0:
                if (hasSeenTT0 == false)
                { 
                    //GetComponent<TMP_Text>().color = Color.black;
                    tutorialText.text = "Solve all the puzzles on the box to release the Spirit"; //default text (start)
                    hasSeenTT0 = true;
                }
                break;
            case TutorialStage.TT1:
                if (hasSeenTT1 == false)
                {
                    //GetComponent<TMP_Text>().color = Color.black;
                    tutorialText.text = "To solve the puzzle you need to align the wheels by rotating your phone, so that the lines connect"; //click on gear
                    hasSeenTT1 = true;
                }
                break;
            case TutorialStage.TT2:
                if (hasSeenTT2 == false)
                {
                    //GetComponent<TMP_Text>().color = Color.black;
                    tutorialText.text = "If you are strugeling to align the lines turn the light off, it might help you"; //rotate gear
                    hasSeenTT2 = true;
                }
                break;
            case TutorialStage.TT3:
                if (hasSeenTT3 == false)
                {
                    //GetComponent<TMP_Text>().color = Color.yellow;
                    tutorialText.text = "But be careful in the dark, if you spend too much time in here you will lose"; //turn light off
                    hasSeenTT3 = true;
                }
                break;
            case TutorialStage.TT4:
                if (hasSeenTT4 == false)
                {
                    //GetComponent<TMP_Text>().color = Color.yellow;
                    tutorialText.text = "Now that the constellation is aligned, you are able to input the Constellation Signs in the Stars by pressing on the symbols in the wheels"; //complete turning part
                    hasSeenTT4 = true;
                }
                break;
            case TutorialStage.TT5:
                if (hasSeenTT5 == false)
                {
                    //GetComponent<TMP_Text>().color = Color.white;
                    tutorialText.text = "You have completed the Tutorial!"; //complete sign input
                    hasSeenTT5 = true;
                }
                break;
            default:
                currentStage = 0;
                hasSeenTT0 = false;
                hasSeenTT1 = false; 
                hasSeenTT2 = false;
                hasSeenTT3 = false;
                hasSeenTT4 = false;
                hasSeenTT5 = false;
                break;
        }
    }


}