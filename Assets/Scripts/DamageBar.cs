using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBar : MonoBehaviour
{

	public float percent { get; set; }

	public float startingXScale { get; private set; }

	void Start ()
	{
		percent = 1f;
		startingXScale = this.transform.localScale.x;
	}

	void Update ()
	{
		float newXScale = startingXScale * percent;
		this.transform.localScale = new Vector3 (newXScale, this.transform.localScale.y, this.transform.localScale.z);
	}
}