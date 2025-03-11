using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class TutorialUITextManager : MonoBehaviour
{
    public enum TutorialStage //TT == Tutorial Text
    {
        TT1 = 1,
        TT2 = 2,
        TT3 = 3,
        TT4 = 4,
        TT5 = 5,
        TT6 = 6,
    }

    public TutorialStage currentStage = TutorialStage.TT1;
    public TMP_Text tutorialText; 
    PuzzleManager puzzleManager;  // 
    SensorInput sensorInput;      // 
    Calibration calibration;      // No clue why I need this tbh
    InputManager inputManager;    //
    public static int TTCount = 0;     
    public int timer = 0;
    private bool hasUpdatedTTCount = false;

    void Start()
    {
        UpdateTutorialText();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTutorialProgress();

        if (!hasUpdatedTTCount && timer >= 50 && TTCount == 0)
        {
            TTCount = 1;
            hasUpdatedTTCount = true;
            return;
        }
        else timer++;

        UpdateTutorialText();

    }
    void CheckTutorialProgress()
    {
        switch (currentStage)
        {
            case TutorialStage.TT1: 
                if(TTCount == 0)
                {
                    NextStage();
                }

                break;

            case TutorialStage.TT2:
                if(TTCount == 1)
                {
                    NextStage();
                }

                break;

            case TutorialStage.TT3:
                if(TTCount == 2)
                {
                    NextStage();
                }
                
                break;

            case TutorialStage.TT4:
                if(TTCount == 3)
                {
                    NextStage();
                }

                break;

            case TutorialStage.TT5:
                if(TTCount == 4)
                {
                    NextStage();
                }

                break;

            case TutorialStage.TT6:
                if(TTCount == 5)
                {
                    NextStage();
                }

                break;
        }
    }
    void UpdateTutorialText()
    {
        switch (currentStage)
        {
            case TutorialStage.TT1:
                tutorialText.text = "Solve the puzzle on the box to release the Spirit";
                break;
            case TutorialStage.TT2:
                tutorialText.text = "To solve the puzzle you need to first press and align the Constellation by rotating your phone";
                break;
            case TutorialStage.TT3:
                tutorialText.text = "But you can only see the Constellation in the dark, so you have to turn the lights off";
                break;
            case TutorialStage.TT4:
                tutorialText.text = "Be careful in the dark, if you spend too much time in here you will lose";
                break;
            case TutorialStage.TT5:
                tutorialText.text = "Now you should be able to input the Constellation Signs in the Stars";
                break;
            case TutorialStage.TT6:
                tutorialText.text = "You have completed the Tutorial!";
                break;
        }
    }
    void NextStage()
    {
        if ((int)currentStage < System.Enum.GetValues(typeof(TutorialStage)).Length - 1)
        {
            currentStage++;
            hasUpdatedTTCount = false; // Allow next TTCount update
            UpdateTutorialText();
            
        }
    }

}