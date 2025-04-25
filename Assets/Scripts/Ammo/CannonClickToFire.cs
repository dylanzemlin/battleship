using UnityEngine;

public class CannonClickToFire : MonoBehaviour
{
    public GameObject loadedAmmo;         // Ammo that was placed in the cannon
    public Transform targetShip;          // Where to fire at
    public float height = 5f;             // Arc height
    public float duration = 2f;           // Flight time

    private void OnMouseDown()
    {
        if (loadedAmmo != null && targetShip != null)
        {
            Debug.Log("Cannon clicked — firing ammo!");

            // Detach the ammo (in case it's parented to the cannon)
            loadedAmmo.transform.SetParent(null);

            // Add the mover and fire it
            var mover = loadedAmmo.AddComponent<AmmoBezierMover>();
            mover.targetPoint = targetShip;
            mover.height = height;
            mover.duration = duration;
            mover.FireFrom(loadedAmmo.transform.position);

            // Clear loaded ammo so this cannon doesn't fire again unless reloaded
            loadedAmmo = null;
        }
        else
        {
            Debug.LogWarning("No loaded ammo or target assigned!");
        }
    }
}
