using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMove : Movement
{
	NavMeshAgent Agent;

	public NavMeshMove (string Name, GameObject player) : base (Name, player)
	{
		Agent = player.GetComponent<NavMeshAgent> ();
	}

	override public void Move ()
	{
		
	}

	override public void StopMove ()
	{
		
	}
}
