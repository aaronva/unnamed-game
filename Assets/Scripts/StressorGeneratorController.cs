using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressorGeneratorController : MonoBehaviour
{
	public GameManager GameManager;
	public StressorController stressorTemplate;
	public MainCameraController mainCamera;
	public float defaultGenerateRadius = 20f;


	private float nextGeneratedTime = 0;

	void Update ()
	{
		if (ShouldGenerate ()) {
			float angle = ComputeAngle ();
			float stressLevel = ComputeStressLevel ();

			Quaternion quaternion = Quaternion.AngleAxis (angle, Vector3.up);

			float generationRadius = mainCamera.MaxDistanceOutsideOfCamera;
			Vector3 spawnPoint = quaternion * new Vector3 (generationRadius, 0, generationRadius);

			StressorController stressor = (StressorController)Instantiate (stressorTemplate, spawnPoint, Quaternion.identity);

			stressor.applyForce (spawnPoint.normalized * -1 * ComputeInitialForce ());
			stressor.setStressLevel (stressLevel);

			nextGeneratedTime = ComputeNextGenerationTime ();
		}
	}

	// Skeleton to allow more logic to be put in here
	private float ComputeInitialForce ()
	{
		const float baseForce = 140f;
		const float difficultyFactor = 1f / 4f;

		return baseForce + GameManager.CurrentDifficulty * difficultyFactor;
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

		if (GameManager.CurrentDifficulty >= difficultyAtFullRadius) {
			return Random.Range (0, 359);
		}

		float range = (midpoint - minRange) * GameManager.CurrentDifficulty / difficultyAtFullRadius + minRange;

		return Random.Range (midpoint - range, midpoint + range);
	}

	private float ComputeStressLevel ()
	{
		float smallStressorWeight = 15;
		float mediumStressorWeight = GameManager.CurrentDifficulty > 15 ? (GameManager.CurrentDifficulty - 15) : 0;
		float largeStressorWeight = GameManager.CurrentDifficulty > 45 ? (GameManager.CurrentDifficulty - 45) : 0;

		float totalStressorWeight = smallStressorWeight + mediumStressorWeight + largeStressorWeight;

		int difficultySizeBoost = (int)GameManager.CurrentDifficulty / 30;

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
		const float maxNextGenerationTime = 2.5f;

		const float minFactor = 1f - varianceFactor;
		const float maxFactor = 1f + varianceFactor;

		float timeDelay = 20 / GameManager.CurrentDifficulty;

		float minTime = timeDelay * minFactor;
		float maxTime = timeDelay * maxFactor;

		float timeUntilNext = Random.Range (minTime, maxTime);
		timeUntilNext = Mathf.Min (maxNextGenerationTime, timeUntilNext);

		return Time.time + timeUntilNext;
	}
}
