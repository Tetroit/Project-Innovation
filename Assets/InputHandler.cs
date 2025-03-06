using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public InputActionAsset inputActions; 
    private InputAction clickAction;

    public RaycastHit hit { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
        //TouchScreenSelect();
    }

    public void TouchScreenSelect()
    {
        Ray ray = Camera.main.ScreenPointToRay(Touchscreen.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10))
        {
            hit = hitInfo;
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
        }
        else
        {
            Debug.Log("No object hit.");
        }
    }

}
