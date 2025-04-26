using UnityEngine;

public class clickToFireCannon : BaseWeaponClick
{
    public BaseAmmo loadedAmmo; // Reference to the currently loaded ammo
    public float launchSpeed = 50f; // How fast the ammo should be launched
    public Transform fireDirectionTransform; // Optional: a child transform (like the barrel tip) to determine firing direction

    public override void Fire()
    {
        if (loadedAmmo != null)
        {
            Debug.Log("Cannon fired using loaded ammo!");

            // Detach the ammo from the cannon so it becomes independent
            loadedAmmo.transform.SetParent(null);

            // Determine the firing direction (prefer fireDirectionTransform if available)
            Vector3 fireDir = fireDirectionTransform != null ? fireDirectionTransform.forward : transform.forward;

            // Trigger the ammo's firing logic
            loadedAmmo.Fire(fireDir, launchSpeed, gameObject);

            // Clear the loaded ammo reference so the cannon is empty
            loadedAmmo = null;
        }
        else
        {
            Debug.LogWarning("No ammo loaded!");
            return;
        }

        // Play explosion particle effect near barrel tip
        ParticleSystemController system = GameController.Instance.explosionController;
        if (system != null)
        {
            Vector3 newPos = barrel.transform.position + barrel.transform.forward * 3f;
            system.Move(newPos);
            system.Rotate(barrel.transform.rotation);
            system.Stop();
            system.Play();
        }

        // Play smoke particle effect from barrel
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
        // Call the inherited input-check logic from BaseWeapon
        OnUpdate();
    }
}

