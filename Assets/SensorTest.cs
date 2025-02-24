using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class SensorTest : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI accelerometerInfo;
    [SerializeField]
    TextMeshProUGUI gyroscopeInfo;
    [SerializeField]
    TextMeshProUGUI lightSensorInfo;
    LightSensor lightSensor;
    // Start is called before the first frame update
    void Start()
    {
        lightSensor = InputSystem.AddDevice<LightSensor>("light sensor");
        InputSystem.EnableDevice(lightSensor);
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        accelerometerInfo.text = "Accelerometer: " + Accelerometer.current.acceleration;
        gyroscopeInfo.text = "Gyroscope: " + UnityEngine.InputSystem.Gyroscope.current.magnitude;
        lightSensorInfo.text = "Light: " + LightSensor.current.lightLevel.magnitude;
    }
}
