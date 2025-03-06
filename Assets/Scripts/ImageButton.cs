using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageButton : PuzzleElement
{
    public List<Texture> icons;
    public int currentIcon = 0;
    Material material;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }
    public override void Move()
    {
        if (SensorInput.DeviceFound(SensorInput.touchscreenLayout))
            if (SensorInput.touchscreen.primaryTouch.press.wasPressedThisFrame)
            {
                SetIcon((currentIcon+1) % icons.Count);
                material.SetTexture("_MagicMask", icons[currentIcon]);
            }
    }

    public override void OnSolved()
    {
    }
    public void SetIcon(int icon)
    {
        currentIcon = icon;
        material.SetTexture("_MagicMask", icons[currentIcon]);
    }
}
