using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingController : MonoBehaviour
{

	public float fluctuation = 0.05f;
	public float minSpeed = 1;
	public float maxSpeed = 20;

	public float speed { get; private set; }

	private float baseFactor;
	private float passedEffectiveTime = 0;

	private bool isPaused = false;

	// Update is called once per frame
	void Update ()
	{
		if (!isPaused) {
			passedEffectiveTime += speed * Time.deltaTime;
			float sizeFactor = (Mathf.Sin (Time.time + passedEffectiveTime) * fluctuation) + 1 - fluctuation;
			this.transform.localScale = new Vector3 (sizeFactor, sizeFactor, sizeFactor) * baseFactor;
		}
	}

	public void pauseBreathing ()
	{
		isPaused = true;
	}

	public void resumeBreathing ()
	{
		float sizeFactor = this.transform.localScale.x / baseFactor;
		passedEffectiveTime = Mathf.Asin((sizeFactor - 1 + fluctuation) / fluctuation) - Time.time;

		isPaused = false;
	}

	public void setSpeedFactor (float factor)
	{
		speed = (maxSpeed - minSpeed) * factor + minSpeed;
	}

	public void setBaseFactor (float factor)
	{
		baseFactor = factor;
	}
}