﻿using UnityEngine;
using UnityEngine.AI;

public class NavMeshMove : Movement
{
	NavMeshAgent Agent;

	public void NavMeshSetData (string Name, GameObject player)
	{
		SetData (Name, player);
		Agent = player.AddComponent<NavMeshAgent> ();
	}

	override public void Move ()
	{
		Agent.destination = new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ());
	}

	override public void StopMove ()
	{
		Destroy (Agent);
	}
}
