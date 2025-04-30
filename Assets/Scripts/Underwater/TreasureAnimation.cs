using UnityEngine;
using System.Collections;

public class TreasureAnimation : MonoBehaviour
{
    public GameObject treasureChestPrefab;

     // === CONFIGURABLE PROPERTIES ===
    public int translateSpeed = 2;
    public int rotationSpeed = 50; 
    public int scaleAmount = 5;
    public float duration = 3f;

    void Start() {}

    void Update() {}

    //  1 of 10 percent chance to find treasure
    // Sets treasure chest on ship as active and has a treasure chest animation
    public void searchForTreasure(GameObject ship, Vector3 ammoTransform, Vector3 planetNormal, Vector3 planetCenter) {
        if (ship == null) {
            Debug.LogWarning("Could not access Ship");
        } else {
            if (Random.Range(0,10) == 1) {
                Transform treasureChest = ship.transform.Find("TresureChest");
                if (treasureChest == null) {
                    Debug.LogWarning("Could not ships TresureChest. Ship's name: " + ship.name);
                } else {
                    // Set treasure Chest on ship active
                    treasureChest.gameObject.SetActive(true);
                    // === ANIMATE CHEST ===
                    // Instantiate treasure chest
                    GameObject treasureInstance = Instantiate(treasureChestPrefab, ammoTransform, Quaternion.identity);
                    if (treasureChestPrefab == null) {
                        Debug.LogWarning("Could not instantiate the treasureChestPrefab from GameController.Instance");
                    } else {
                        // set the direction of the particle system to the planet normal
                        treasureInstance.transform.forward = planetNormal;
                        // start coroutine
                        StartCoroutine(RiseOfTheTreasure(treasureInstance, planetCenter));
                    }
                }
            }
        }
    }

    // Coroutine for treasure animation - rotate and raise
    private IEnumerator RiseOfTheTreasure(GameObject treasureInstance, Vector3 planetCenter) {
        float elapsedTime = 0f;

        Vector3 startPosition = treasureInstance.transform.position;
        Quaternion startRotation = treasureInstance.transform.rotation;
        Vector3 startScale = treasureInstance.transform.localScale;

        Vector3 targetScale = startScale * scaleAmount;

        // Animate for 2 seconds
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;  // Normalized time (0 to 1)

            // Scale
            // Interpolate the scale between initial and target scale
            treasureInstance.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            // Rotate
            treasureInstance.transform.Rotate(new Vector3(1,1,1) * rotationSpeed * Time.deltaTime);
            // Move
            // Get the normal vector from the sphere center to the object's position
            Vector3 directionFromCenter = treasureInstance.transform.position - planetCenter;
            // Normalize the direction vector to get the normal (unit vector)
            Vector3 normal = directionFromCenter.normalized;
            // Move the object away from the sphere by moving in the opposite direction of the normal
            treasureInstance.transform.position += normal * translateSpeed * Time.deltaTime;

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            yield return null; // Wait until the next frame
        }

        // Destroy after animation
        Destroy(treasureInstance);
    }
}