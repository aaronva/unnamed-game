using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AmoebaManager : MonoBehaviour
{
	public StressorController stressorTemplate;

	public float stressLevel { get; private set; }

	public float maxStressLevel = 50f;
	// TODO remove this
	public const float stressDecayRatio = 0.1f;

	// Percent per second
	public const float colorAdaptionSpeed = 1f;
	public const float capacitySizeFactor = 50;

	private Renderer rend;

	private Color inverseColor = Color.black;

	public BreathingController breathingController;

	private const float lightPeakIntensity = 3;
	private const float lightIntensityIncreaseDuration = 0.2f;
	private const float lightIntensityDecreaseDuration = 0.5f;

	private bool lightIntensityIsIncreasing = false;
	private float lightIntensityTarget;
	private bool hasOutburstSinceLastGrowth = false;

	public const float growthFactor = 0.2f;

	private float maxStressSinceGrowth = 0;

	private List<GameObject> listeners = new List<GameObject> ();

	private bool isLashingOut = false;
	private float lastOutburstTime = 0;
	public const float baseOutburstFrequency = 3;
	public const int numberStressorsProducedDuringOutburst = 6;

	void Start ()
	{
		rend = GetComponent<Renderer> ();
		rend.material.color = Color.white;
		stressLevel = 0;
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.CompareTag ("Stressor")) {
			AbsorbStress (other.gameObject);
		}
	}

	void AbsorbStress (GameObject gameObject)
	{
		StressorController stressor = (StressorController)gameObject.GetComponent<StressorController> ();
		if (stressor.creator == this.gameObject) {
			return;
		}
			
		stressLevel += stressor.stressLevel;
		Destroy (gameObject);
	}

	void Update ()
	{
		UpdateLashout ();
		UpdateStress ();
		UpdateBreathingSpeed ();
		UpdateColors ();
		UpdateSize ();
		UpdateLight ();
	}

	void UpdateColors ()
	{
		float redDelta = (stressLevel / maxStressLevel) - inverseColor.r;
		if (redDelta != 0) {
			float newRed = inverseColor.r + Mathf.Min (colorAdaptionSpeed * Time.deltaTime, redDelta);
			inverseColor = new Color (newRed, inverseColor.g, inverseColor.b);
		}

		Color currentColor = new Color (1 - inverseColor.b - inverseColor.g,
			                     1 - inverseColor.b - inverseColor.r, 1 - inverseColor.r - inverseColor.g);

		rend.material.color = currentColor;
	}

	void UpdateStress ()
	{
		if (stressLevel >= maxStressLevel) {
			MaxCapacityReached ();
		}

		if (isLashingOut) {
			// special logic will be done when lashing out
			return;
		}

		if (stressLevel > maxStressSinceGrowth) {
			maxStressSinceGrowth = stressLevel;
		}

		if (stressLevel < maxStressLevel && stressLevel > 0) {
			stressLevel -= maxStressLevel * stressDecayRatio * Time.deltaTime;
		}

		if (stressLevel <= 0 && maxStressSinceGrowth > 0) {
			TriggerGrowth ();
			stressLevel = 0;
			maxStressSinceGrowth = 0;
			hasOutburstSinceLastGrowth = false;
		}
	}

	void UpdateBreathingSpeed ()
	{
		breathingController.setSpeedFactor (stressLevel / maxStressLevel);
	}

	void UpdateSize ()
	{
		float sizeRatio = maxStressLevel / capacitySizeFactor;
		sizeRatio = Mathf.Log (sizeRatio + 1) + 0.5f;
		breathingController.setBaseFactor (sizeRatio);
	}

	void UpdateLight ()
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

	void UpdateLashout ()
	{
		if (!isLashingOut) {
			// Don't need to do work if we aren't lashing out.
			return;
		}

		float timeSinceLastOutburst = Time.time - lastOutburstTime;
		float timeBetweenOutbursts = 1 / baseOutburstFrequency;

		if (timeSinceLastOutburst > timeBetweenOutbursts) {
			TriggerOutburst ();
			lastOutburstTime = Time.time;
		}

		float fluctuationScale = timeSinceLastOutburst / timeBetweenOutbursts * -2 * breathingController.fluctuation + 1;
		this.transform.localScale = Vector3.one * breathingController.baseFactor * fluctuationScale;
	}

	void TriggerGrowth ()
	{
		float outburstPenalty = hasOutburstSinceLastGrowth ? 0.5f : 1f;
		TriggerIntensityIncrease (maxStressSinceGrowth / maxStressLevel * outburstPenalty);
		maxStressLevel += maxStressSinceGrowth * growthFactor;
	}

	void TriggerIntensityIncrease (float percentage)
	{
		lightIntensityIsIncreasing = true;
		// TODO possible log here?
		lightIntensityTarget = percentage * lightPeakIntensity;
	}

	private void MaxCapacityReached ()
	{
		if (!isLashingOut) {
			NotifyListeners ();
		}
		stressLevel = maxStressLevel;
		BeginLashout ();
	}

	private void BeginLashout ()
	{
		isLashingOut = true;
		breathingController.pauseBreathing ();
	}

	private void EndLashout ()
	{
		isLashingOut = false;
		breathingController.resumeBreathing ();
	}

	private void TriggerOutburst ()
	{
		const float increament = 360f / numberStressorsProducedDuringOutburst;
		float initialAngle = 2 * stressLevel / maxStressLevel * increament;
		float outburstStressLevel = (float)maxStressLevel / 8;

		for (int i = 0; i < numberStressorsProducedDuringOutburst; i++) {
			float angle = initialAngle + increament * i;
			Quaternion quaternion = Quaternion.AngleAxis (angle, Vector3.up);

			Vector3 forceUnitVector = quaternion * new Vector3 (1, 0, 1);

			StressorController stressor = (StressorController)
				Instantiate (stressorTemplate, this.transform.position, Quaternion.identity);
			stressor.applyForce (forceUnitVector * 220); // TODO replace hard coded force scalor
			stressor.creator = this.gameObject;
			stressor.setStressLevel (2 * outburstStressLevel);

			// TODO have an out-of-bounds area to cleanup stressors instead
			Destroy (stressor.gameObject, 1);
		}

		// this should probably just be part of the breathingController
		this.transform.localScale = Vector3.one * breathingController.baseFactor * (1 + breathingController.fluctuation);
		stressLevel -= outburstStressLevel;

		if (stressLevel < maxStressLevel / 2) {
			EndLashout ();
		}
	}

	private void NotifyListeners ()
	{
		foreach (GameObject target in listeners) {
			ExecuteEvents.Execute<IMaxStressTarget> (target, null, (x, y) => x.MaxStressReached (this));
		}
	}

	public void RegisterListener (GameObject target)
	{
		listeners.Add (target);
	}
}