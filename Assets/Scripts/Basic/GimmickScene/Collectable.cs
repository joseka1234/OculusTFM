using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
	void OnCollisionEnter (Collision col)
	{
		if (col.transform.tag == "Player") {
			GameObject.Find ("GameManager").GetComponent<GameManager> ().IncrementScore ();
			Destroy (gameObject);
		}
	}
}
