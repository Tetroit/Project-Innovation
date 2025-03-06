using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

using Random = UnityEngine.Random;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public LevelData levelData;

    public float lightSamplingRate = 0.1f;
    public int lightSamples = 10;
    public float lightThreshold = 0.2f;
    public Vector2 lightLimits = new Vector2(1, 4);
    Queue<float> lightData = new();
    float m_lightLevel;
    float m_lightFac;

    float simulatedLightData;
    float simulatedLightLevel;
    public bool hasLightSensor => SensorInput.DeviceFound(SensorInput.lightSensorLayout);

    [SerializeField]
    bool DisplayLogInBuild = true;
    [SerializeField]
    bool DisplayLogInEditor = true;


    public Action onSwitchLight;
    public Action onSwitchDark;
    public float lightFac => m_lightFac;
    public float lightAverage { get; private set; }
    public bool isLight;
    public bool isDark => !isLight;

    public float ghostTimeTotal;
    [SerializeField]
    float ghostTimer;
    public Action<float> onGhostTimer;
    public Action onGhostFail;
    public Action onGhostSuccess;
    public Vector2 Resolution => new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

    public TextMeshProUGUI infoDisplay;
    private void Awake()
    {
        //level data must be present!
        if (levelData == null)
            levelData = FindObjectOfType<LevelData>();
        if (levelData == null)
            Debug.LogError("No level data found");


        //singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            instance.levelData = levelData;
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SensorInput.OnInitialise += SensorInputInitialised;
        if (m_lightFac > 0.5f) 
            onSwitchLight?.Invoke();
        else
            onSwitchDark?.Invoke();
    }
    private void Update()
    {
        //decrease timer when it is dark
        if (isDark)
            onGhostTimer?.Invoke(ghostTimer += Time.deltaTime);
        if (ghostTimer > ghostTimeTotal)
        {
            Debug.Log("u gay");
            onGhostFail?.Invoke();
        }
            

        //debug, nothing to see here
#if UNITY_EDITOR
        if (DisplayLogInEditor)
            DisplayInfo();
        else if (infoDisplay != null)
            infoDisplay.gameObject.SetActive(false);
#else
        if (DisplayLogInBuild)
            DisplayInfo();
        else if (infoDisplay != null)
            infoDisplay.gameObject.SetActive(false);
#endif
        //simulates light if no light sensor
        if (!hasLightSensor)
            SimulateLight();
    }

    public void PuzzlesCompleted()
    {
        Debug.Log("u win");
        onGhostSuccess?.Invoke();
    }
    /// <summary>
    /// generates random values from reasonable range, that is similar to LightSensor behaviour.
    /// press o to increase light.
    /// press i to decrease light.
    /// </summary>
    void SimulateLight()
    {
        if (Keyboard.current.oKey.isPressed)
            simulatedLightLevel += Time.deltaTime * (simulatedLightLevel + 1f);
        if (Keyboard.current.iKey.isPressed)
            simulatedLightLevel -= Time.deltaTime * (simulatedLightLevel + 1f);

        simulatedLightLevel = Mathf.Clamp(simulatedLightLevel, 0, 1000);
        simulatedLightData = simulatedLightLevel + Random.Range(0, simulatedLightLevel) * 0.05f;
        simulatedLightData = Mathf.Clamp(simulatedLightData, 0, 1000);

    }
    /// <summary>
    /// just debug, nothing is happening here really
    /// </summary>
    public void DisplayInfo()
    {
        if (infoDisplay == null) return;

        string res = "Sensor found: \n";
        res += "\nLight Sensor:" + SensorInput.DeviceFound(SensorInput.lightSensorLayout).ToString() + '\n';
        res += "\nAverage: \n";
        res += m_lightLevel;
        res += "\nFac: \n";
        res += m_lightFac;
        res += "\nDark state ---- Light state: \n";
        res += lightLimits.x + "----" + lightLimits.y + "\n";
        res += "";
        res += "\nTouhcscreen:" + SensorInput.DeviceFound(SensorInput.lightSensorLayout).ToString() + '\n';
        res += "\nPosition:" + SensorInput.DeviceFound(SensorInput.lightSensorLayout).ToString();

        infoDisplay.text = res;
    }
    /// <summary>
    /// Start reading Light Sensor data
    /// </summary>
    public void SensorInputInitialised()
    {
        StartCoroutine(ProcessLightData());
    }
    /// <summary>
    /// Changes how frequently game processes light sensor data
    /// </summary>
    /// <param name="period"></param>
    public void ChangeLightSamplingRate(float period)
    {
        lightSamplingRate = period;
    }
    /// <summary>
    /// sets LIGHT limit. For calibration
    /// </summary>
    public void RecordLight()
    {
        lightLimits.y = m_lightLevel;
    }
    /// <summary>
    /// sets DARK limit. For calibration
    /// </summary>
    public void RecordDark()
    {
        lightLimits.x = m_lightLevel;
    }

    /// <summary>
    /// processes light info
    /// </summary>
    /// <returns></returns>
    IEnumerator ProcessLightData()
    {
        while (true)
        {
            while (lightData.Count >= lightSamples)
                lightData.Dequeue();

            if (hasLightSensor)
                lightData.Enqueue(SensorInput.GetControlValue(SensorInput.lightSensor.lightLevel));
            else
                lightData.Enqueue(simulatedLightData);

            if (lightData.Count > 0)
            {

                float res = 0;
                foreach (var value in lightData)
                {
                    res += value;
                }
                res /= lightData.Count;
                m_lightLevel = res;
            }
            else
                m_lightLevel = 0;

            var lnmin = Mathf.Log(lightLimits.x);
            var lnmax = Mathf.Log(lightLimits.y);
            var lnlevel = Mathf.Log(m_lightLevel);

            m_lightFac = Mathf.Clamp((lnlevel - lnmin) / (lnmax - lnmin), 0, 1);
            lightAverage = (lnmax + lnmin) / 2;

            var init = isLight;

            isLight = isLight ? 
                m_lightFac > 0.5f - lightThreshold : 
                (m_lightFac > 0.5f + lightThreshold);

            if (init && !isLight)
            {
                Debug.Log("dark");
                onSwitchDark?.Invoke();
            }
            if (!init && isLight)
            {
                Debug.Log("dark");
                onSwitchLight?.Invoke();
            }


            yield return new WaitForSeconds(lightSamplingRate);
        }
    }
}
