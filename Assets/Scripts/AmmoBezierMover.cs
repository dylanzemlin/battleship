using UnityEngine;

public class AmmoBezierMover : MonoBehaviour
{
    public Transform targetPoint;
    public float height = 5f;
    public float duration = 2f;
    public GameObject ammoPrefabToReload;      // The prefab to reload onto the deck
    public Transform reloadPointOnDeck;        // The deck location where ammo reappears
    private float t = 0f;
    private bool isFired = false;
    private Vector3 p0, p1, p2;

    public void FireFrom(Vector3 startPosition)
    {
        p0 = startPosition;
        p2 = targetPoint.position;
        p1 = (p0 + p2) / 2f + Vector3.up * height;
        t = 0f;
        isFired = true;
    }

    void Update()
    {
        if (!isFired) return;

        t += Time.deltaTime / duration;
        if (t > 1f)
        {
            t = 1f;
            isFired = false;
            Debug.Log("Ammo hit the target!");
        }

        Vector3 pos = Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
        transform.position = pos;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        string targetName = collision.gameObject.name;

        switch (targetName)
        {
            case "targetMast":
                Debug.Log("Hit the mast!");
                var mast = collision.gameObject.GetComponent<MastFallOnHit>();
                if (mast != null)
                    mast.TriggerFall();

                break;

            case "targetStarboardStern":
                Debug.Log("Hit the ship (starboard stern)!");
                // TODO: Add damage logic or animation
                break;

            case "targetLand":
                Debug.Log("Hit land!");
                // TODO: Add explosion or crater effect
                break;

            case "targetWater":
                Debug.Log("Hit the water!");
                // TODO: Play splash particles / sound
                break;

            default:
                Debug.Log("Hit something else: " + targetName);
                break;
        }
        
        // Reload logic: spawn new ammo back on deck
        if (ammoPrefabToReload != null && reloadPointOnDeck != null)
        {
            Instantiate(ammoPrefabToReload, reloadPointOnDeck.position, reloadPointOnDeck.rotation);
            Debug.Log("New ammo reloaded on deck!");
        }

Destroy(gameObject);

        // Optionally destroy ammo after any hit
        Destroy(gameObject);
    }

}
