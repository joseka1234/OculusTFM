using UnityEngine;

public class TeleportMove : Movement
{

	public void TeleportSetData (string Name, GameObject player)
	{
		SetData (Name, player);
	}

	override public void Move ()
	{
		player.transform.position = new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ());
	}

	override public void StopMove ()
	{
	}
}
