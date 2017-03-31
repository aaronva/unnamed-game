using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressorController : MonoBehaviour
{
	public float stressLevel = 10;

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
		// TODO complete this
	}

	public void applyForce (Vector3 force)
	{
		Rigidbody rb = GetComponent<Rigidbody> ();
		rb.AddForce (force);
	}
}
