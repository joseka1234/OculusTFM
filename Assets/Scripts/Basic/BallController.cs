using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
	private const float RESET_TIME = 10f;
	private Vector3 initialPosition;

	void Start ()
	{
		initialPosition = transform.position;
	}

	public void ResetBall ()
	{
		StartCoroutine (ResetBallCoroutine ());
	}

	private  IEnumerator ResetBallCoroutine ()
	{
		yield return new WaitForSeconds (RESET_TIME);
		transform.position = initialPosition;
	}
}
