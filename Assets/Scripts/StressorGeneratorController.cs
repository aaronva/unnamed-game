using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressorGeneratorController : MonoBehaviour
{
	public GameManager GameManager;
	public StressorController stressorTemplate;
	public float generatedRadius = 20f;

	//	private int generatedCount = 0;
	[System.Obsolete]
	private float lastGeneratedTime;
	private float nextGeneratedTime = 0;

	private float difficultyValue = float.MinValue;

//	public enum DifficultyLevel {Easy = 0, Normal = 1, Hard = 2}

	// Update is called once per frame
	void Update ()
	{
		difficultyValue = GameManager.ComputeTimeBasedDifficulty ();

		Debug.Log (difficultyValue);

		if (ShouldGenerate ()) {
			float angle = ComputeAngle ();
			Quaternion quaternion = Quaternion.AngleAxis (angle, Vector3.up);

			Vector3 spawnPoint = quaternion * new Vector3 (generatedRadius, 0, generatedRadius);

			float stressLevel = ComputeStressLevel ();

			StressorController stressor = (StressorController)Instantiate (stressorTemplate, spawnPoint, Quaternion.identity);

			stressor.applyForce (spawnPoint.normalized * -1 * ComputeInitialForce ());
			stressor.setStressLevel (stressLevel);

			// Kill the created stressors in 5 seconds.
			Destroy (stressor.gameObject, 3);

			nextGeneratedTime = ComputeNextGenerationTime ();
		}
	}

	// Skeleton to allow more logic to be put in here
	private float ComputeInitialForce ()
	{
		return 140f;
	}

	private bool ShouldGenerate ()
	{
		// I don't like how much framerate can affect this (effectively spawn rate is capped at framerate). 
		// It's minor enough that finding another solution is out of scope for this project, but I should 
		// keep it in the back of my mind for future projects.
		return Time.time > nextGeneratedTime;
	}

	private float ComputeAngle ()
	{
		const float difficultyAtFullRadius = 40f;
		const int minRange = 30;
		const int midpoint = 360 / 2;

		if (difficultyValue >= difficultyAtFullRadius) {
			return Random.Range (0, 359);
		}

		float range = (midpoint - minRange) * difficultyValue / difficultyAtFullRadius + minRange;

		return Random.Range (midpoint - range, midpoint + range);
	}

	private float ComputeStressLevel ()
	{
		float smallStressorWeight = 40;
		float mediumStressorWeight = difficultyValue > 40 ? (difficultyValue - 40) : 0;
		float largeStressorWeight = difficultyValue > 80 ? (difficultyValue - 80) : 0;

		float totalStressorWeight = smallStressorWeight + mediumStressorWeight + largeStressorWeight;

		int difficultySizeBoost = (int)difficultyValue / 40;

		float randomGen = Random.Range (0, totalStressorWeight);

		if (randomGen < smallStressorWeight) {
			return 5 + difficultySizeBoost;
		} else if (randomGen < mediumStressorWeight) {
			return 7 + difficultySizeBoost;
		} else {
			return 10 + difficultySizeBoost;
		}
	}

	private float ComputeNextGenerationTime ()
	{
		const float varianceFactor = 0.2f;
		const float maxBaseline = 2.5f;

		float baselineTimeDelay = Mathf.Min(50 / difficultyValue, maxBaseline);
//		float baselineFrequency = currentDifficulty / 50;

		float minTime = baselineTimeDelay * (1 - varianceFactor);
		float maxTime = baselineTimeDelay * (1 + varianceFactor);

		float timeUntilNext = Random.Range (minTime, maxTime);

		return Time.time + timeUntilNext;
	}
}
