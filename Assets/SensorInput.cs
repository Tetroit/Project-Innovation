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


public class SensorInput : MonoBehaviour
{
    public static SensorInput instance;

    [SerializeField]
    TextMeshProUGUI accelerometerInfo;
    [SerializeField]
    TextMeshProUGUI gyroscopeInfo;
    [SerializeField]
    TextMeshProUGUI lightSensorInfo;
    [SerializeField]
    TextMeshProUGUI sensorInfo;

    public Dictionary<string, InputDevice> devices = new();

    string keyboardLayout = "Keyboard";
    string mouseLayout = "Mouse";
    string penLayout = "Pen";
    string touchscreenLayout = "Touchscreen";

#if UNITY_EDITOR

    string attituteSensorLayout = "AttitudeSensor";
    string gravitySensorLayout = "GravitySensor";
    string linearAccelerationSensorLayout = "LinearAccelerationSensor";
    string accelerometerLayout = "Accelerometer";
    string gyroscopeLayout = "Gyroscope";

    //unavailable on editor
    string lightSensorLayout = "LightSensor";
    string proximitySensorLayout = "ProximitySensor";
    string ambientTemperatureSensorLayout = "AmbientTemperatureSensor";
    string humiditySensorLayout = "HumiditySensor";
    string magneticFieldSensorLayout = "MagneticFieldSensor";
    string pressureSensorLayout = "PressureSensor";
    string stepCounterLayout = "StepCounter";

#elif UNITY_ANDROID

    string attituteSensorLayout = "AndroidRotationVector";
    string gravitySensorLayout = "AndroidGravitySensor";
    string linearAccelerationSensorLayout = "AndroidLinearAccelerationSensor";
    string accelerometerLayout = "AndroidAccelerometer";
    string gyroscopeLayout = "AndroidGyroscope";

    string lightSensorLayout = "AndroidLightSensor";
    string proximitySensorLayout = "AndroidProximitySensor";
    string ambientTemperatureSensorLayout = "AndroidAmbientTemperatureSensor";
    string humiditySensorLayout = "AndroidHumiditySensor";
    string magneticFieldSensorLayout = "AndroidMagneticFieldSensor";
    string pressureSensorLayout = "AndroidPressureSensor";
    string stepCounterLayout = "AndroidStepCounter";

#endif

    public Touchscreen touchscreen => devices[touchscreenLayout] as Touchscreen;
    public Accelerometer accelerometer => devices[accelerometerLayout] as Accelerometer;
    public Gyroscope gyroscope => devices[gyroscopeLayout] as Gyroscope;
    public AttitudeSensor attitudeSensor => devices[attituteSensorLayout] as AttitudeSensor;
    public GravitySensor gravitySensor => devices[gravitySensorLayout] as GravitySensor;
    public LinearAccelerationSensor linearAccelerationSensor => devices[linearAccelerationSensorLayout] as LinearAccelerationSensor;
    public LightSensor lightSensor => devices[lightSensorLayout] as LightSensor;
    public ProximitySensor proximitySensor => devices[proximitySensorLayout] as ProximitySensor;
    public AmbientTemperatureSensor ambientTemperatureSensor => devices[ambientTemperatureSensorLayout] as AmbientTemperatureSensor;
    public HumiditySensor relativeHumiditySensor => devices[humiditySensorLayout] as HumiditySensor;
    public MagneticFieldSensor magneticFieldSensor => devices[magneticFieldSensorLayout] as MagneticFieldSensor;
    public PressureSensor pressureSensor => devices[pressureSensorLayout] as PressureSensor;


    Vector3 acceleration;
    Vector3 angularVelocity;
    float lightLevel;

    [SerializeField]
    float sensorDetectionDelay = 0.01f; 

    int currentDeviceId = 0;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        Invoke(nameof(InputSystemFindDevices), sensorDetectionDelay);

        StreamWriter writer = new 
        StreamWriter(Application.persistentDataPath + "/testlog.txt");

        // This using statement will ensure the writer will be closed when no longer used   
        using(writer)   
        {
            // Loop through the numbers from 1 to 20 and write them
            for (int i = 1; i <= 20; i++)
            {
                writer.WriteLine(i);
            }
        }
    }

    private void OnDisable()
    {
        //while (InputSystem.devices.Count > 0)
        //{
        //    if (InputSystem.devices[0] != null)
        //        InputSystem.RemoveDevice(InputSystem.devices[0]);
        //}
    }
    public T GetControlValue<T> (InputControl<T> control) where T : struct
    {
        if (!control.device.enabled)
            InputSystem.EnableDevice(control.device);
        return control.ReadValue();
    }
    public object GetControlValueObject(InputControl control)
    {
        if (!control.device.enabled)
            InputSystem.EnableDevice(control.device);
        return control.ReadValueAsObject();
    }
    public bool DeviceFound(string layout)
    {
        return devices.ContainsKey(layout);
    }
    void InputSystemFindDevices()
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            if (!devices.ContainsKey(device.layout))
                devices.Add(device.layout, device);
        }

#if !UNITY_EDITOR

        

#else

        string newFiles = "";
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

            newFiles += path + '\n';
        }
        sensorInfo.text = newFiles;
#endif
    }
    void Update()
    {
        InputSystemAccelerometer();

        InputSystemGyroscope();

#if UNITY_EDITOR
        DisplaySensorInfo();
#else
        DisplaySensorChecklist();
        InputSystemLightSensor();
#endif
    }
    void DisplaySensorChecklist()
    {
        string deviceChecklist = "Devices:\n";

        deviceChecklist += '\n';
        
        foreach (var device in InputSystem.devices)
        {
            deviceChecklist += device.layout + " " + device.enabled + "\n";
        }

        deviceChecklist += '\n';

        deviceChecklist += "Touchscreen: " + GetControlValueObject(touchscreen.position) + "\n";

        deviceChecklist += "Accelerometer: " + GetControlValueObject(accelerometer.acceleration) + "\n";
        deviceChecklist += "Gyroscope: " + GetControlValueObject(gyroscope.angularVelocity) + "\n";
        deviceChecklist += "Attitude sensor: " + GetControlValueObject(attitudeSensor.attitude) + "\n";
        deviceChecklist += "Gravity: " + GetControlValueObject(gravitySensor.gravity) + "\n";
        deviceChecklist += "LinearAcceleration: " + GetControlValueObject(linearAccelerationSensor.acceleration) + "\n";

        deviceChecklist += "Light: " + GetControlValueObject(lightSensor.lightLevel) + "\n";
        deviceChecklist += "Magnetic: " + GetControlValueObject(magneticFieldSensor.magneticField) + "\n";

        if (DeviceFound(pressureSensorLayout))
            deviceChecklist += "Pressure: " + GetControlValueObject(pressureSensor.atmosphericPressure) + "\n";


        sensorInfo.text = deviceChecklist;
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
            result += " = " + GetControlValueObject(control) + '\n';
            return;
        }

        result += '\n';

        foreach (var child in control.children)
        {
            WriteChildrenControls(child, ref result, it + 1);
        }
        return;
    }

    void InputSystemAccelerometer()
    {
        if (!DeviceFound(accelerometerLayout)) return;

        Debug.Log("Accelerometer: " + accelerometer.enabled);

        if (accelerometer.enabled)
            acceleration = GetControlValue(accelerometer.acceleration);

        if (accelerometerInfo != null)
            accelerometerInfo.text = "Accelerometer: " + acceleration;
    }

    void InputSystemGyroscope()
    {
        if (!DeviceFound(gyroscopeLayout)) return;

        InputSystem.EnableDevice(gyroscope);
        Debug.Log("Gyroscope: " + Gyroscope.current.enabled);

        if (gyroscope.enabled)
            angularVelocity = gyroscope.angularVelocity.ReadValue();

        if (gyroscopeInfo != null)
            gyroscopeInfo.text = "Gyroscope: " + gyroscope.angularVelocity.ReadValue();
    }

    void InputSystemLightSensor()
    {
        if (!DeviceFound(lightSensorLayout)) return;

        InputSystem.EnableDevice(lightSensor);
        Debug.Log("LightSensor: " + lightSensor.enabled);

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
