using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AmoebaManager : MonoBehaviour
{
	public StressorController stressorTemplate;

	public float StressLevel { get; private set; }

	public float MaxStressLevel = 50f;
	// TODO remove this
	public const float StressDecayRatio = 0.1f;

	// Percent per second
	public const float ColorAdaptionSpeed = 1f;
	public const float CapacitySizeFactor = 50;

	private Renderer rend;

	private Color inverseColor = Color.black;

	public BreathingController BreathingController;

	private const float lightPeakIntensity = 3;
	private const float lightIntensityIncreaseDuration = 0.2f;
	private const float lightIntensityDecreaseDuration = 0.5f;

	private bool lightIntensityIsIncreasing = false;
	private float lightIntensityTarget;
	private bool hasOutburstSinceLastGrowth = false;

	public const float GrowthFactor = 0.1f;

	private float maxStressSinceGrowth = 0;

	private List<GameObject> listeners = new List<GameObject> ();

	public bool IsLashingOut { get; private set; }

	private float lastOutburstTime = 0;
	public const float BaseOutburstFrequency = 3;
	public const int NumberStressorsProducedDuringOutburst = 6;

	private float initialAngle = 0f;

	void Start ()
	{
		rend = GetComponent<Renderer> ();
		rend.material.color = Color.white;
		StressLevel = 0;
		IsLashingOut = false;
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
			
		StressLevel += stressor.stressLevel;
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
		float redDelta = (StressLevel / MaxStressLevel) - inverseColor.r;
		if (redDelta != 0) {
			float newRed = inverseColor.r + Mathf.Min (ColorAdaptionSpeed * Time.deltaTime, redDelta);
			inverseColor = new Color (newRed, inverseColor.g, inverseColor.b);
		}

		Color currentColor = new Color (1 - inverseColor.b - inverseColor.g,
			                     1 - inverseColor.b - inverseColor.r, 1 - inverseColor.r - inverseColor.g);

		rend.material.color = currentColor;
	}

	void UpdateStress ()
	{
		if (StressLevel >= MaxStressLevel) {
			MaxCapacityReached ();
		}

		if (IsLashingOut) {
			// special logic will be done when lashing out
			return;
		}

		if (StressLevel > maxStressSinceGrowth) {
			maxStressSinceGrowth = StressLevel;
		}

		if (StressLevel < MaxStressLevel && StressLevel > 0) {
			StressLevel -= MaxStressLevel * StressDecayRatio * Time.deltaTime;
		}

		if (StressLevel <= 0 && maxStressSinceGrowth > 0) {
			TriggerGrowth ();
			StressLevel = 0;
			maxStressSinceGrowth = 0;
			hasOutburstSinceLastGrowth = false;
		}
	}

	void UpdateBreathingSpeed ()
	{
		BreathingController.setSpeedFactor (StressLevel / MaxStressLevel);
	}

	void UpdateSize ()
	{
		float sizeRatio = MaxStressLevel / CapacitySizeFactor;
		sizeRatio = Mathf.Log (sizeRatio + 1) + 0.5f;
		BreathingController.setBaseFactor (sizeRatio);
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
		if (!IsLashingOut) {
			// Don't need to do work if we aren't lashing out.
			return;
		}

		float timeSinceLastOutburst = Time.time - lastOutburstTime;
		float timeBetweenOutbursts = 1 / BaseOutburstFrequency;

		if (timeSinceLastOutburst > timeBetweenOutbursts) {
			TriggerOutburst ();
			lastOutburstTime = Time.time;
		}

		float fluctuationScale = timeSinceLastOutburst / timeBetweenOutbursts * -2 * BreathingController.fluctuation + 1;
		this.transform.localScale = Vector3.one * BreathingController.baseFactor * fluctuationScale;
	}

	void TriggerGrowth ()
	{
		float outburstPenalty = hasOutburstSinceLastGrowth ? 0.5f : 1f;
		float oldMaxStress = MaxStressLevel;
		MaxStressLevel += Mathf.Pow (maxStressSinceGrowth * outburstPenalty * GrowthFactor, 2);
		TriggerIntensityIncrease (1f - oldMaxStress / MaxStressLevel);
	}

	void TriggerIntensityIncrease (float percentage)
	{
		lightIntensityIsIncreasing = true;
		// TODO possible log here?
		lightIntensityTarget = percentage * lightPeakIntensity;
	}

	private void MaxCapacityReached ()
	{
		if (!IsLashingOut) {
			NotifyListeners ();
		}
		StressLevel = MaxStressLevel;
		BeginLashout ();
	}

	private void BeginLashout ()
	{
		IsLashingOut = true;
		BreathingController.pauseBreathing ();
		initialAngle = -1f;
	}

	private void EndLashout ()
	{
		IsLashingOut = false;
		BreathingController.resumeBreathing ();
	}

	private void TriggerOutburst ()
	{
		const float increament = 360f / NumberStressorsProducedDuringOutburst;
		float outburstStressLevel = (float)MaxStressLevel / 8;

		if (initialAngle < 0) {
			initialAngle = Random.Range (0f, increament);
		}

		for (int i = 0; i < NumberStressorsProducedDuringOutburst; i++) {
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
		this.transform.localScale = Vector3.one * BreathingController.baseFactor * (1 + BreathingController.fluctuation);
		StressLevel -= outburstStressLevel;

		if (StressLevel < MaxStressLevel / 2) {
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