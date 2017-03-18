using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{

	public Vector3 orbitPoint;
	public float radius = 2.5f;
	public float speed = 1f; // 1 is instant match with mouse
	public float inertia = 0;

	// Might need to be computed in the future, but fixed is fine for now.
	public int cameraHeight = 20;

	// Use this for initialization
	void Start ()
	{
		
	}

	float computeSignedAngle (Vector3 targetDirectionVector)
	{
		float deltaAngle = Vector3.Angle (this.transform.position, targetDirectionVector);
		Vector3 cross = Vector3.Cross (this.transform.position, targetDirectionVector);
		if (cross.y < 0) 
		{
			deltaAngle = -deltaAngle;
		}
		return deltaAngle;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 targetDirectionVector = getInworldMousePosition ();

		var deltaAngle = computeSignedAngle (targetDirectionVector);

		Quaternion quaternion = Quaternion.AngleAxis (deltaAngle, Vector3.up);

		Debug.Log ("Angle:" + deltaAngle);

		this.transform.position = quaternion * this.transform.position;
	}

	private Vector3 getInworldMousePosition ()
	{
		Vector2 mousePosition = Input.mousePosition;
		Vector3 mouse3d = new Vector3(mousePosition.x, mousePosition.y, cameraHeight);
		return Camera.main.ScreenToWorldPoint(mouse3d);
	}
}
