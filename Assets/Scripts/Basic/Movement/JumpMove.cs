using UnityEngine;
using System.Collections;

public class JumpMove : Movement
{
	private Coroutine moveCoroutine;
	public float AlturaSalto;

	public JumpMove (float AlturaSalto, string Name, GameObject player) : base (Name, player)
	{
		this.AlturaSalto = AlturaSalto;
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
		Vector3 lerpVector;
		Vector3 newForward = GetNewForward ();
		while (transform.position != Destination) {
			lerpVector = Vector3.Lerp (Origin, Origin, aux);
			player.transform.position = new Vector3 (lerpVector.x, Mathf.Sin (Mathf.LerpAngle (0, Mathf.PI, aux)) * AlturaSalto + Destination.y, lerpVector.z);
			player.transform.forward = Vector3.Lerp (player.transform.forward, newForward, aux);
			aux += 0.005f;

			if (player.transform.position == Destination) {
				isMoving = false;
			}

			yield return new WaitForSeconds (0.01f);
		}
	}

	override public void StopMove ()
	{
		StopCoroutine (moveCoroutine);
		player.transform.position = new Vector3 (player.transform.position.x, YCorrector, player.transform.position.z);
	}
}
