using UnityEngine;

public class TeleportMove : Movement
{

	public TeleportMove (string Name, GameObject player) : base (Name, player)
	{
	}

	override public void Move ()
	{
		player.transform.position = Destination;
		player.transform.forward = GetNewForward ();
	}

	override public void StopMove ()
	{
		// Con el teleport no es necesario implementar nada aquí
	}
}
