using UnityEngine;

// RequireComponent ensures a Rigidbody is always attached to this GameObject
[RequireComponent(typeof(Rigidbody))]
public class BaseAmmo : MonoBehaviour
{
    // === CONFIGURABLE PROPERTIES ===

    public Transform planetCenter;     // Reference to the center of the planet for gravity simulation
    public float gravityStrength = 9.8f; // Gravity force applied toward the planet
    public Transform visualModel;      // Optional: if you want to rotate visuals separately from physics

    public Rigidbody rb;               // Rigidbody that will move the ammo

    // === FIRING LOGIC ===

    public void Fire(Vector3 direction, float speed)
    {
        // Applies initial velocity to the ammo in the given direction and speed
        rb.linearVelocity = direction.normalized * speed;
    }

    // === PHYSICS & GRAVITY ===

    private void FixedUpdate()
    {
        // Apply custom gravity that pulls ammo toward the planet's center
        if (planetCenter != null)
        {
            // Compute direction toward planet center
            Vector3 gravityDirection = (planetCenter.position - transform.position).normalized;

            // Apply a force toward the planet (like artificial gravity)
            rb.AddForce(gravityDirection * gravityStrength, ForceMode.Acceleration);
        }

        // Smoothly rotate the ammo to face in the direction it’s flying
        if (rb.linearVelocity != Vector3.zero)
        {
            // Determine rotation facing the velocity direction
            Quaternion rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);

            // Smoothly interpolate from current rotation to target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    // === COLLISION & TRIGGER DETECTION ===

    private void OnTriggerEnter(Collider other)
    {
        // Ignore collisions with cannons since it is a firing point
        if (other.gameObject.name.Contains("Cannon"))
        {
            return; // Skip processing
        }

        // Check if the ammo hit a ship prefab called "PirateShipUpdated"
        if (other.gameObject.name.Contains("PirateShipUpdated"))
        {
            // Try to find the child mast object inside the ship
            Transform mastTransform = other.transform.Find("centerMast");

            if (mastTransform != null)
            {
                // Attempt to get the falling mast behavior script
                MastFallOnHit mastFall = mastTransform.GetComponent<MastFallOnHit>();

                // If the script exists on the mast, trigger its fall animation/rotation
                if (mastFall != null)
                {
                    mastFall.TriggerFall();
                    // You could add a Debug.Log here if you want to confirm hits
                }
            }

            return; // Skip the splash effect
        }

        // === SPLASH EFFECT (WATER IMPACT) ===

        // Access the shared splash particle system from the game controller
        ParticleSystem system = GameController.Instance.waterSplash;

        // Move the particle system to the point of impact
        system.transform.position = transform.position;

        // Calculate the surface normal based on impact point and planet center
        Vector3 planetNormal = (transform.position - planetCenter.position).normalized;

        // Orient the splash to look "away" from the planet, using surface normal
        system.transform.forward = planetNormal;
        system.transform.rotation = Quaternion.LookRotation(planetNormal, Vector3.up);

        // Trigger the water splash visual effect
        system.Play();


        // Create New Instance of fishScatterParticlePrefab --------------------------
        GameObject fishScatterInstance = Instantiate(GameController.Instance.fishScatterParticlePrefab, transform.position, Quaternion.identity);
        if (fishScatterInstance == null) {
            Debug.LogWarning("Could not instantiate the fishScatterParticlePrefab from GameController.Instance");
        } else {
            // set the direction of the particle system to the planet normal
            fishScatterInstance.transform.forward = planetNormal;
            // fishScatterInstance.transform.position = Vector3.MoveTowards(fishScatterInstance.transform.position, planetCenter.position, 3f);
            fishScatterInstance.transform.rotation = Quaternion.LookRotation(planetNormal, Vector3.up);
            // Play the particle system
            ParticleSystem fishScatterParticles = fishScatterInstance.GetComponent<ParticleSystem>();
            if (fishScatterParticles == null) {
                Debug.LogWarning("Could not access fish Scatter particle system from fishScatterInstance");
            } else {
                fishScatterParticles.Play();
            }
        }

        // Destroy Ammo ---------------------------------------------------------------
        Destroy(gameObject);
    }
}
