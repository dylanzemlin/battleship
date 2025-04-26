using UnityEngine;

public abstract class BaseWeaponClick : MonoBehaviour
{
    public GameObject ammoPrefab; // Prefab to use when spawning ammo (if applicable)
    public GameObject barrel; // Optional reference to the cannon's barrel

    public virtual void Fire()
    {
        Debug.LogWarning("BaseWeaponClick.Fire() called");
    }

    protected void OnUpdate()
    {
        // Wait until the player clicks the left mouse button
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        // Raycast from the camera to where the mouse clicked
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            return;
        }

        // Only proceed if the object clicked is this weapon
        if (hit.collider.gameObject != gameObject)
        {
            return;
        }

        // Call the fire method
        Fire();
    }
}
