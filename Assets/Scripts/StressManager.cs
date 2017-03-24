using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressManager : MonoBehaviour
{

	public float redStress { get; private set; }

	public float redStressCapacity = 50f;
	public float redDecayUnitsPerSecond = 2f;

	// units per second
	public float colorAdaptionSecs = 0.1f;

	public const float capacitySizeFactor = 50;

	private Renderer rend;

	private Color currentColor = Color.white;
	private Color inverseColor = Color.black;

	public BreathingController breathingController;

	private const float lightFlashIntensity = 6;
	private const float lightFlashLength = 0.1f;
	private const float growthFactor = 0.2f;

	private float maxStressSinceGrowth = 0;

	void Start ()
	{
		rend = GetComponent<Renderer> ();
		rend.material.color = currentColor;
	}


	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.CompareTag ("Stressor")) {
			absorbStress (other.gameObject);
		}
	}

	void absorbStress (GameObject gameObject)
	{
		gameObject.SetActive (false);
		StressorController stressor = (StressorController)gameObject.GetComponent (typeof(StressorController));
		redStress += stressor.stressLevel;
	}

	void Update ()
	{
		UpdateStress ();
		UpdateBreathingSpeed ();
		UpdateColors ();
		UpdateSize ();
		updateLight ();
	}

	void UpdateColors ()
	{
		float redDelta = (redStress / redStressCapacity) - inverseColor.r;
		if (redDelta != 0) {
			float newRed = inverseColor.r + redDelta * Time.deltaTime / colorAdaptionSecs;
			inverseColor = new Color (newRed, inverseColor.g, inverseColor.b);
		}

		currentColor = new Color (1 - inverseColor.b - inverseColor.g, 1 - inverseColor.b - inverseColor.r, 1 - inverseColor.r - inverseColor.g);

		rend.material.color = currentColor;
	}

	void UpdateStress ()
	{
		redStress = Mathf.Min (redStress, redStressCapacity);

		if (redStress > maxStressSinceGrowth) {
			maxStressSinceGrowth = redStress;
		}

		if (redStress != redStressCapacity && redStress > 0) {
			redStress -= redDecayUnitsPerSecond * Time.deltaTime;
		} else {
			// TODO game over here?
		}

		if (redStress < 0) {
			triggerGrowth ();
			redStress = 0;
			maxStressSinceGrowth = 0;
		}
	}

	void UpdateBreathingSpeed ()
	{
		breathingController.setSpeedFactor (redStress / redStressCapacity);
	}

	void UpdateSize ()
	{
		float sizeRatio = redStressCapacity / capacitySizeFactor;
		sizeRatio = Mathf.Log (sizeRatio + 1) + 0.5f;
		breathingController.setBaseFactor (sizeRatio);
	}

	void updateLight ()
	{
		// TODO might need to put logic to pulse correctly in the future
		Light light = GetComponent<Light> ();
		if (light.intensity > 0) {
			light.intensity -= lightFlashIntensity * Time.deltaTime / lightFlashLength;
		}
	}

	void triggerGrowth ()
	{
		Light light = GetComponent<Light> ();
		light.intensity = maxStressSinceGrowth / redStressCapacity * lightFlashIntensity;
		redStressCapacity += maxStressSinceGrowth * growthFactor;
	}
}