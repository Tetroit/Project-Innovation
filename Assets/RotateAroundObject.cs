using UnityEngine;
using UnityEngine.InputSystem;

public class RotateAroundObject : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 50f;
    public Vector3 rotationAxis = Vector3.up;
    public LayerMask cubeLayer;
    public float damping = 5f; 

    private Vector2 touchDelta;
    private Vector2 lastTouchDelta;
    private Vector2 inertia;
    private bool isTouching;
    private float verticalAngle = 0f;
    private float minVerticalAngle = -90f;
    private float maxVerticalAngle = 90f;

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
                transform.RotateAround(target.position, Vector3.up, touchMovement.x * rotationSpeed * Time.deltaTime * 0.1f);

                float verticalRotation = -touchMovement.y * rotationSpeed * Time.deltaTime * 0.1f;
                float newVerticalAngle = Mathf.Clamp(verticalAngle + verticalRotation, minVerticalAngle, maxVerticalAngle);

                transform.RotateAround(target.position, transform.right, newVerticalAngle - verticalAngle);
                verticalAngle = newVerticalAngle;

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

            // Apply inertia if not touching
            if (!isTouching && inertia.magnitude > 0.01f)
            {
                transform.RotateAround(target.position, Vector3.up, inertia.x * rotationSpeed * Time.deltaTime * 0.1f);

                float verticalRotation = -inertia.y * rotationSpeed * Time.deltaTime * 0.1f;
                float newVerticalAngle = Mathf.Clamp(verticalAngle + verticalRotation, minVerticalAngle, maxVerticalAngle);

                transform.RotateAround(target.position, transform.right, newVerticalAngle - verticalAngle);
                verticalAngle = newVerticalAngle;

                inertia = Vector2.Lerp(inertia, Vector2.zero, Time.deltaTime * damping);
            }
        }
    }
}
