using System.Collections.Generic;
using UnityEngine;

public class Cannon : BaseWeapon
{
    public override void Fire()
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
    }

    private void Update()
    {
        // Perform the base weapon update
        OnUpdate();
    }
}