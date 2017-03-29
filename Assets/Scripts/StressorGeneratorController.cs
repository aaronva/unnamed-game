using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressorGeneratorController : MonoBehaviour
{
	public StressorController stressorTemplate;
	public float generatedRadius = 20f;

	private static int generatedCount = 0;

	// Update is called once per frame
	void Update ()
	{
		if (shouldGenerate ()) {
			float angle = getRandomAngle ();
			Quaternion quaternion = Quaternion.AngleAxis (angle, Vector3.up);

			Vector3 spawnPoint = quaternion * new Vector3 (generatedRadius, 0, generatedRadius);

			StressorController stressor = (StressorController)Instantiate (stressorTemplate, spawnPoint, Quaternion.identity);

			stressor.applyForce (spawnPoint.normalized * -1 * computeInitialForce ());

			generatedCount++;
		}
	}

	// Skeleton to allow more logic to be put in here
	private float computeInitialForce ()
	{
		return 200f;
	}

	private bool shouldGenerate ()
	{
		// For now just always return once a second
		return Time.time > generatedCount;
	}

	private float getRandomAngle ()
	{
		return Random.Range (0, 359);
	}
}
