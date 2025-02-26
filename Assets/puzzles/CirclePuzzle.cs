using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CirclePuzzle : MonoBehaviour
{
    [SerializeField] private GameObject[] Wheels;
    private int chosenWheel = 0;
    private bool isWheelOneInCorrectPosition = false;
    private bool isWheelTwoInCorrectPosition = false;
    private bool puzzleIsSolved = false;

    private ProjectInnovation controls;

    private void Awake()
    {
        controls = new ProjectInnovation();

        controls.Turn.Left.performed += ctx => TurnWheel(-1);
        //controls.Sensors.Click.performed += ctx => TouchScreenSelect();
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
        Debug.Log(Wheels[1].transform.localRotation.y);
        if (!puzzleIsSolved)
        {
            CheckIfPuzzleIsSolved();
            CheckIfWheelsAreInCorrectPosition();
            MakeSureWheelDoesNotGoOutOfBounds();
        }
    }

    private void TurnWheel(int direction)
    {
        Wheels[chosenWheel].transform.Rotate(0, direction, 0);
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

}
