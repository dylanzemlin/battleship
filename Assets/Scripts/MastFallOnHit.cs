using UnityEngine;

public class MastFallOnHit : MonoBehaviour
{
    public float fallSpeed = 30f; // Degrees per second
    public float fallAngle = 90f; // How far to fall

    private bool isFalling = false;
    private float currentRotation = 0f;

    void Update()
    {
        if (isFalling && currentRotation < fallAngle)
        {
            float rotationStep = fallSpeed * Time.deltaTime;
            float remaining = fallAngle - currentRotation;

            float actualStep = Mathf.Min(rotationStep, remaining);
           transform.Rotate(Vector3.forward, actualStep);


            currentRotation += actualStep;
        }
    }

    public void TriggerFall()
    {
        isFalling = true;
        Debug.Log("Mast is falling!");
    }
}
