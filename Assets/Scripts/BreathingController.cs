using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingController : MonoBehaviour {

	public float fluctuation = 0.05f;
	public float minSpeed = 1;
	public float maxSpeed = 20;

	private float speed;
	private float baseFactor;
	private float passedEffectiveTime = 0;

	// Update is called once per frame
	void Update () {
		passedEffectiveTime += speed * Time.deltaTime;
		float sizeFactor = (Mathf.Sin (Time.time + passedEffectiveTime) * fluctuation) + (1 - fluctuation);
		this.transform.localScale = new Vector3(sizeFactor, sizeFactor, sizeFactor) * baseFactor;
	}

	public void setSpeedFactor(float factor) {
		speed = (maxSpeed - minSpeed) * factor + minSpeed;
	}

	public void setBaseFactor(float factor) {
		baseFactor = factor;
	}
}