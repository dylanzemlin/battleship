using System.Collections.Generic;
using UnityEngine;

public class Cannon : BaseWeapon
{
    public override void Fire()
    {
        GameObject ship = this.transform.parent.gameObject.transform.parent.gameObject;
        bool isSelected = ship.GetComponent<PirateShip>().isSelected;

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
            scrpt.Fire(barrel.transform.forward, 50f);

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
        }
    }

    private void Update()
    {
        // Perform the base weapon update
        OnUpdate();
    }
}