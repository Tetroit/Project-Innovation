using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
public class CirclePuzzle : MonoBehaviour
{
    [SerializeField] private GameObject[] Wheels;
    [SerializeField] private GameObject[] Buttons;
    [SerializeField] private Material[] materials;

    [SerializeField] private bool DoOnce;
    private int chosenWheel = 0;
    private int[] materialIndices;
    private bool isWheelOneInCorrectPosition = false;
    private bool isWheelTwoInCorrectPosition = false;
    private bool puzzleIsSolved = false;
    

    [SerializeField]
    Vector3 startRot = new Vector3(0, 0, 0);
    [SerializeField]
    Vector3 endRot = new Vector3(0, 0, 0);
    [SerializeField]
    Quaternion phoneRotation;

    private ProjectInnovation controls;
    private float turnDirection = 0f;
    private float turnSpeed = 50f;

    public string currentWheel;
    Vector3 acceleration;

    bool colorChange = false;

    private void Awake()
    {
        controls = new ProjectInnovation();

        controls.Turn.Left.performed += ctx => StartTurning(-1);
        controls.Turn.Left.canceled += ctx => StopTurning();
        controls.Turn.Right.performed += ctx => StartTurning(1);
        controls.Turn.Right.canceled += ctx => StopTurning();

        materialIndices = new int[Buttons.Length];
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

            CheckIfPuzzleIsSolved();
            CheckIfWheelsAreInCorrectPosition();
            MakeSureWheelDoesNotGoOutOfBounds();
            CheckWhichWheelIsSelected();
        if (!puzzleIsSolved)
        {
            PhoneOrientation();
        }



        // Debug.Log(SensorInput.instance.accelerometer);


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
        isWheelOneInCorrectPosition = Mathf.Abs(Wheels[1].transform.localRotation.z * Mathf.Rad2Deg) <= 2;
        isWheelTwoInCorrectPosition = Mathf.Abs(Wheels[2].transform.localRotation.z * Mathf.Rad2Deg) <= 2;
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
            if (Wheels.Contains(hitObject))
            {
                currentWheel = hitObject.name;

                int index = System.Array.IndexOf(Wheels, hitObject);
                if (index >= 0)
                {
                    chosenWheel = index;
                }
            }
            else if (Buttons.Contains(hitObject))
            {
                if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame && puzzleIsSolved)
                {
                    SetMaterial(hitObject);
                }
            }

        }
        else
        {
            currentWheel = "";
        }

        if (turnDirection != 0 && !puzzleIsSolved)
        {
            Wheels[chosenWheel].transform.Rotate(0, 0, turnDirection * turnSpeed * Time.deltaTime);
        }
    }

    private void PhoneOrientation()
    {
        if (SensorInput.DeviceFound(SensorInput.accelerometerLayout))
        {
            var acc = SensorInput.accelerometer;
            var acceleration = SensorInput.GetControlValue(acc.acceleration);

            Debug.Log(acceleration);

            if (acceleration.x > 0.2)
            {
                Debug.Log("right held");
                Wheels[chosenWheel].transform.Rotate(0, 0, Time.deltaTime * 60);
            }
            if (acceleration.x < -0.2)
            {
                Debug.Log("left held");
                Wheels[chosenWheel].transform.Rotate(0, 0, -Time.deltaTime * 60);
            }
        }
    }

    public void SetMaterial(GameObject hitObject)
    {
        int index = System.Array.IndexOf(Buttons, hitObject);
        Debug.Log("Current index: " + index);

        Renderer renderer = hitObject.GetComponent<Renderer>();
        if (renderer != null && materials.Length > 0)
        {

                materialIndices[index] = (materialIndices[index] + 1) % materials.Length;
                renderer.material = materials[materialIndices[index]];
                DoOnce = true;    
        }
    }
}
