using UnityEngine;
using System;

public abstract class Movement : MonoBehaviour
{
	public string Name;

	protected Vector3 Destination;
	protected Vector3 Origin;
	protected GameObject player;
	protected float YPosition;

	private bool isMoving;

	public abstract void Move ();

	public abstract void StopMove ();

	protected void SetData (string Name, GameObject player)
	{
		this.Name = Name;
		this.player = player;
		Origin = this.player.transform.position;
		YPosition = this.player.transform.position.y;
		isMoving = false;
	}

	protected void SetNewOrigin ()
	{
		Origin = player.transform.position;
	}

	#region Get coordinates functions

	protected float GetYCoordinate ()
	{
		/*
		GameObject destino = GameObject.Find ("DestinoGO");
		float y;
		try {
			y = destino.transform.position.y + (player.GetComponent<CharacterController> ().height / 4);
		} catch (Exception e) {
			y = player.transform.position.y;
		}
		return y;
		*/

		/*
		GameObject destino = GameObject.Find ("DestinoGO");
		return destino.transform.position.y + 20.0f;
		*/

		return YPosition;
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
