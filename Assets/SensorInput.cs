using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.Controls;
using System.Linq;
using System.IO;







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

public interface IControlInfo
{
    public string name { get; }
    
}
public struct ControlInfo<T> : IControlInfo where T : struct
{
    public string name;
    public InputControl<T> control;
    public T value;
    string IControlInfo.name => name;

    public ControlInfo (string name, InputControl<T> control)
    {
        this.name = name;
        this.control = control;
        value = control.ReadValue();
    }
}

public struct ControlToDisplay
{
    public string name;
    public TextMeshProUGUI display;
}

public class SensorInput : MonoBehaviour
{
    [SerializeField]
    List<ControlToDisplay> controlsToDisplays;
    public Dictionary<string, TextMeshProUGUI> dataLayout => 
        controlsToDisplays.ToDictionary(field => field.name, field => field.display);

    [SerializeField]
    TextMeshProUGUI accelerometerInfo;
    [SerializeField]
    TextMeshProUGUI gyroscopeInfo;
    [SerializeField]
    TextMeshProUGUI lightSensorInfo;
    [SerializeField]
    TextMeshProUGUI sensorInfo;


    bool accelerometerFound = false;
    bool gyroscopeFound = false;
    bool lightSensorFound = false;

    Vector3 acceleration;
    Vector3 angularVelocity;
    float lightLevel;

    [SerializeField]
    float sensorDetectionDelay = 0.01f; 

    int currentDeviceId = 0;

    private void OnEnable()
    {
        Invoke(nameof(InputSystemFindDevices), sensorDetectionDelay);
#if !UNITY_EDITOR
        sensorInfo.text = 
        Application.dataPath + "\n" + 
        Application.persistentDataPath;
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

        Invoke(nameof(InputSystemInitAccelerometer), Time.deltaTime);
        Invoke(nameof(InputSystemInitGyroscope), Time.deltaTime);


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
        
        Invoke(nameof(InputSystemInitAccelerometer), Time.deltaTime);
        Invoke(nameof(InputSystemInitGyroscope), Time.deltaTime);
        Invoke(nameof(InputSystemInitLightSensor), Time.deltaTime);

        foreach (InputDevice device in InputSystem.devices)
        {
            string sensors = "";
            foreach (InputControl ic in device.children)
            {
                WriteChildrenControls(ic, ref sensors, 1);
            }
            sensors += '\n';

            string path = Application.persistentDataPath + "/" + device.layout + ".txt";
            File.WriteAllText(path, sensors);

            sensorInfo.text = path;

        }

#endif
        if (!gyroscopeFound && gyroscopeInfo != null)
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

#if UNITY_EDITOR
        DisplaySensorInfo();
#else
        InputSystemLightSensor();
#endif
    }

    void DisplaySensorInfo()
    {
        if (sensorInfo == null) return;

        //listing all sensors
        string sensors = "";

        InputSystem.EnableDevice(Keyboard.current);

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            Debug.Log("aKey");
            currentDeviceId--;
            if (currentDeviceId < 0)
                currentDeviceId = InputSystem.devices.Count - 1;
        }
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            Debug.Log("dKey");
            currentDeviceId++;
            if (currentDeviceId >= InputSystem.devices.Count)
                currentDeviceId = 0;
        }

        var device = InputSystem.devices[currentDeviceId];

        sensors += device.deviceId + "  ";
        sensors += device.layout + ":  \n";
        sensors += (device.enabled ? "ENABLED" : "DISABLED") + "\n\n";

        foreach (InputControl ic in device.children)
        {
            WriteChildrenControls(ic, ref sensors, 1);
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            string path = Application.dataPath + "/device info";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path += "/" + device.layout + ".txt";
            File.WriteAllText(path, sensors);
            Debug.Log("Log saved to: " + path);
        }
        sensors += '\n';

        sensorInfo.text = sensors;
    }

    void WriteChildrenControls(InputControl control, ref string result, int it)
    {
        for (int i = 0; i < it; i++)
            result += "\t";

        result += "<" + control.valueType + "> " + control.displayName;

        if (control.children.Count == 0)
        {
            result += " = " + control.ReadValueAsObject() + '\n';
            return;
        }

        result += '\n';

        foreach (var child in control.children)
        {
            WriteChildrenControls(child, ref result, it + 1);
        }
        return;
    }
    void InputSystemInitAccelerometer()
    {
        if (!accelerometerFound) return;
        InputSystem.EnableDevice(Accelerometer.current);
    }

    void InputSystemAccelerometer()
    {
        if (!accelerometerFound) return;

        Debug.Log("Accelerometer: " + Accelerometer.current.enabled);

        if (Accelerometer.current.enabled)
            acceleration = Accelerometer.current.acceleration.ReadValue();

        if (accelerometerInfo != null)
            accelerometerInfo.text = "Accelerometer: " + acceleration;
    }

    void InputSystemInitGyroscope()
    {
        if (!gyroscopeFound) return;
        InputSystem.EnableDevice(Gyroscope.current);
    }
    void InputSystemGyroscope()
    {
        if (!gyroscopeFound) return;

        InputSystem.EnableDevice(Gyroscope.current);
        Debug.Log("Gyroscope: " + Gyroscope.current.enabled);

        if (Gyroscope.current.enabled)
            angularVelocity = Gyroscope.current.angularVelocity.ReadValue();

        if (gyroscopeInfo != null)
            gyroscopeInfo.text = "Gyroscope: " + Gyroscope.current.angularVelocity.ReadValue();
    }
    void InputSystemInitLightSensor()
    {
        if (!lightSensorFound) return;
        InputSystem.EnableDevice(LightSensor.current);
    }
    void InputSystemLightSensor()
    {
        if (!lightSensorFound) return;

        InputSystem.EnableDevice(LightSensor.current);
        Debug.Log("LightSensor: " + LightSensor.current.enabled);

        if (LightSensor.current.enabled)
            lightLevel = LightSensor.current.lightLevel.ReadValue();

        if (lightSensorInfo != null)
            lightSensorInfo.text = "LightSensor: " + LightSensor.current.lightLevel.ReadValue();
    }




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
}
