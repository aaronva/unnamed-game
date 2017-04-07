using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{

	public float DisplacementRatio = 0.02f;
	public float MinDistance = 0.2f;
	public float MaxDistance = 20f;

	private Vector3 defaultPosition;
//	private Quaternion defaultRotation;

	void Start ()
	{
		defaultPosition = this.transform.position;
//		defaultRotation = this.transform.rotation;
	}
	
	void Update ()
	{
		Vector3 mousePosition = GetInworldMousePosition ();
		Vector3 targetDisplacement = ComputeNewPosition (mousePosition);
		this.transform.position = defaultPosition + targetDisplacement;
//		this.transform.ro
//		Quaternion angularDelta = ComputeNewRotation(mousePosition);
//		this.transform.rotation = angularDelta;
	}

	private Vector3 ComputeNewPosition (Vector3 mousePosition)
	{
		return mousePosition * DisplacementRatio;
	}

	// TODO research and better understand Quaternion (and/or angles)
	private Quaternion ComputeNewRotation (Vector3 mousePosition)
	{
//		Vector3 delta = this.transform.position - mousePosition;
		return Quaternion.LookRotation (new Vector3(0,0,5) - this.transform.position, Vector3.forward);
//		return Quaternion.FromToRotation (this.transform.position, delta);
	}

	private Vector3 GetInworldMousePosition ()
	{
		Vector2 mousePosition = Input.mousePosition;
		Vector3 mouse3d = new Vector3 (mousePosition.x, mousePosition.y, this.transform.position.y);
		return Camera.main.ScreenToWorldPoint (mouse3d);
	}
}