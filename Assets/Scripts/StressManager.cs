using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressManager : MonoBehaviour
{

	public float redStress { get; private set; }

	public float redStressCapacity = 50f;
//	public float redDecayUnitsPerSecond = 2f;
	public const float redStressDecayRatio = 0.05f;

	// units per second
	public float maxColorAdaptionSpeed = 0.1f;

	public const float capacitySizeFactor = 50;

	private Renderer rend;

	private Color currentColor = Color.white;
	private Color inverseColor = Color.black;

	public BreathingController breathingController;

	private const float lightPeakIntensity = 3;
	private const float lightIntensityIncreaseDuration = 0.2f;
	private const float lightIntensityDecreaseDuration = 0.5f;

	private bool lightIntensityIsIncreasing = false;
	private float lightIntensityTarget;

	public const float growthFactor = 0.2f;

	private float maxStressSinceGrowth = 0;

	void Start ()
	{
		rend = GetComponent<Renderer> ();
		rend.material.color = currentColor;
		redStress = 0;
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
			float newRed = inverseColor.r + Mathf.Min(redDelta * Time.deltaTime, maxColorAdaptionSpeed);
			inverseColor = new Color (newRed, inverseColor.g, inverseColor.b);
		}

		currentColor = new Color (1 - inverseColor.b - inverseColor.g,
			1 - inverseColor.b - inverseColor.r, 1 - inverseColor.r - inverseColor.g);

		rend.material.color = currentColor;
	}

	void UpdateStress ()
	{
		if (redStress > maxStressSinceGrowth) {
			maxStressSinceGrowth = redStress;
		}

		if (redStress < redStressCapacity && redStress > 0) {
			redStress -= redStressCapacity * redStressDecayRatio * Time.deltaTime;
		} else {
			// TODO game over here?
		}

		if (redStress <= 0 && maxStressSinceGrowth > 0) {
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

		if (lightIntensityIsIncreasing) {
			light.intensity += Time.deltaTime * lightIntensityTarget / lightIntensityIncreaseDuration;
			lightIntensityIsIncreasing = light.intensity < lightIntensityTarget;
		} else if (light.intensity > 0) {
			light.intensity -= Time.deltaTime * lightIntensityTarget;
		}
	}

	void triggerGrowth ()
	{
		triggerIntensityIncrease(maxStressSinceGrowth / redStressCapacity);
		redStressCapacity += maxStressSinceGrowth * growthFactor;
	}

	void triggerIntensityIncrease (float percentage)
	{
		lightIntensityIsIncreasing = true;
		// TODO possible log here?
		lightIntensityTarget = percentage * lightPeakIntensity;
	}
}