using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
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
    private void Start()
    {
        button.onClick.AddListener(NextStep);
        retryButton.onClick.AddListener(Retry);
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        SetState(0);
    }

    private void Update()
    {
        if (isCalibrated)
            UpdateLight();
    }
    void NextStep()
    {
        SetState(step + 1);
    }
    void RecordDark()
    {
        GameManager.instance.RecordDark();
        NextStep();
    }
    void RecordLight()
    {
        GameManager.instance.RecordLight();
        NextStep();
    }
    void UpdateLight()
    {
        indicatorMaterial.SetFloat("_Fac", GameManager.instance.lightFac);
    }
    void Retry()
    {
        isCalibrated = false;
        retryButton.gameObject.SetActive(false);
        SetState(Steps.WAIT_LIGHT);
    }
    void SetState(Steps step)
    {
        this.step = step;

        switch(step)
        {
            case Steps.INIT:

                retryButton.gameObject.SetActive(false);
                buttonText.text = "Next";
                description.text = "Welcome to Ghost Box!\nPlease complete a short calibration process " +
                    "before you start playing to ensure you get full experience from the game.\nPress \"Next\" when ready!";
                break;

            case Steps.WAIT_LIGHT:

                indicatorMaterial.SetFloat("_Fac", 1);
                description.text = "First, turn on the light and press \"Next\".\nMake sure your light source is directly above the screen.";
                buttonText.text = "Next";
                break;

            case Steps.PROCESS_LIGHT:

                description.text = "Collecting light energy for the ghosts...";
                button.enabled = false;
                Invoke(nameof(RecordLight), 1);
                break;

            case Steps.WAIT_DARK:

                indicatorMaterial.SetFloat("_Fac", 0);
                button.enabled = true;
                description.text = "Done! Now turn off the light and press \"Next\".\n(don't worry, ghosts are not ready yet)";
                buttonText.text = "Next";
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
                description.text = "Great! Now try turning the light on and off to see if you are happy with this setting.\nPress \"Next\" to continue.\nPress \"Retry\" if you are not satisfied";
                break;

            case Steps.COMPLETE:

                retryButton.gameObject.SetActive(false);
                buttonText.text = "Finish";
                description.text = "The ghosts are ready! Press \"Finish\" to start the game";
                break;

        }
    }
}
