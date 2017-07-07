using UnityEngine;
using System.Collections;

public class JumpMove : Movement
{
	private Coroutine moveCoroutine;
	public float AlturaSalto;

	public void JumpSetData (float AlturaSalto, string Name, GameObject player)
	{
		this.AlturaSalto = AlturaSalto;
		SetData (Name, player);
	}

	override public void Move ()
	{
		Destination = new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ());
		YPosition = GetYCoordinate ();
		moveCoroutine = StartCoroutine (MoveCoroutine ());
	}

	IEnumerator MoveCoroutine ()
	{
		setMoving (true);
		float aux = 0.0f;
		Vector3 lerpVector;
		while (transform.position != Destination) {
			lerpVector = Vector3.Lerp (Origin, Origin, aux);
			player.transform.position = new Vector3 (lerpVector.x, Mathf.Sin (Mathf.LerpAngle (0, Mathf.PI, aux)) * AlturaSalto + Destination.y, lerpVector.z);
			aux += 0.005f;

			if (player.transform.position == Destination) {
				setMoving (false);
			}

			yield return new WaitForSeconds (0.01f);
		}
	}

	override public void StopMove ()
	{
		if (moveCoroutine != null) {
			StopCoroutine (moveCoroutine);
		}
		player.transform.position = new Vector3 (player.transform.position.x, GetYCoordinate (), player.transform.position.z);
	}
}
