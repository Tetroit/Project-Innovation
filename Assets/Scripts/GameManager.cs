using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameData gameData;

    public Vector2 Resolution => new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

    float ghostTimer;
    public float ghostTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            instance.gameData = gameData;
            Destroy(gameObject);
        }
    }
    void Start()
    {
    }
}
