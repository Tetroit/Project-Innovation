using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public InputActionAsset inputActions; // Assign in Inspector
    private InputAction clickAction;

    private void Awake()
    {
        var sensorMap = inputActions.FindActionMap("Sensors");
        clickAction = sensorMap.FindAction("Click");

        clickAction.performed += ctx => OnClick();
    }

    private void OnEnable()
    {
        clickAction.Enable();
    }

    private void OnDisable()
    {
        clickAction.Disable();
    }

    private void OnClick()
    {
        Debug.Log("Click detected!");
        TouchScreenSelect();
    }

    public void TouchScreenSelect()
    {
        Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
        }
    }
}
