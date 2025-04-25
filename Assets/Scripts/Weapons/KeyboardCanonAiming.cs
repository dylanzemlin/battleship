using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardCanonAiming : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public GameObject barrel;
    private float maxXAngle = -18f;
    private float minXAngle = -85f;

    void Update()
    {
        float rotationDelta = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            rotationDelta = -rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            rotationDelta = rotationSpeed * Time.deltaTime;
        }

        if (rotationDelta != 0f)
        {
            // Quaternion representing that small X‐axis rotation
            Quaternion deltaQuat = Quaternion.AngleAxis(rotationDelta, Vector3.right);

            transform.localRotation = transform.localRotation * deltaQuat;

            // Read back the Euler angles so we can clamp the X component
            Vector3 e = transform.localEulerAngles;
            float x = e.x;

            // Convert from [0..360) to (-180..+180] for intuitive clamping
            if (x > 180f)
            {
                x -= 360f;
            }

            // Clamp between minXAngle and maxXAngle
            if (x > maxXAngle)
            {
                x = maxXAngle;
            }
            else if (x < minXAngle)
            {
                x = minXAngle;
            }

            // Convert back into [0..360) if negative
            if (x < 0f)
            {
                x += 360f;
            }

            transform.localRotation = Quaternion.Euler(x, e.y, e.z);
        }
    }
}
