using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CirclePuzzle : MonoBehaviour
{
    [SerializeField] private GameObject[] Wheels;

    private int chosenWheel = 0;
    private bool isWheelOneInCorrectPosition = false;
    private bool isWheelTwoInCorrectPosition = false;
    private bool puzzleIsSolved = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Wheels[1].transform.localRotation.y);
        if (puzzleIsSolved == false)
        {
            TurnWheel();
            ChooseWheel();
        }

        CheckIfPuzzleIsSolved();
        CheckIfWheelsAreInCorrectPosition();
        MakeSureWheelDoesNotGoOutOfBounds();

        //Wheels[1].transform.rotation = Quaternion.Lerp(Wheels[1].transform.localRotation, 0,0,0, Time.time * 1);

    }

   private void TurnWheel()
    {
        if (Input.GetKey("left"))
        {
            Debug.Log("left held");
            Wheels[chosenWheel].transform.Rotate(0, -1, 0);
        }

        if (Input.GetKey("right"))
        {
            Debug.Log("right held");
            Wheels[chosenWheel].transform.Rotate(0, 1, 0);
        }
    }

   private void ChooseWheel()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            chosenWheel++;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            chosenWheel--;
        }

    }

    private void CheckIfWheelsAreInCorrectPosition()
    {
        if (Wheels[1].transform.localRotation.y * Mathf.Rad2Deg <= 2 && Wheels[1].transform.localRotation.y * Mathf.Rad2Deg >= -2)
        {
            isWheelOneInCorrectPosition = true;
            Debug.Log("wheel is good");
        }
        else isWheelOneInCorrectPosition = false;

        if (Wheels[2].transform.localRotation.y * Mathf.Rad2Deg <= 2 && Wheels[2].transform.localRotation.y * Mathf.Rad2Deg >= -2)
        {
            isWheelTwoInCorrectPosition = true;
        }
        else isWheelTwoInCorrectPosition = false;
    }

    private void CheckIfPuzzleIsSolved()
    {
        if(isWheelOneInCorrectPosition && isWheelTwoInCorrectPosition)
        {
            Debug.Log("solved");
            puzzleIsSolved = true;
        }
    }

    private void MakeSureWheelDoesNotGoOutOfBounds()
    {
        if (chosenWheel == Wheels.Length)
        {
            chosenWheel = 0;
        }
        if (chosenWheel == -1)
        {
            chosenWheel = Wheels.Length-1;
        }
    }
}
