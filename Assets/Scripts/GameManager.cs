using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public LevelData levelData;

    public Vector2 Resolution => new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

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
    void Start()
    {
    }
}
