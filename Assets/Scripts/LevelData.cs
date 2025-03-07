using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    /// <summary>
    /// will launch the timer if true
    /// </summary>
    public bool shouldRunGhostTimer = true;
    /// <summary>
    /// time after which the player loses
    /// </summary>
    public float ghostTimer = 60;
    /// <summary>
    /// puzzle manager in the level. Leave empty if no puzzles
    /// </summary>
    public PuzzleManager puzzleManager;
    /// <summary>
    /// attach if you want to display gamemanager debug info. Leave empty if you dont want to output gamemanager info
    /// </summary>
    public TextMeshProUGUI infoDisplay;
}
