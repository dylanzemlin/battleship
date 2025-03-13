using System.Collections.Generic;
using UnityEngine;

public class Cannon : BaseWeapon
{
    public override void Fire()
    {
        // TODO: Create ammo
    }

    private void Update()
    {
        // Perform the base weapon update
        OnUpdate();

        // TODO: Either simulate the ammo here, or create a AmmoController script (or something similar I dunno)
    }
}