using UnityEngine;
using UnityEngine.InputSystem;

public class RotateAroundObject : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 50f;
    public Vector3 rotationAxis = Vector3.up;
    private Vector2 touchDelta;
    private bool isTouching;
    public LayerMask cubeLayer; 

    void Update()
    {
        if (target != null)
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.layer == Mathf.Log(cubeLayer.value, 2))
                {
                    isTouching = false;
                    return;
                }

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
