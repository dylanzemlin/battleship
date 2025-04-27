using UnityEngine; // Import Unity's core engine features (GameObjects, Input, Camera, Physics)

public abstract class BaseWeaponClick : MonoBehaviour
{
    // === CONFIGURABLE PROPERTIES ===

    public GameObject ammoPrefab; // A reference to an ammo prefab (could be used for spawning or firing later)
    public GameObject barrel;     // Optional reference to the barrel of the cannon (used for effects positioning)

    // === ABSTRACT FIRE LOGIC ===

    public virtual void Fire()
    {
        // By default, just log a warning if Fire() is called on the base class
        Debug.LogWarning("BaseWeaponClick.Fire() called");
        // Child classes (like clickToFireCannon) will override this with actual firing behavior
    }

    // === CLICK HANDLING FUNCTION ===

    protected void OnUpdate()
    {
        // Wait until the player clicks the left mouse button (button index 0 = left click)
        if (!Input.GetMouseButtonDown(0))
        {
            return; // If no click, do nothing and exit early
        }

        // If clicked, cast a ray from the camera toward where the mouse clicked
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Perform the raycast into the scene, checking what the ray hits
        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            return; // If ray hits nothing, exit
        }

        // Only continue if the object clicked is THIS weapon (this GameObject)
        if (hit.collider.gameObject != gameObject)
        {
            return; // If clicked object is not me, do nothing
        }

        // If all checks pass (left click, hit something, and hit ME), then fire the weapon
        Fire();
    }
}
