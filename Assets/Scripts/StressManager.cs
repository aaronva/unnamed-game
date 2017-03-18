using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressManager : MonoBehaviour
{

	public float redStress { get; private set; }

	public int redStressCapacity = 50;
	public int redDecayUnitsPerSecond = 2;

	// units per second
	public float colorAdaptionSecs = 0.1f;

	public const float capacitySizeFactor = 50;

	private Renderer rend;

	private Color currentColor = Color.white;
	private Color inverseColor = Color.black;

	public BreathingController breathingController;

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
		UpdateRedStress ();
		UpdateBreathingSpeed ();
		UpdateColors ();
		UpdateSize ();
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

	void UpdateRedStress ()
	{
		redStress = Mathf.Min (redStress, redStressCapacity);

		if (redStress != redStressCapacity) {
			redStress -= redDecayUnitsPerSecond * Time.deltaTime;
		} else {
			// TODO game over here?
		}

		redStress = Mathf.Max (redStress, 0);
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
}