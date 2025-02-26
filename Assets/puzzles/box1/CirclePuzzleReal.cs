using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CirclePuzzleReal : MonoBehaviour
{
    [SerializeField] private GameObject[] Wheels;

    private int chosenItem = 0;
    private bool isWheelOneInCorrectPosition = false;
    private bool isWheelTwoInCorrectPosition = false;
    private bool circlePuzzleIsSolved = false;
    public int offset = 1;



    private bool puzzleIsSolved = false;

    [SerializeField] private bool[] isSymbolInCorrectPosition;
    [SerializeField] private Material[] materials;
    [SerializeField] private Renderer[] symbolPositions;
    [SerializeField] private int[] currentColor;

    [SerializeField] private int[] correctColor;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Wheels[1].transform.localRotation.y);
        if (circlePuzzleIsSolved == false)
        {
            TurnWheel();
            CheckIfWheelsAreInCorrectPosition();
        }

        if (circlePuzzleIsSolved == true && puzzleIsSolved == false)
        {
            ChangeSymbol();
            CheckIfColorsAreCorrect();
        }
        ChooseWheel();
        CheckIfPuzzleIsSolved();
        MakeSureWheelDoesNotGoOutOfBounds();

        checkIfPuzzleIsSolved();

        Debug.Log(isSymbolInCorrectPosition[0]);

        //Wheels[1].transform.rotation = Quaternion.Lerp(Wheels[1].transform.localRotation, 0,0,0, Time.time * 1);

    }

    private void TurnWheel()
    {
        if (Input.GetKey("left"))
        {
            Debug.Log("left held");
            Wheels[chosenItem].transform.Rotate(0, -1, 0);
        }

        if (Input.GetKey("right"))
        {
            Debug.Log("right held");
            Wheels[chosenItem].transform.Rotate(0, 1, 0);
        }
    }

    private void ChooseWheel()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            chosenItem++;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            chosenItem--;
        }

    }

    private void ChangeSymbol()
    {
        if (Input.GetKeyDown("right"))
        {
            currentColor[chosenItem] = currentColor[chosenItem] + 1;
            symbolPositions[chosenItem].material.CopyPropertiesFromMaterial(materials[currentColor[chosenItem]]);

        }

        if (Input.GetKeyDown("left"))
        {
            currentColor[chosenItem] = currentColor[chosenItem] - 1;
            symbolPositions[chosenItem].material.CopyPropertiesFromMaterial(materials[currentColor[chosenItem]]);

        }
    }

    private void CheckIfWheelsAreInCorrectPosition()
    {
        if (Wheels[1].transform.localRotation.y * Mathf.Rad2Deg <= offset && Wheels[1].transform.localRotation.y * Mathf.Rad2Deg >= -offset)
        {
            isWheelOneInCorrectPosition = true;
            Debug.Log("wheel is good");
        }
        else isWheelOneInCorrectPosition = false;

        if (Wheels[2].transform.localRotation.y * Mathf.Rad2Deg <= offset && Wheels[2].transform.localRotation.y * Mathf.Rad2Deg >= -offset)
        {
            isWheelTwoInCorrectPosition = true;
        }
        else isWheelTwoInCorrectPosition = false;
    }

    private void CheckIfPuzzleIsSolved()
    {
        if (isWheelOneInCorrectPosition && isWheelTwoInCorrectPosition)
        {
            Debug.Log("solved");
            circlePuzzleIsSolved = true;
        }
    }

    private void MakeSureWheelDoesNotGoOutOfBounds()
    {
        if (chosenItem == Wheels.Length)
        {
            chosenItem = 0;
        }
        if (chosenItem == -1)
        {
            chosenItem = Wheels.Length - 1;
        }

        for (int i = 0; i < currentColor.Length; i++)
        {
            if (currentColor[i] == materials.Length)
            {
                currentColor[i] = 0;
                symbolPositions[chosenItem].material.CopyPropertiesFromMaterial(materials[currentColor[chosenItem]]);
            }
            if (currentColor[i] == -1)
            {
                currentColor[i] = materials.Length - 1;
                symbolPositions[chosenItem].material.CopyPropertiesFromMaterial(materials[currentColor[chosenItem]]);
            }
        }

    }

    private void CheckIfColorsAreCorrect()
    {
        for (int i = 0; i < isSymbolInCorrectPosition.Length; i++)
        {
            if (currentColor[i] == correctColor[i])
            {
                isSymbolInCorrectPosition[i] = true;
            }
            else isSymbolInCorrectPosition[i] = false;
        }
    }

    private void checkIfPuzzleIsSolved()
    {

        if (isSymbolInCorrectPosition[0] == true && isSymbolInCorrectPosition[1] == true && isSymbolInCorrectPosition[2] == true)
        {
            puzzleIsSolved = true;
            Debug.Log("you won!!");

        }


    }
}
