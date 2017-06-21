using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{

	public float DisplacementRatio = 0.4f;
	public float AngularDisplacementRatio = 0.4f;
	public Transform movingTarget;

	private Vector3 defaultPosition;

	void Start ()
	{
		defaultPosition = this.transform.position;
	}
	
	void Update ()
	{
		Vector3 targetDisplacement = ComputeNewPosition (movingTarget.position);
		this.transform.position = defaultPosition + targetDisplacement;
	}

	private Vector3 ComputeNewPosition (Vector3 lookingPosition)
	{
		return lookingPosition * DisplacementRatio;
	}
}