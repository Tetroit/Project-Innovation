using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{

    [SerializeField] GameObject endMenu;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private TextMeshProUGUI puzzleSolved;
    [SerializeField] private string puzzleName;


    // Start is called before the first frame update
    void Start()
    {
        endMenu.SetActive(false);
        continueButton.onClick.AddListener(ContinuePressed);
        resetButton.onClick.AddListener(ResetPuzzlePressed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimationEnded()
    {
        endMenu.SetActive(true);
        puzzleSolved.text = puzzleName + " exorcized";
    }

    private void ContinuePressed()
    {
        SceneManager.LoadScene("MenuScreen");
    }

    private void ResetPuzzlePressed()
    {
        SceneManager.LoadScene(GameManager.instance.selectedLevelName);
    }
}
