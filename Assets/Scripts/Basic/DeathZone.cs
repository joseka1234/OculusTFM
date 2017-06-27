using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{

	void OnCollisionEnter (Collision col)
	{
		Destroy (col.collider.gameObject);
	}
}
