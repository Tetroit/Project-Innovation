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
    [SerializeField] private Button continueButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        //pauseScreen.SetActive(false);

        pauseButton.onClick.AddListener(PausePressed);
        continueButton.onClick.AddListener(PausePressed);
        resetButton.onClick.AddListener(ResetPuzzlePressed);
        quitButton.onClick.AddListener(QuitPressed);
    }

    // Update is called once per frame
    void Update()
    {

    }

   public void PausePressed()
    {
        pauseScreen.SetActive(true);
    }

    void ContinuePressed()
    {
        pauseScreen.SetActive(false);
    }

    void ResetPuzzlePressed()
    {
        SceneManager.LoadScene("MainGame");
    }

    void QuitPressed()
    {
        SceneManager.LoadScene("MenuScreen");
    }
}
