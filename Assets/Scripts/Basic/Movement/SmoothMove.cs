using UnityEngine;
using System.Collections;

public class SmoothMove : Movement
{

	private Coroutine moveCoroutine;

	public void SmoothSetData (string Name, GameObject player)
	{
		SetData (Name, player);
	}

	override public void Move ()
	{
		SetNewOrigin ();
		Destination = new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ());
		moveCoroutine = StartCoroutine (MoveCoroutine ());
	}

	IEnumerator MoveCoroutine ()
	{
		setMoving (true);
		float aux = 0.0f;
	
		while (player.transform.position != Destination) {
			player.transform.position = Vector3.Lerp (Origin, Destination, aux);
			aux += 0.0005f;

			if (player.transform.position == Destination) {
				setMoving (false);
			}

			yield return new WaitForSeconds (0.01f);
		}
		setMoving (false);
	}

	override public void StopMove ()
	{
		if (moveCoroutine != null) {
			StopCoroutine (moveCoroutine);	
		}
		player.transform.position = new Vector3 (player.transform.position.x, GetYCoordinate (), player.transform.position.z);
	}
}

