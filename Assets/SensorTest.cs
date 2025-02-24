using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.XR;

using Gyroscope = UnityEngine.InputSystem.Gyroscope;
public class SensorTest : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI accelerometerInfo;
    [SerializeField]
    TextMeshProUGUI gyroscopeInfo;
    [SerializeField]
    TextMeshProUGUI lightSensorInfo;

    //INPUT:
    // Accelerometer: OK
    // Gyroscope: OK
    // LightSensor: NO SUPPORT
    //INPUT SYSTEM:
    // Accelerometer: OK
    // Gyroscope: FAIL
    // LightSensor: NO IMPLEMENTATION
    //
    public enum InputType
    {
        InputSystem,
        Input
    }

    public InputType accelerometerInputType = InputType.Input;
    public InputType gyroscopeInputType = InputType.Input;
    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnEnable()
    {
        InputSystem.EnableDevice(Accelerometer.current);
        if (accelerometerInputType == InputType.Input)
            ;
        else if (accelerometerInputType == InputType.InputSystem)
            InputSystemInitAccelerometer();

        if (gyroscopeInputType == InputType.Input)
            //a bit of delay cuz otherwise wont work
            Invoke(nameof(InputInitGyroscope),0.01f);
        else if (gyroscopeInputType == InputType.InputSystem)
            InputSystemInitGyroscope();
    }
    void Update()
    {
        if (accelerometerInputType == InputType.Input)
            InputAccelerometer();
        else if (accelerometerInputType == InputType.InputSystem)
            InputSystemAccelerometer();

        if (gyroscopeInputType == InputType.Input)
            InputGyroscope();
        else if (gyroscopeInputType == InputType.InputSystem)
            InputSystemGyroscope();
    }

    void InputAccelerometer()
    {
        Debug.Log("Gyroscope: " + Input.gyro.enabled);

        if (Accelerometer.current.enabled)
            accelerometerInfo.text = "Accelerometer: " + Input.acceleration;
    }

    void InputSystemInitAccelerometer()
    {
        InputSystem.EnableDevice(Accelerometer.current);
    }

    void InputSystemAccelerometer()
    {
        Debug.Log("Accelerometer: " + Accelerometer.current.enabled);

        if (Accelerometer.current.enabled)
            accelerometerInfo.text = "Accelerometer: " + Accelerometer.current.acceleration.ReadValue();
    }
    void InputInitGyroscope()
    {
        Input.gyro.enabled = true;
    }
    //NOT WORKING
    void InputSystemInitGyroscope()
    {
        InputSystem.EnableDevice(Gyroscope.current);
    }

    //NOT WORKING
    void InputGyroscope()
    {
        Debug.Log("Gyroscope: " + Input.gyro.enabled);

        if (Input.gyro.enabled)
            gyroscopeInfo.text = "Gyroscope: " + Input.gyro.rotationRate;
    }
    void InputSystemGyroscope()
    {
        Debug.Log("Gyroscope: " + Gyroscope.current.enabled);

        if (Gyroscope.current.enabled)
            gyroscopeInfo.text = "Gyroscope: " + Gyroscope.current.angularVelocity.ReadValue();
    }

    public void SwitchInputMethod()
    {   
        //TO BE IMPLEMENTED
    }
}
