using UnityEngine; // Unity's main engine library, needed for GameObjects, Rigidbody, MonoBehaviour, etc.

public class clickToFireCannon : BaseWeaponClick
{
    // === CONFIGURABLE AND RUNTIME PROPERTIES ===

    public ammoTrajectoryAndCollision loadedAmmo; // Reference to the currently loaded ammo (assigned when player clicks ammo to load it)
    public float launchSpeed = 50f;                // How fast the ammo should be launched out of the cannon
    public Transform fireDirectionTransform;      // Child transform used to determine the cannon's firing direction (for aiming)

    // === MAIN FIRING FUNCTION ===
    public override void Fire()
    {
        if (loadedAmmo != null) // Only fire if there is ammo loaded
        {
            GameObject ship = this.transform.parent.gameObject.transform.parent.gameObject;

            Debug.Log("Cannon fired using loaded ammo!");

            // Detach ammo from the cannon to let it move independently
            loadedAmmo.transform.SetParent(null);

            // Unlock physics BEFORE firing so the projectile can move freely
            Rigidbody rb = loadedAmmo.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Enable physics simulation
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; 
                // Important: prevents "tunneling" through fast-moving objects
            }

            // Mark the ammo as fired so its collision code knows to act differently
            loadedAmmo.hasBeenFired = true;

            // Pick the firing direction:
            // Use the fireDirectionTransform’s forward direction if available, otherwise default to the cannon’s forward direction
            Vector3 fireDir = fireDirectionTransform != null ? fireDirectionTransform.forward : transform.forward;

            // Actually launch the ammo
            loadedAmmo.Fire(fireDir, launchSpeed, ship);

            // Clear the loaded ammo so the cannon is now empty and ready to reload
            loadedAmmo = null;
        }
        else
        {
            Debug.LogWarning("No ammo loaded!"); // Warn if the player tries to fire an empty cannon
            return; // Exit early
        }

        // === SPECIAL EFFECTS AFTER FIRING ===

        // Access and trigger the explosion particle system
        ParticleSystemController system = GameController.Instance.explosionController;
        if (system != null)
        {
            // Calculate a position slightly in front of the cannon barrel
            Vector3 newPos = barrel.transform.position + barrel.transform.forward * 3f;

            // Move particle system to the firing point
            system.Move(newPos);

            // Rotate particle system to match barrel's orientation
            system.Rotate(barrel.transform.rotation);

            // Restart the particle system
            system.Stop();
            system.Play();
        }

        // Access and trigger the smoke burst particle system
        ParticleSystem smoke = GameController.Instance.smokeBurst;
        if (smoke != null)
        {
            // Position the smoke burst slightly offset from the cannon barrel
            Vector3 newPos = barrel.transform.position + barrel.transform.forward * 3f;
            smoke.transform.position = newPos - transform.forward * 4f - transform.up * 2f;

            // Set smoke facing backwards (opposite to cannon firing)
            smoke.transform.forward = -transform.forward;
            smoke.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);

            // Play the smoke effect
            smoke.Play();
        }
    }

    // === UPDATE LOOP ===
    private void Update()
    {
        OnUpdate(); // Inherit whatever update behavior is defined in BaseWeaponClick
    }
}
