using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{

	public float displacementRatio = 0.02f;
	public float minDistance = 0.2f;
	public float maxDistance = 20f;

	private Vector3 defaultPosition;

	// Use this for initialization
	void Start ()
	{
		defaultPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 mousePosition = GetInworldMousePosition ();
		Vector3 targetDisplacement = ComputeTransformation (mousePosition);
		this.transform.position = defaultPosition + targetDisplacement;
	}

	private Vector3 ComputeTransformation (Vector3 position)
	{
		return position *= displacementRatio;
	}

	private Vector3 GetInworldMousePosition ()
	{
		Vector2 mousePosition = Input.mousePosition;
		Vector3 mouse3d = new Vector3 (mousePosition.x, mousePosition.y, this.transform.position.y);
		return Camera.main.ScreenToWorldPoint (mouse3d);
	}
}
