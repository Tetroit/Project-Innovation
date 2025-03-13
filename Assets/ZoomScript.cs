using UnityEngine;
using UnityEngine.InputSystem;

public class PinchZoom : MonoBehaviour
{
    public Camera mainCamera;
    public float zoomSpeed = 0.1f;
    public float minZoom = 5f;
    public float maxZoom = 30f;

    private float previousDistance;

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.touches.Count >= 2)
        {
            var touch1 = Touchscreen.current.touches[0];
            var touch2 = Touchscreen.current.touches[1];

            if (touch1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved &&
                touch2.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touch1.position.ReadValue(), touch2.position.ReadValue());

                if (previousDistance != 0)
                {
                    float delta = previousDistance - currentDistance;
                    mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView + delta * zoomSpeed, minZoom, maxZoom);
                }

                previousDistance = currentDistance;
            }
        }
        else
        {
            previousDistance = 0;
        }
    }

    void PauseGame()
    {
        if (GameManager.instance.isPaused == true)
        {
            zoomSpeed = 0;
        }
        else
        {
            zoomSpeed = 0.1f;
        }
    }
}
