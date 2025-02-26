using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
public class CirclePuzzle : MonoBehaviour
{
    [SerializeField] private GameObject[] Wheels;
    private int chosenWheel = 0;
    private bool isWheelOneInCorrectPosition = false;
    private bool isWheelTwoInCorrectPosition = false;
    private bool puzzleIsSolved = false;

    private ProjectInnovation controls;
    private float turnDirection = 0f;
    private float turnSpeed = 50f;

    public string currentWheel;
    Vector3 acceleration;


    private void Awake()
    {
        controls = new ProjectInnovation();

        controls.Turn.Left.performed += ctx => StartTurning(1);
        controls.Turn.Left.canceled += ctx => StopTurning();
        controls.Turn.Right.performed += ctx => StartTurning(-1);
        controls.Turn.Right.canceled += ctx => StopTurning();
    }

    private void OnEnable()
    {
        controls.Turn.Enable();
    }

    private void OnDisable()
    {
        controls.Turn.Disable();
    }

    private void Update()
    {
        if (!puzzleIsSolved)
        {
            CheckIfPuzzleIsSolved();
            CheckIfWheelsAreInCorrectPosition();
            MakeSureWheelDoesNotGoOutOfBounds();
            CheckWhichWheelIsSelected();
        }



        
            


    }

    private void StartTurning(int direction)
    {
        turnDirection = direction;
    }

    private void StopTurning()
    {
        turnDirection = 0;
    }

    private void ChangeWheel(int direction)
    {
        chosenWheel += direction;
    }

    private void CheckIfWheelsAreInCorrectPosition()
    {
        isWheelOneInCorrectPosition = Mathf.Abs(Wheels[1].transform.localRotation.y * Mathf.Rad2Deg) <= 2;
        isWheelTwoInCorrectPosition = Mathf.Abs(Wheels[2].transform.localRotation.y * Mathf.Rad2Deg) <= 2;
    }

    private void CheckIfPuzzleIsSolved()
    {
        if (isWheelOneInCorrectPosition && isWheelTwoInCorrectPosition)
        {
            Debug.Log("solved");
            puzzleIsSolved = true;
        }
    }

    private void MakeSureWheelDoesNotGoOutOfBounds()
    {
        if (chosenWheel >= Wheels.Length)
            chosenWheel = 0;
        if (chosenWheel < 0)
            chosenWheel = Wheels.Length - 1;
    }

    private void CheckWhichWheelIsSelected()
    {
        if (InputManager.Instance != null && InputManager.Instance.hit.collider != null)
        {
            GameObject hitObject = InputManager.Instance.hit.collider.gameObject;
            currentWheel = hitObject.name;

            int index = System.Array.IndexOf(Wheels, hitObject);
            if (index >= 0)
            {
                chosenWheel = index;
            }
        }
        else
        {
            currentWheel = "";
        }

        if (turnDirection != 0)
        {
            Wheels[chosenWheel].transform.Rotate(0, turnDirection * turnSpeed * Time.deltaTime, 0);
        }
    }
}
