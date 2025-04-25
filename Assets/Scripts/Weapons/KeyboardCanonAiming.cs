using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardCanonAiming : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float maxXAngle = -15f;

    private bool onCanon = false;

    private void OnMouseDown()
    {
        onCanon = true;
    }

    private void OnMouseUp() 
    { 
        onCanon = false; 
    }

    void Update()
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
        if (Input.GetKey(KeyCode.UpArrow) && onCanon)
        {
            rotationDelta = -rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && onCanon) // Move canon down
        {
            rotationDelta = rotationSpeed * Time.deltaTime;
        }

        // Compute the new angle and clamp it between minXAngle and maxXAngle
        float newXAngle = Mathf.Clamp(currentXAngle + rotationDelta, -90, maxXAngle);

        // Update the canonBase rotation. We only change the x component and keep y and z unchanged.
        transform.eulerAngles = new Vector3(newXAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
