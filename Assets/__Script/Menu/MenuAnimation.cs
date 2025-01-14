using UnityEngine;

public class MenuAnimation : MonoBehaviour
{
    [Header("Rocking Settings")]
    public GameObject rockingChair; // The chair to animate
    public float rockingAngle = 15f; // Maximum angle of rocking
    public float rockingSpeed = 2f; // Speed of the rocking motion


    [Header("Flickering Light Settings")]
    public Light pointLight; // Assign the light component in the Inspector
    public float minIntensity = 0.5f; // Minimum light intensity
    public float maxIntensity = 2f;  // Maximum light intensity
    public float flickerSpeed = 5f;  // Speed of flickering

    private float currentAngle;

    void Update()
    {
        RockingBackAndForth();
        FlickeringLight();
    }

    void RockingBackAndForth()
    {
        // Calculate the rocking angle using a sine wave
        currentAngle = Mathf.Sin(Time.time * rockingSpeed) * rockingAngle;

        // Apply the rotation to the rocking chair
        if (rockingChair != null)
        {
            rockingChair.transform.localRotation = Quaternion.Euler(currentAngle, 180f, 0f);
        }
    }

    void FlickeringLight()
    {
        if (pointLight != null)
        {
            // Generate a smooth random value using Perlin Noise
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
            pointLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
        else
        {
            Debug.LogWarning("Point Light is not assigned!");
        }
    }
}
