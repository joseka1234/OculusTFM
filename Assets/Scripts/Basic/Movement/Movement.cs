using UnityEngine;
using System;

public abstract class Movement : MonoBehaviour
{
	public string Name;

	protected Vector3 Destination;
	protected Vector3 Origin;
	protected GameObject player;
	protected float YCorrector;

	private bool isMoving;

	public abstract void Move ();

	public abstract void StopMove ();

	protected void SetData (string Name, GameObject player)
	{
		this.Name = Name;
		this.player = player;
		Origin = this.player.transform.position;
		// Destination = new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ());
		isMoving = false;
	}

	#region Get coordinates functions

	protected float GetYCoordinate ()
	{
		GameObject destino = GameObject.Find ("DestinoGO");
		float y;
		try {
			y = destino.transform.position.y + (player.GetComponent<CharacterController> ().height / 4);
		} catch (Exception e) {
			// Debug.Log ("Usando modelo de NewtonVR");
			y = player.transform.position.y;
		}
		return y;
	}

	protected float GetXCoordinate ()
	{
		GameObject destino = GameObject.Find ("DestinoGO");
		return destino.transform.position.x;
	}

	protected float GetZCoordinate ()
	{
		GameObject destino = GameObject.Find ("DestinoGO");
		return destino.transform.position.z;
	}

	protected Vector3 GetNewForward ()
	{
		GameObject hand = GameObject.Find ("RightHand");
		return new Vector3 (hand.transform.forward.x, player.transform.forward.y, hand.transform.forward.z);
	}

	#endregion

	public bool playerIsMoving ()
	{
		return isMoving;
	}

	public void setMoving (bool isMoving)
	{
		this.isMoving = isMoving;
	}

	protected void UpdateDestination ()
	{
		Destination = new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ());
	}
}
