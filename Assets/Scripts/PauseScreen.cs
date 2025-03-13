using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] 
    private GameObject pauseScreen;
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pauseButtonGameObject;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button RecalibrateButton;

    // Start is called before the first frame update
    void Start()
    {
        pauseScreen.SetActive(false);
        RecalibrateButton.onClick.AddListener(RecalibratePressed);
        pauseButton.onClick.AddListener(PausePressed);
        continueButton.onClick.AddListener(ContinuePressed);
        resetButton.onClick.AddListener(ResetPuzzlePressed);
        quitButton.onClick.AddListener(QuitPressed);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RecalibratePressed()
    {
        SceneManager.LoadScene("Calibration");
    }

    void PausePressed()
    {
        pauseScreen.SetActive(true);
        pauseButtonGameObject.SetActive(false);
    }

    void ContinuePressed()
    {
        pauseScreen.SetActive(false);
        pauseButtonGameObject.SetActive(true);
    }

    void ResetPuzzlePressed()
    {
        SceneManager.LoadScene(GameManager.instance.selectedLevelName);
    }

    void QuitPressed()
    {
        SceneManager.LoadScene("MenuScreen");
    }
}
