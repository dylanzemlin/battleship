using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseAmmo : MonoBehaviour
{
    public Transform planetCenter; // Assign in inspector or via code
    public float gravityStrength = 9.8f;
    public Transform visualModel;

    public Rigidbody rb;

    public void Fire(Vector3 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void FixedUpdate()
    {
        // Apply gravity towards the planet center if it's assigned
        if (planetCenter != null)
        {
            Vector3 gravityDirection = (planetCenter.position - transform.position).normalized;
            rb.AddForce(gravityDirection * gravityStrength, ForceMode.Acceleration);
        }

        // Rotate the ammo towards the direction of travel
        if (rb.linearVelocity != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (other.gameObject.name.Contains("Cannon"))
        {
            return;
        }

        ParticleSystem system = GameController.Instance.waterSplash;
        
        // set the systems position to the hit
        system.transform.position = transform.position;
        
        // calculate the direction "away" from the planet based on the location of the hit. basically, the planet normal at the hit point
        Vector3 planetNormal = (transform.position - planetCenter.position).normalized;

        // set the direction of the particle system to the planet normal
        system.transform.forward = planetNormal;
        system.transform.rotation = Quaternion.LookRotation(planetNormal, Vector3.up);
        system.Play();
    }
}
