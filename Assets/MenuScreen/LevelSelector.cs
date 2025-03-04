using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    void Start()
    {
        totalLevels = contentPanel.childCount;
        UpdatePosition();
        targetRotation = contentPanel.GetChild(currentLevelIndex).transform.rotation;
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
                PuzzleDescription.text = "puzzle 1 discription";
                break;
            case 1:
                PuzzleDescription.text = "puzzle 2 discription";
                break;
            case 2:
                PuzzleDescription.text = "puzzle 3 discription";
                break;
        }
    }

    public void LeftButtonPressed()
    {
        currentLevelIndex--;
        StartCoroutine(SmoothMove(contentPanel.anchoredPosition, new Vector2(-currentLevelIndex * spacing, 0)));
    }

   public void RightButtonPressed()
    {
        currentLevelIndex++;
        StartCoroutine(SmoothMove(contentPanel.anchoredPosition, new Vector2(-currentLevelIndex * spacing, 0)));
    }

    void KeepCurrentLevelIndexInBounds()
    {
        if (currentLevelIndex < 0)
        {
            currentLevelIndex = totalLevels -2;
            StartCoroutine(SmoothMove(contentPanel.anchoredPosition, new Vector2(-currentLevelIndex * spacing, 0)));
        }

        if (currentLevelIndex > totalLevels -2)
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

    private void StartPressed()
    {
        SceneManager.LoadScene(currentLevelIndex);
    }
}
