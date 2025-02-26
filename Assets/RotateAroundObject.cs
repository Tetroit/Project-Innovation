using UnityEngine;
using UnityEngine.InputSystem;

public class RotateAroundObject : MonoBehaviour
{
    public Transform target; // Object to rotate around
    public float rotationSpeed = 50f;
    public Vector3 rotationAxis = Vector3.up; // Default rotation axis
    private Vector2 touchDelta;
    private bool isTouching;

    void Update()
    {
        if (target != null)
        {
            // Rotate using touch input
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                if (!isTouching)
                {
                    isTouching = true;
                    touchDelta = Vector2.zero;
                }
                else
                {
                    Vector2 touchMovement = Touchscreen.current.primaryTouch.delta.ReadValue();
                    transform.RotateAround(target.position, Vector3.up, touchMovement.x * rotationSpeed * Time.deltaTime * 0.1f);
                    transform.RotateAround(target.position, transform.right, -touchMovement.y * rotationSpeed * Time.deltaTime * 0.1f);
                }
            }
            else
            {
                isTouching = false;
            }
        }
    }
}