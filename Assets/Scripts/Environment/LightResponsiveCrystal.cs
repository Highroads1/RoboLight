using UnityEngine;

public enum CrystalColor { White, Red, Blue, Green, Yellow }

public class LightResponsiveCrystal : MonoBehaviour
{
    [Header("Crystal Settings")]
    public CrystalColor crystalColor = CrystalColor.White;
    public float activationThreshold = 0.1f; // Minimum light intensity needed to trigger
    
    [Header("Current Status")]
    public float currentIllumination = 0f; // 0 to 1 scale of how lit it is
    public bool isActivated = false;

    // References to the player's flashlight
    private Transform flashlightTransform;
    private Light flashlightComponent;

    void Start()
    {
        // Find the light source in the scene. 
        // For a production game, a PlayerManager or Event system is better, 
        // but this works perfectly for a quick 10-level prototype.
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light l in allLights)
        {
            if (l.type == LightType.Spot)
            {
                flashlightComponent = l;
                flashlightTransform = l.transform;
                break;
            }
        }

        if (flashlightTransform == null)
        {
            Debug.LogError("No Spotlight found in the scene for the Crystal to detect!");
        }
    }

    void Update()
    {
        if (flashlightTransform == null || flashlightComponent == null) return;

        currentIllumination = CalculateLightIntensity();

        // Check if the illumination meets the threshold and matches conditions
        if (currentIllumination >= activationThreshold)
        {
            isActivated = true;
            // TODO: Scale up your activation timer, change crystal emission color, etc.
        }
        else
        {
            isActivated = false;
        }
    }

    private float CalculateLightIntensity()
    {
        // 1. Check Distance
        Vector3 dirToCrystal = transform.position - flashlightTransform.position;
        float distance = dirToCrystal.magnitude;
        float maxRange = flashlightComponent.range;

        // If out of physical range, there is no light hitting it
        if (distance > maxRange) return 0f;

        // 2. Check Angle
        dirToCrystal.Normalize(); // Normalize for vector angle calculation
        float angleToCrystal = Vector3.Angle(flashlightTransform.forward, dirToCrystal);
        float halfSpotAngle = flashlightComponent.spotAngle / 2f;

        // If outside the cone of the spotlight, no light hits it
        if (angleToCrystal > halfSpotAngle) return 0f;

        // 3. Calculate Tapering (Falloff)
        // Distance falloff: 1 at close range, 0 at maximum range
        float distanceFactor = 1f - (distance / maxRange);

        // Angular falloff: 1 at the dead center of the beam, 0 at the very edge of the cone
        float angleFactor = 1f - (angleToCrystal / halfSpotAngle);

        // Combine both factors for a smooth, natural light attenuation value (0 to 1)
        return distanceFactor * angleFactor;
    }

    // Optional: Draw the detection boundaries in the Unity Editor for debugging
    private void OnDrawGizmos()
    {
        if (isActivated)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 1.2f);
        }
    }
}