using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public static CameraShake Instance;
	public bool start = false;
	public AnimationCurve curve;
	public float duration = 1f;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (start)
		{
			start = false;
			StartCoroutine(Shake());
		}
	}

	public void TriggerShake()
	{
		StartCoroutine(Shake());
	}

	IEnumerator Shake()
	{
		Vector3 originalLocalPos = transform.localPosition;
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float strength = curve.Evaluate(elapsedTime / duration);
			transform.localPosition = originalLocalPos + Random.insideUnitSphere * strength;
			yield return null;
		}

		transform.localPosition = originalLocalPos;
	}

}