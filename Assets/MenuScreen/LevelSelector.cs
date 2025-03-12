using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public RectTransform contentPanel;
    public float snapSpeed = 10f;
    public float swipeThreshold = 50f;
    public float spacing = 500f; // adjust based on your images' width + spacing
    public TMP_Text PuzzleDescription;

    private Vector2 startTouchPos, endTouchPos;
    private int currentLevelIndex = 0;
    private int totalLevels;

    private Quaternion targetRotation;
    private GameObject[] PuzzleBoxes;
    public Vector3 rotationSpeed = new Vector3(30f, 30f, 0f); // Adjust speeds for X and Y

    [SerializeField] private GameObject startButtonImage;
    [SerializeField] private TextMeshProUGUI startButtonText;

    [SerializeField] private GameObject recalibrateButton;

    private bool canStart = true;
    private bool DoOnce = false;

    void Start()
    {
        totalLevels = contentPanel.childCount;
        UpdatePosition();
        targetRotation = contentPanel.GetChild(currentLevelIndex).transform.rotation;
        CheckIfEligableForRecalibration();
    }

    void Update()
    {


        // For Touch Input
        if (Touchscreen.current != null)
        {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                startTouchPos = Touchscreen.current.primaryTouch.position.ReadValue();
                Debug.Log("Touch started: " + startTouchPos);
            }
            if (Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
            {
                endTouchPos = Touchscreen.current.primaryTouch.position.ReadValue();
                Debug.Log("Touch released: " + endTouchPos);
                HandleSwipe(endTouchPos - startTouchPos);
            }
        }

        // For Mouse Input (useful for testing in Editor)
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                startTouchPos = Mouse.current.position.ReadValue();
                Debug.Log("Mouse click started: " + startTouchPos);
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                endTouchPos = Mouse.current.position.ReadValue();
                Debug.Log("Mouse click released: " + endTouchPos);
                HandleSwipe(endTouchPos - startTouchPos);
            }
        }

        PuzzleDiscriptionChanger();
        KeepCurrentLevelIndexInBounds();
        RotateCubes();
        GrayOutIncompleteLevels();
    }

    void HandleSwipe(Vector2 swipeDelta)
    {
        if (Mathf.Abs(swipeDelta.x) > swipeThreshold)
        {
            if (swipeDelta.x > 0)
            {
                // Swipe right: show previous level
                currentLevelIndex = currentLevelIndex - 1;
                Debug.Log("Swiped right. New index: " + currentLevelIndex);
            }
            else
            {
                // Swipe left: show next level
                currentLevelIndex = currentLevelIndex + 1;
                Debug.Log("Swiped left. New index: " + currentLevelIndex);
            }
            StartCoroutine(SmoothMove(contentPanel.anchoredPosition, new Vector2(-currentLevelIndex * spacing, 0)));
            DoOnce = false;
        }
    }

    System.Collections.IEnumerator SmoothMove(Vector2 startPos, Vector2 targetPos)
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * snapSpeed;
            contentPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, time);
            yield return null;
        }
        contentPanel.anchoredPosition = targetPos;
    }

    void UpdatePosition()
    {
        contentPanel.anchoredPosition = new Vector2(-currentLevelIndex * spacing, 0);
    }

    void PuzzleDiscriptionChanger()
    {
        switch (currentLevelIndex)
        {
            case 0:
                if (DoOnce == false)
                {
                    PuzzleDescription.text = "Scorpio puzzle";
                    GameManager.instance.selectedLevelName = "MainGame";
                    DoOnce = true;
                }
                break;
            case 1:
                if (DoOnce == false)
                {
                    PuzzleDescription.text = "Orrery puzzle";
                    GameManager.instance.selectedLevelName = "SecondPuzzle";
                    DoOnce = true;
                }
                break;
            case 2:
                if (DoOnce == false)
                {
                    PuzzleDescription.text = "Summer triangle puzzle";
                    GameManager.instance.selectedLevelName = "PyramidPuzzle";
                    DoOnce = true;
                }
                break;
        }
    }

    public void LeftButtonPressed()
    {
        currentLevelIndex--;
        StartCoroutine(SmoothMove(contentPanel.anchoredPosition, new Vector2(-currentLevelIndex * spacing, 0)));
        DoOnce = false;
    }

    public void RightButtonPressed()
    {
        currentLevelIndex++;
        StartCoroutine(SmoothMove(contentPanel.anchoredPosition, new Vector2(-currentLevelIndex * spacing, 0)));
        DoOnce = false;
    }

    void KeepCurrentLevelIndexInBounds()
    {
        if (currentLevelIndex < 0)
        {
            currentLevelIndex = totalLevels - 1;
            StartCoroutine(SmoothMove(contentPanel.anchoredPosition, new Vector2(-currentLevelIndex * spacing, 0)));
        }

        if (currentLevelIndex > totalLevels - 1)
        {
            currentLevelIndex = 0;
            StartCoroutine(SmoothMove(contentPanel.anchoredPosition, new Vector2(-currentLevelIndex * spacing, 0)));
        }
    }

    private void RotateCubes()
    {
        //PuzzleBoxes[currentLevelIndex].transform.Rotate(10f, 0f, 0f);
        //contentPanel.GetChild(currentLevelIndex).transform.Rotate(1f, 1f, 1f);

        Quaternion targetRotation = contentPanel.GetChild(currentLevelIndex).transform.rotation * Quaternion.Euler(rotationSpeed * Time.deltaTime);
        contentPanel.GetChild(currentLevelIndex).transform.rotation = Quaternion.RotateTowards(contentPanel.GetChild(currentLevelIndex).transform.rotation, targetRotation, rotationSpeed.magnitude * Time.deltaTime);

    }

    void StartPressed()
    {
        if (canStart == true)
        {
            if (GameManager.instance.isCalibrated)
            {
                SceneManager.LoadScene(GameManager.instance.selectedLevelName);
            }
            else
            {
                SceneManager.LoadScene("Calibration");
            }
        }

    }

    public void RecalibratePressed()
    {
        GameManager.instance.selectedLevelName = "MenuScreen";
        SceneManager.LoadScene("Calibration");
    }

    private void GrayOutIncompleteLevels()
    {
        if (GameManager.instance.selectedLevelName == "PyramidPuzzle")
        {
            //startButtonText.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
            startButtonImage.GetComponent<Image>().color = new Color32(157, 157, 157, 200);
            canStart = false;
        }
        else
        {
            //startButtonText.GetComponent<Image>().color = new Color32(219, 252, 241, 255);
            startButtonImage.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            canStart = true;
        }
    }

    private void CheckIfEligableForRecalibration()
    {
        if (GameManager.instance.isCalibrated)
        {
            recalibrateButton.SetActive(true);
            Debug.Log("true");
        }
        else
        {
            recalibrateButton.SetActive(false);
            Debug.Log("false");
        }
    }
}
