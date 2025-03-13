using UnityEngine;
using UnityEngine.InputSystem;

public class RotateAroundObject : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 50f;
    public Vector3 rotationAxis = Vector3.up;
    public LayerMask cubeLayer;
    public float damping = 5f;
    public float defaultCameraDistance = 5f;

    private Vector2 lastTouchDelta;
    private Vector2 inertia;
    private bool isTouching;
    private float verticalAngle = 0f;
    private float minVerticalAngle = -90f;
    private float maxVerticalAngle = 90f;
    private Vector3 initialOffset;

    void Start()
    {
        GameManager.instance.isPaused = false;
        if (target != null)
        {
            initialOffset = (transform.position - target.position).normalized * defaultCameraDistance;
            transform.position = target.position + initialOffset;
            transform.LookAt(target);
        }
    }

    void Update()
    {
        if (target != null)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.layer == Mathf.Log(cubeLayer.value, 2))
                {
                    isTouching = false;
                    return;
                }

                Vector2 touchMovement = Touchscreen.current.primaryTouch.delta.ReadValue();
                RotateCamera(touchMovement);

                lastTouchDelta = touchMovement;
                isTouching = true;
            }
            else
            {
                if (isTouching)
                {
                    inertia = lastTouchDelta;
                }
                isTouching = false;
            }

            if (!isTouching && inertia.magnitude > 0.01f)
            {
                RotateCamera(inertia);
                inertia = Vector2.Lerp(inertia, Vector2.zero, Time.deltaTime * damping);
            }
        }

        PauseGame();
    }

    void RotateCamera(Vector2 movement)
    {
        transform.RotateAround(target.position, Vector3.up, movement.x * rotationSpeed * Time.deltaTime * 0.1f);

        float verticalRotation = -movement.y * rotationSpeed * Time.deltaTime * 0.1f;
        float newVerticalAngle = Mathf.Clamp(verticalAngle + verticalRotation, minVerticalAngle, maxVerticalAngle);

        transform.RotateAround(target.position, transform.right, newVerticalAngle - verticalAngle);
        verticalAngle = newVerticalAngle;

        Vector3 direction = (transform.position - target.position).normalized;
        transform.position = target.position + direction * defaultCameraDistance;
    }

    void PauseGame()
    {
        if(GameManager.instance.isPaused == true)
        {
            rotationSpeed = 0;
        }
        else
        {
            rotationSpeed = 50;
        }
    }
}
