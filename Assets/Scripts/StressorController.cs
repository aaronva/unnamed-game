using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressorController : MonoBehaviour {
	public int stressLevel = 10;

	public void applyForce(Vector3 force) {
		Rigidbody rb = GetComponent<Rigidbody> ();
		rb.AddForce (force);
	}
}
