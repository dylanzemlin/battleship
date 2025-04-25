using UnityEngine;

public class Cannon : BaseWeapon
{
    private Animator m_Animator;
    private float rawX;

    public override void Fire()
    {
        GameObject ship = this.transform.parent.gameObject.transform.parent.gameObject;
        bool isSelected = ship.GetComponent<PirateShip>().isSelected;
        
        m_Animator = this.GetComponent<Animator>();

        if (isSelected)
        {
            // Spawn a ammo prefab at the cannon's barrels position and rotation
            GameObject ammo = Instantiate(ammoPrefab, barrel.transform.position, barrel.transform.rotation);

            BaseAmmo scrpt = ammo.GetComponent<BaseAmmo>();
            if (scrpt == null)
            {
                Debug.LogError("BaseAmmo component not found on ammo prefab.");
                return;
            }

            scrpt.planetCenter = GameObject.Find("Planet").transform; // Assuming the planet is named "Planet"
            scrpt.Fire(barrel.transform.forward, 50f, ship);

            if (rawX > -52f)
                m_Animator.SetTrigger("Fire");
            else
                m_Animator.SetTrigger("Fire Up");

            // 
            ParticleSystemController system = GameController.Instance.explosionController;
            if (system == null)
            {
                Debug.LogError("ParticleSystemController not found in GameController.");
                return;
            }

            // Create a new transform with +3 z
            Vector3 newPos = barrel.transform.position + barrel.transform.forward * 3f;
            system.Move(newPos);
            system.Rotate(barrel.transform.rotation);
            system.Stop();
            system.Play();

            // Now do the smoke animation
            ParticleSystem smoke = GameController.Instance.smokeBurst;
            if (smoke == null)
            {
                Debug.LogError("Smoke particle system not found in GameController.");
                return;
            }

            smoke.transform.position = newPos;
            smoke.transform.position += -transform.forward * 4f;
            smoke.transform.position += -transform.up * 2f;
            smoke.transform.forward = -transform.forward;
            smoke.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
            smoke.Play();            
        }
    }

    private void Update()
    {
        // Perform the base weapon update
        OnUpdate();

        rawX = this.transform.GetChild(0).transform.GetChild(1).transform.localEulerAngles.x;

        if (rawX > 180f)
        {
            rawX -= 360f;
        }
    }
}