using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor;
using System;



#if UNITY_EDITOR
using Gyroscope = UnityEngine.InputSystem.Gyroscope;
using Accelerometer = UnityEngine.InputSystem.Accelerometer;
using LightSensor = UnityEngine.InputSystem.LightSensor;
using InputDevice = UnityEngine.InputSystem.InputDevice;

#else
using InputDevice = UnityEngine.InputSystem.InputDevice;
using Accelerometer = UnityEngine.InputSystem.Android.AndroidAccelerometer;
using Gyroscope = UnityEngine.InputSystem.Android.AndroidGyroscope;
using LightSensor = UnityEngine.InputSystem.Android.AndroidLightSensor;
#endif

public class SensorInput : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI accelerometerInfo;
    [SerializeField]
    TextMeshProUGUI gyroscopeInfo;
    [SerializeField]
    TextMeshProUGUI lightSensorInfo;
    [SerializeField]
    TextMeshProUGUI sensorInfo;

    //INPUT:
    // Accelerometer: OK
    // Gyroscope: OK
    // LightSensor: NO SUPPORT
    //INPUT SYSTEM:
    // Accelerometer: OK
    // Gyroscope: FAIL
    // LightSensor: NO IMPLEMENTATION
    //

    bool accelerometerFound = false;
    bool gyroscopeFound = false;
    bool lightSensorFound = false;
    // Start is called before the first frame update

    [Obsolete("Use input system instead of input")]
    void InputInitGyroscope()
    {
        Input.gyro.enabled = true;
    }

    [Obsolete("Use input system instead of input")]
    void InputGyroscope()
    {
        Debug.Log("Gyroscope: " + Input.gyro.enabled);

        if (Input.gyro.enabled)
            gyroscopeInfo.text = "Gyroscope: " + Input.gyro.rotationRate;
    }

    [Obsolete("Use input system instead of input")]
    void InputAccelerometer()
    {
        Debug.Log("Gyroscope: " + Input.gyro.enabled);

        accelerometerInfo.text = "Accelerometer: " + Input.acceleration;
    }
    void Start()
    {
    }
    private void OnEnable()
    {
        Invoke(nameof(InputSystemFindDevices),0.01f);

        Invoke(nameof(InputSystemInitAccelerometer), 0.02f);
        Invoke(nameof(InputSystemInitGyroscope), 0.02f);

#if !UNITY_EDITOR
        if (lightSensorFound)
            Invoke(nameof(InputSystemInitLightSensor), 0.02f);
#endif
    }

    void InputSystemFindDevices()
    {
#if UNITY_EDITOR
        foreach (var dev in InputSystem.devices)
        {
            if (dev.layout == "Accelerometer")
                accelerometerFound = true;

            if (dev.layout == "Gyroscope")
                gyroscopeFound = true;
        }
#else
        foreach (var dev in InputSystem.devices)
        {
            if (dev.layout == "AndroidAccelerometer")
                accelerometerFound = true;

            if (dev.layout == "AndroidGyroscope")
                gyroscopeFound = true;

            if (dev.layout == "AndroidLightSensor")
                lightSensorFound = true;
        }
#endif

        if (!gyroscopeFound)
            gyroscopeInfo.text = "Gyroscope: not found";
        if (!accelerometerFound)
            accelerometerInfo.text = "Accelerometer: not found";
        if (!lightSensorFound)
            lightSensorInfo.text = "LightSensor: not found";
    }
    void Update()
    {
            InputSystemAccelerometer();

            InputSystemGyroscope();

#if !UNITY_EDITOR
        InputSystemLightSensor();
#endif
        //listing all sensors
        string sensors = "";
        foreach (InputDevice device in InputSystem.devices)
        {
            sensors += device.deviceId + "|";
            sensors += device.GetType() + "|";
            sensors += device.layout + "|";
            sensors += device.name + "|";
            sensors += '\n';
        }

        sensorInfo.text = sensors;
    }

    void InputSystemInitAccelerometer()
    {
        InputSystem.EnableDevice(Accelerometer.current);
    }

    void InputSystemAccelerometer()
    {
        if (!accelerometerFound) return;

        Debug.Log("Accelerometer: " + Accelerometer.current.enabled);

        if (Accelerometer.current.enabled)
            accelerometerInfo.text = "Accelerometer: " + Accelerometer.current.acceleration.ReadValue();
    }

    void InputSystemInitGyroscope()
    {
        InputSystem.EnableDevice(Gyroscope.current);
    }
    void InputSystemGyroscope()
    {
        if (!gyroscopeFound) return;

        InputSystem.EnableDevice(Gyroscope.current);
        Debug.Log("Gyroscope: " + Gyroscope.current.enabled);

        if (Gyroscope.current.enabled)
            gyroscopeInfo.text = "Gyroscope: " + Gyroscope.current.angularVelocity.ReadValue();
    }

    public void SwitchInputMethod()
    {
        //TO BE IMPLEMENTED
    }
    void InputSystemInitLightSensor()
    {
        InputSystem.EnableDevice(LightSensor.current);
    }
    void InputSystemLightSensor()
    {
        if (!lightSensorFound) return;

        InputSystem.EnableDevice(LightSensor.current);
        Debug.Log("LightSensor: " + LightSensor.current.enabled);

        if (LightSensor.current.enabled)
            lightSensorInfo.text = "LightSensor: " + LightSensor.current.lightLevel.ReadValue();
    }
}
