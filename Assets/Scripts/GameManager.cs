using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
public class GameManager : MonoBehaviour
{
    public bool isPaused;
    public static GameManager instance;
    public LevelData levelData;

    public float lightSamplingRate = 0.1f;
    public int lightSamples = 10;
    public float lightThreshold = 0.2f;
    public Vector2 lightLimits = new Vector2(1, 4);
    Queue<float> lightData = new();
    float m_lightLevel;
    float m_lightFac;

    public string selectedLevelName = "MainGame";

    float simulatedLightData;
    float simulatedLightLevel;
    public bool hasLightSensor => SensorInput.DeviceFound(SensorInput.lightSensorLayout);
    public bool isCalibrated { get; private set; } = false;

    [SerializeField]
    bool DisplayLogInBuild = true;
    [SerializeField]
    bool DisplayLogInEditor = true;


    public Action onSwitchLight;
    public Action onSwitchDark;
    bool firstSwitchDark = false;
    public Action onFirstSwitchDark;
    public float lightFac => m_lightFac;
    public float lightAverage { get; private set; }
    public bool isLight;
    public bool isDark => !isLight;
    bool timeOut = false;

    public float ghostTimeTotal => levelData.ghostTimer;
    [SerializeField]
    float ghostTimer;
    public Action<float> onGhostTimer;
    public Action onGhostFail;
    public Action onGhostSuccess;

    CanvasGroup deathEffect => levelData?.deathEffect;
    CanvasGroup blinder => levelData?.blinder;
    public Vector2 Resolution => new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

    public TextMeshProUGUI infoDisplay => levelData.infoDisplay;

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
        SceneManager.sceneLoaded += OnSceneChange;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnSceneChange(Scene _new, LoadSceneMode mode)
    {
        Debug.Log("Scene change");
        if (m_lightFac > 0.5f)
            onSwitchLight?.Invoke();
        else
            onSwitchDark?.Invoke();

        timeOut = false;
        if (deathEffect != null)
            deathEffect.alpha = 0;
        if (blinder != null)
            StartCoroutine(FadeInScene());
        UnityEvent ev;
    }
    private void Update()
    {

        if (levelData.shouldRunGhostTimer)
        {
            //decrease timer when it is dark
            if (isDark)
                onGhostTimer?.Invoke(ghostTimer += Time.deltaTime);
            if (ghostTimer > ghostTimeTotal && !timeOut)
            {
                Debug.Log("u gay");
                onGhostFail?.Invoke();
                StartDeathEffect();
                timeOut = true;
            }
        }
        if (SensorInput.isInitialized)
            ProcessLightData();

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
        levelData.shouldRunGhostTimer = false;
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
        if (infoDisplay == null || !SensorInput.isInitialized) return;

        string res = "Sensor found: \n";
        if (SensorInput.DeviceFound(SensorInput.lightSensorLayout))
        {
            res += "\nLight Sensor:\nRaw:" + SensorInput.GetControlValue(SensorInput.lightSensor.lightLevel) + '\n';
            res += "\nAverage: \n";
            res += m_lightLevel;
            res += "\nFac: \n";
            res += m_lightFac;
            res += "\nDark state ---- Light state: \n";
            res += lightLimits.x + "----" + lightLimits.y + "\n";
            res += "";
        }
        if (SensorInput.DeviceFound(SensorInput.touchscreenLayout))
        {
            res += "\nTouhcscreen:" + SensorInput.DeviceFound(SensorInput.lightSensorLayout).ToString() + '\n';
            res += "\nPosition:" + SensorInput.GetControlValue(SensorInput.touchscreen.position);
        }

        infoDisplay.text = res;
    }
    /// <summary>
    /// Start reading Light Sensor data
    /// </summary>
    public void SensorInputInitialised()
    {
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

    public void MarkCalibrated() => isCalibrated = true;

    /// <summary>
    /// processes light info
    /// </summary>
    /// <returns></returns>
    void ProcessLightData()
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

        var lnmin = Mathf.Log(lightLimits.x+1);
        var lnmax = Mathf.Log(lightLimits.y+1);
        var lnlevel = Mathf.Log(m_lightLevel+1);

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

            if (!firstSwitchDark)
            {
                firstSwitchDark = true;
                onFirstSwitchDark?.Invoke();
            }
            
        }
        if (!init && isLight)
        {
            Debug.Log("light");
            onSwitchLight?.Invoke();
            ghostTimer = 0;
        }
    }

    void StartDeathEffect()
    {
        StartCoroutine(DeathEffect());
    }
    IEnumerator DeathEffect()
    {
        float t = 0;
        while (t < 4) {
            t+= Time.deltaTime;
            float fac = Mathf.InverseLerp(0f, 1f, t);
            if (blinder != null)
                blinder.alpha = fac;
            if (t >= 1f && t <= 2f)
            {
                fac = Mathf.InverseLerp(1f, 2f, t);
                if (deathEffect != null)
                    deathEffect.alpha = fac;
            }
            if (t >= 3f && t <= 4f)
            {
                fac = Mathf.InverseLerp(3f, 4f, t);
                if (deathEffect != null)
                    deathEffect.alpha = 1 - fac;
            }
            yield return new WaitForEndOfFrame();
        }
        Restart();
        yield return null;
    }
    IEnumerator FadeInScene(float time = 1)
    {
        if (blinder != null)
        {
            float t = 0;
            while (t < time)
            {
                t+= Time.deltaTime;
                float fac = t / time;
                blinder.alpha = 1 - fac;
                yield return new WaitForEndOfFrame();
            }
        }
        yield break;
    }
    IEnumerator FadeOutScene(float time = 1, Action func = null)
    {
        if (blinder != null)
        {
            float t = 0;
            while (t < time)
            {
                t += Time.deltaTime;
                float fac = t / time;
                blinder.alpha = fac;
                yield return new WaitForEndOfFrame();
            }
        }
        if (func != null)
            func();
        yield break;
    }
    public void Restart()
    {
        ghostTimer = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
