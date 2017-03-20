using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressorGeneratorController : MonoBehaviour
{

	public float generatedRadius = 20f;

	private static int generatedCount = 0;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (shouldGenerate ()) {

		}
	}

	private bool shouldGenerate ()
	{
		// For now just always return once a second
		return Time.time > generatedCount;
	}

	private float getRandomAngle() {
		return Random.Range (0, 359);
	}
}
