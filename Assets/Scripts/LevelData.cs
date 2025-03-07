using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public bool shouldRunGhostTimer = true;
    public float ghostTimer = 60;
    public PuzzleManager puzzleManager;
    public TextMeshProUGUI infoDisplay;
}
