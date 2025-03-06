using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using System.IO;

/* After start please wait for a few seconds for the sensors to be detected.
 * You can check it with "isInitialized" variable.
 * 
 * To read values either use [control].ReadValue() or GetControlValue([control])
 * second one is a wrapper for the first one, but it also enables the device if it's disabled
 * 
 *Keyboard: 
 * 
 *  keyboard.[key].wasPressedThisFrame
 *  keyboard.[key].wasReleasedThisFrame
 * 
 *Mouse: 
 *
 *  mouse.[button].wasPressedThisFrame
 *
 *Touchscreen: 
 *
 *  ---- gets last pressed position ----
 *  GetSensorValue(touchscreen.position)
 *  
 *  ---- true if user is touching the screen ----
 *  touchscreen.press.isPressed
 *  
 *  ---- get all touch positions ----
 *  touchscreen.touches[n]
 *  
 */






using Gyroscope = UnityEngine.InputSystem.Gyroscope;

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

    public const string keyboardLayout = "Keyboard";
    public const string mouseLayout = "Mouse";
    public const string penLayout = "Pen";
    public const string touchscreenLayout = "Touchscreen";

#if UNITY_EDITOR

    public const string attituteSensorLayout = "AttitudeSensor";
    public const string gravitySensorLayout = "GravitySensor";
    public const string linearAccelerationSensorLayout = "LinearAccelerationSensor";
    public const string accelerometerLayout = "Accelerometer";
    public const string gyroscopeLayout = "Gyroscope";

    //unavailable on editor
    public const string lightSensorLayout = "LightSensor";
    public const string proximitySensorLayout = "ProximitySensor";
    public const string ambientTemperatureSensorLayout = "AmbientTemperatureSensor";
    public const string humiditySensorLayout = "HumiditySensor";
    public const string magneticFieldSensorLayout = "MagneticFieldSensor";
    public const string pressureSensorLayout = "PressureSensor";
    public const string stepCounterLayout = "StepCounter";

#elif UNITY_ANDROID

    public const string attituteSensorLayout = "AndroidRotationVector";
    public const string gravitySensorLayout = "AndroidGravitySensor";
    public const string linearAccelerationSensorLayout = "AndroidLinearAccelerationSensor";
    public const string accelerometerLayout = "AndroidAccelerometer";
    public const string gyroscopeLayout = "AndroidGyroscope";

    public const string lightSensorLayout = "AndroidLightSensor";
    public const string proximitySensorLayout = "AndroidProximitySensor";
    public const string ambientTemperatureSensorLayout = "AndroidAmbientTemperatureSensor";
    public const string humiditySensorLayout = "AndroidHumiditySensor";
    public const string magneticFieldSensorLayout = "AndroidMagneticFieldSensor";
    public const string pressureSensorLayout = "AndroidPressureSensor";
    public const string stepCounterLayout = "AndroidStepCounter";

#endif

    public static Keyboard keyboard => instance.devices[keyboardLayout] as Keyboard;
    public static Mouse mouse => instance.devices[mouseLayout] as Mouse;
    public static Pen pen => instance.devices[penLayout] as Pen;
    public static Touchscreen touchscreen => instance.devices[touchscreenLayout] as Touchscreen;
    public static Accelerometer accelerometer => instance.devices[accelerometerLayout] as Accelerometer;
    public static Gyroscope gyroscope => instance.devices[gyroscopeLayout] as Gyroscope;
    public static AttitudeSensor attitudeSensor => instance.devices[attituteSensorLayout] as AttitudeSensor;
    public static GravitySensor gravitySensor => instance.devices[gravitySensorLayout] as GravitySensor;
    public static LinearAccelerationSensor linearAccelerationSensor => instance.devices[linearAccelerationSensorLayout] as LinearAccelerationSensor;
    public static LightSensor lightSensor => instance.devices[lightSensorLayout] as LightSensor;
    public static ProximitySensor proximitySensor => instance.devices[proximitySensorLayout] as ProximitySensor;
    public static AmbientTemperatureSensor ambientTemperatureSensor => instance.devices[ambientTemperatureSensorLayout] as AmbientTemperatureSensor;
    public static HumiditySensor relativeHumiditySensor => instance.devices[humiditySensorLayout] as HumiditySensor;
    public static MagneticFieldSensor magneticFieldSensor => instance.devices[magneticFieldSensorLayout] as MagneticFieldSensor;
    public static PressureSensor pressureSensor => instance.devices[pressureSensorLayout] as PressureSensor;


    Vector3 acceleration;
    Vector3 angularVelocity;
    float lightLevel;

    [SerializeField]
    float sensorDetectionDelay = 0.01f; 

    int currentDeviceId = 0;

    bool initialized = false;
    public bool isInitialized => initialized;

    public static Action OnInitialise;
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
    public static T GetControlValue<T> (InputControl<T> control) where T : struct
    {
        if (!control.device.enabled)
            InputSystem.EnableDevice(control.device);
        return control.ReadValue();
    }
    public static object GetControlValueObject(InputControl control)
    {
        if (!control.device.enabled)
            InputSystem.EnableDevice(control.device);
        return control.ReadValueAsObject();
    }
    public static bool DeviceFound(string layout)
    {
        return instance.devices.ContainsKey(layout);
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
        if (sensorInfo != null)
            sensorInfo.text = newFiles;
#endif
        initialized = true;
        OnInitialise?.Invoke();
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

        //Debug.Log("Accelerometer: " + accelerometer.enabled);

        if (accelerometer.enabled)
            acceleration = GetControlValue(accelerometer.acceleration);

        if (accelerometerInfo != null)
            accelerometerInfo.text = "Accelerometer: " + acceleration;
    }

    void InputSystemGyroscope()
    {
        if (!DeviceFound(gyroscopeLayout)) return;

        InputSystem.EnableDevice(gyroscope);
        //Debug.Log("Gyroscope: " + gyroscope.enabled);

        if (gyroscope.enabled)
            angularVelocity = gyroscope.angularVelocity.ReadValue();

        if (gyroscopeInfo != null)
            gyroscopeInfo.text = "Gyroscope: " + angularVelocity;
    }

    void InputSystemLightSensor()
    {
        if (!DeviceFound(lightSensorLayout)) return;

        InputSystem.EnableDevice(lightSensor);
        Debug.Log("LightSensor: " + lightSensor.enabled);

        if (LightSensor.current.enabled)
            lightLevel = lightSensor.lightLevel.ReadValue();

        if (lightSensorInfo != null)
            lightSensorInfo.text = "LightSensor: " + lightLevel;
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
