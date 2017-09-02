using System.Collections;
using UnityEngine;

public class WheelMovement : MonoBehaviour
{
	private const float RADIUS = 0.5f;
	private float AngularVelocity;

	void Start ()
	{
		AngularVelocity = 0.0f;
	}

	void Update ()
	{
		
	}

	private float getLinealVelocity (float radius)
	{
		StartCoroutine (getAngularVelocity ());
		return AngularVelocity * radius;
	}

	private IEnumerator getAngularVelocity ()
	{
		AngularVelocity = 0.0f;
		float auxAngle = this.transform.rotation.z;
		yield return new WaitForSeconds (1.0f);
		AngularVelocity = this.transform.rotation.z - auxAngle;
	}
}
