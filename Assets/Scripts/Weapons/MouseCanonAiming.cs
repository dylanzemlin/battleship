using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCanonAiming : MonoBehaviour
{

    public float rotationSpeed = 100f;
    public float maxXAngle = -15f;

    private void OnMouseDown()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnMouseUp()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void OnMouseDrag()
    {
        // Retrieve the current x rotation in Euler angles
        float currentXAngle = transform.eulerAngles.x;
        float rotationDelta = 0f;

        // Convert from 0-360 to -180 to 180 for easier clamping
        if (currentXAngle > 180f)
        {
            currentXAngle -= 360f;
        }

        // Move canon up
        if (Input.GetAxis("Mouse Y") > 0 && Input.GetAxis("Mouse X") < 0)
        {
            rotationDelta = -rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetAxis("Mouse Y") < 0 && Input.GetAxis("Mouse X") > 0) // Move canon down
        {
            rotationDelta = rotationSpeed * Time.deltaTime;
        }

        // Compute the new angle and clamp it between minXAngle and maxXAngle
        float newXAngle = Mathf.Clamp(currentXAngle + rotationDelta, -90, maxXAngle);

        // Update the canonBase rotation. We only change the x component and keep y and z unchanged.
        transform.eulerAngles = new Vector3(newXAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
