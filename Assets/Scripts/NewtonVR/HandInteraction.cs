using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteraction : MonoBehaviour
{

	public void OnCollisionEnter (Collision col)
	{
		if (col.transform.tag == "Interactable") {
			col.gameObject.GetComponent<Renderer> ().material.color = new Color (1, 1, 1, 0.5f);
		}
	}

	public void OnCollisionExit (Collision col)
	{
		col.gameObject.GetComponent<Renderer> ().material.color = new Color (1, 1, 1, 1);
	}
}
