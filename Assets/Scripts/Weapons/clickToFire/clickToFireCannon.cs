using UnityEngine;

public class clickToFireCannon : BaseWeaponClick
{
    public ammoTrajectoryAndCollision loadedAmmo; // Reference to currently loaded ammo
    public float launchSpeed = 50f; // How fast the ammo should be launched
    public Transform fireDirectionTransform; // Child transform for aiming

    public override void Fire()
    {
        if (loadedAmmo != null)
        {
            Debug.Log("Cannon fired using loaded ammo!");

            // Detach from cannon
            loadedAmmo.transform.SetParent(null);

            // Unlock physics BEFORE firing
            Rigidbody rb = loadedAmmo.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // important for fast collision
            }

            loadedAmmo.hasBeenFired = true; // 💥 Mark it as fired for collision logic

            // Pick firing direction
            Vector3 fireDir = fireDirectionTransform != null ? fireDirectionTransform.forward : transform.forward;

            // Actually launch it
            loadedAmmo.Fire(fireDir, launchSpeed, gameObject);

            // Clear loaded ammo
            loadedAmmo = null;
        }
        else
        {
            Debug.LogWarning("No ammo loaded!");
            return;
        }

        // === Play Effects ===
        ParticleSystemController system = GameController.Instance.explosionController;
        if (system != null)
        {
            Vector3 newPos = barrel.transform.position + barrel.transform.forward * 3f;
            system.Move(newPos);
            system.Rotate(barrel.transform.rotation);
            system.Stop();
            system.Play();
        }

        ParticleSystem smoke = GameController.Instance.smokeBurst;
        if (smoke != null)
        {
            Vector3 newPos = barrel.transform.position + barrel.transform.forward * 3f;
            smoke.transform.position = newPos - transform.forward * 4f - transform.up * 2f;
            smoke.transform.forward = -transform.forward;
            smoke.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
            smoke.Play();
        }
    }

    private void Update()
    {
        OnUpdate();
    }
}
