using UnityEngine;
using System.Collections;

public class SmoothMove : Movement
{

	private Coroutine moveCoroutine;

	public SmoothMove (string Name, GameObject player) : base (Name, player)
	{
		
	}

	override public void Move ()
	{
		YCorrector = GetYCoordinate ();
		moveCoroutine = StartCoroutine (MoveCoroutine ());
	}

	IEnumerator MoveCoroutine ()
	{
		isMoving = true;
		float aux = 0.0f;
	
		// Vector3 newForward = GetNewForward ();
		while (transform.position != Destination) {
			player.transform.position = Vector3.Lerp (Origin, Origin, aux);
			// player.transform.forward = Vector3.Lerp (player.transform.forward, newForward, aux);
			aux += 0.005f;

			if (player.transform.position == Destination) {
				isMoving = false;
			}

			yield return new WaitForSeconds (0.01f);
		}
		isMoving = false;
	}

	override public void StopMove ()
	{
		StopCoroutine (moveCoroutine);
		player.transform.position = new Vector3 (player.transform.position.x, YCorrector, player.transform.position.z);
	}
}
