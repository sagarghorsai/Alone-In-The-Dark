using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleFromMicrophone : MonoBehaviour
{
    public AudioSource source;
    public Vector3 minScale = new Vector3(1, 1, 1);
    public Vector3 maxScale = new Vector3(3, 3, 3);
    public AudioLoudnessDetection detector;

    public float loudnessSensiblity = 100f;
    public float threshold = 0.1f;
    public float smoothTime = 0.2f; // Smoothing time for scale changes

    public float loudness;

    public Image image;

    private Vector3 currentVelocty;

    private void Update()
    {
        if (detector == null)
        {
            Debug.LogError("AudioLoudnessDetection is not assigned!");
            return;
        }

        loudness = detector.GetLoudnessFromMicrophone() * loudnessSensiblity;

        if (loudness < threshold)
            loudness = 0;

        loudness = Mathf.Clamp01(loudness);

        image.fillAmount = loudness;


        // Smoothly scale the object based on loudness
        //transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.Lerp(minScale, maxScale, loudness), ref currentVelocity, smoothTime);
    }
}
