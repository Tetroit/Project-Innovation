using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public LevelData levelData;

    public float lightSamplingRate = 0.1f;
    public int lightSamples = 10;
    public Vector2 lightLimits = new Vector2(3, 100);
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

    public float lightFac => m_lightFac;
    public Vector2 Resolution => new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

    public TextMeshProUGUI infoDisplay;
    private void Awake()
    {
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
    }
    private void Update()
    {
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
        if (!hasLightSensor)
            SimulateLight();
    }
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
    public void DisplayInfo()
    {
        if (infoDisplay == null) return;

        string res = "Sensor found: \n";
        res += SensorInput.DeviceFound(SensorInput.lightSensorLayout).ToString() + '\n';
        res += "Data: \n";
        foreach (var item in lightData)
        {
            res += "\t" + item.ToString() + "\n";
        }
        res += "\nAverage: \n";
        res += m_lightLevel;
        res += "\nFac: \n";
        res += m_lightFac;
        res += "\nDark state ---- Light state: \n";
        res += lightLimits.x + "----" + lightLimits.y;

        infoDisplay.text = res;
    }
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
    public void RecordLight()
    {
        lightLimits.y = m_lightLevel;
    }
    public void RecordDark()
    {
        lightLimits.x = m_lightLevel;
    }
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

            m_lightFac = Mathf.Clamp((m_lightLevel - lightLimits.x) / (lightLimits.y - lightLimits.x), 0, 1);

            Debug.Log(lightFac);

            yield return new WaitForSeconds(lightSamplingRate);
        }
    }
}
