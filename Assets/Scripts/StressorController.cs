using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressorController : MonoBehaviour
{
	public float stressLevel = 10f;

	public GameObject creator = null;

	public void Start ()
	{
		updateSize ();
	}

	public void setStressLevel (float value)
	{
		stressLevel = value;
		updateSize ();
	}

	private void updateSize ()
	{	
		this.transform.localScale = Vector3.one * ComputeSize (stressLevel);
	}

	public void applyForce (Vector3 force)
	{
		Rigidbody rb = GetComponent<Rigidbody> ();
		rb.AddForce (force);
	}

	public static float ComputeSize (float stressLevel)
	{
		return Mathf.Log (1 + stressLevel / 50) + 0.5f;
	}
}
