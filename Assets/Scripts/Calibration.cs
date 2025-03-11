using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Calibration : MonoBehaviour
{
    public enum Steps
    {
        INIT = 0,
        WAIT_LIGHT = 1,
        PROCESS_LIGHT = 2,
        WAIT_DARK = 3,
        PROCESS_DARK = 4,
        CHECK = 5,
        COMPLETE = 6,
        NEXTSCENE = 7,
    }

    Steps step = Steps.INIT;

    [SerializeField]
    TextMeshProUGUI description;
    TextMeshProUGUI buttonText;
    [SerializeField]
    Button button;
    [SerializeField]
    Button retryButton;

    bool isCalibrated = false;
    [SerializeField]
    Material indicatorMaterial;

    Tween alphaUp;
    Tween alphaDown;

    public float animationTime = 0.5f;
    private void Start()
    {
        button.onClick.AddListener(OnNextButton);
        retryButton.onClick.AddListener(OnRetryButton);
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        alphaDown = DOTween.ToAlpha(() => description.color, x => description.color = x, 0, animationTime);
        alphaUp = DOTween.ToAlpha(() => description.color, x => description.color = x, 1, animationTime);

        var col = description.color;
        col.a = 0;
        description.color = col;

        SetState(0);

        indicatorMaterial.SetFloat("_Fac", 1);
    }

    private void Update()
    {
        if (isCalibrated)
            UpdateLight();
    }
    void OnNextButton()
    {
        if (alphaDown != null && alphaDown.active) return;
        if (alphaUp != null && alphaUp.active)
            DOTween.Kill(alphaUp);
        alphaDown = DOTween.ToAlpha(() => description.color, x => description.color = x, 0, animationTime);
        alphaDown.OnComplete(NextStep);
    }
    void OnRetryButton()
    {
        if (alphaUp != null && alphaUp.active)
            DOTween.Pause(alphaUp);
        alphaDown = DOTween.ToAlpha(() => description.color, x => description.color = x, 0, animationTime);
        alphaDown.OnComplete(Retry);
    }
    void NextStep()
    {
        alphaUp = DOTween.ToAlpha(() => description.color, x => description.color = x, 1, animationTime);
        SetState(step + 1);
    }
    void Retry()
    {
        isCalibrated = false;
        retryButton.gameObject.SetActive(false);
        alphaUp = DOTween.ToAlpha(() => description.color, x => description.color = x, 1, animationTime);
        SetState(Steps.WAIT_LIGHT);
    }
    void RecordDark()
    {
        GameManager.instance.RecordDark();
        alphaDown = DOTween.ToAlpha(() => description.color, x => description.color = x, 0, animationTime);
        alphaDown.OnComplete(NextStep);
    }
    void RecordLight()
    {
        GameManager.instance.RecordLight();
        alphaDown = DOTween.ToAlpha(() => description.color, x => description.color = x, 0, animationTime);
        alphaDown.OnComplete(NextStep);
    }
    void UpdateLight()
    {
        indicatorMaterial.SetFloat("_Fac", GameManager.instance.lightFac);
    }
    void SetState(Steps step)
    {
        alphaUp = DOTween.ToAlpha(() => description.color, x => description.color = x, 1, animationTime);
        this.step = step;

        switch(step)
        {
            case Steps.INIT:

                retryButton.gameObject.SetActive(false);
                buttonText.text = "Continue";
                description.text = "Spirit Box uses your\nphones light-sensor.\nFor the best experience please complete a short calibration process ";
                //+
                //    "before you start playing to ensure you get full experience from the game.\nPress \"Next\" when ready!";
                break;

            case Steps.WAIT_LIGHT:

                indicatorMaterial.SetFloat("_Fac", 1);
                description.text = "First, turn on the lights in your room and press \"Next\".\nMake sure your light source is directly above the screen.";
                buttonText.text = "Continue";
                break;

            case Steps.PROCESS_LIGHT:

                description.text = "Collecting light energy for the ghosts...";
                button.enabled = false;
                Invoke(nameof(RecordLight), 1);
                break;

            case Steps.WAIT_DARK:

                indicatorMaterial.SetFloat("_Fac", 0);
                button.enabled = true;
                description.text = "Done! Now turn off the light in your room and press \"Continue\".";
                    //"\n(don't worry, ghosts are not ready yet)";
                buttonText.text = "Continue";
                break;

            case Steps.PROCESS_DARK:

                description.text = "Luring dark spirits to your device...";
                button.enabled = false;
                Invoke(nameof(RecordDark), 1);
                break;

            case Steps.CHECK:

                isCalibrated = true;
                button.enabled = true;
                retryButton.gameObject.SetActive(true);
                description.text = "Great! Now try turning your lights on and off to see if you are happy with this setting.\nPress \"Retry\" if you are not satisfied";
                break;

            case Steps.COMPLETE:

                retryButton.gameObject.SetActive(false);
                buttonText.text = "Finish";
                description.text = "The ghosts are ready! Press \"Finish\" to start the game";
                
                
                break;

            case Steps.NEXTSCENE:

                SceneManager.LoadScene(2);
                break;

        }
    }
}
