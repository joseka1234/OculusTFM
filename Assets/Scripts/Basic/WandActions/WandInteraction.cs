using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandInteraction : WandAction
{
	private GameObject manipulatedObject;
	private GameObject hitObject;

	protected override void ButtonOnePressed ()
	{
		manipulatedObject.transform.parent = transform;
	}

	protected override void ButtonOneReleased ()
	{
		manipulatedObject.transform.parent = GameObject.Find ("ObjetosEscena").transform;
	}

	protected override void ButtonTwoPressed ()
	{
		throw new System.NotImplementedException ();
	}

	protected override void ButtonTwoReleased ()
	{
		throw new System.NotImplementedException ();
	}

	protected override void UpdateAction ()
	{
		Ray ray = new Ray (this.transform.position, this.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 10000)) {
			HandleHit (hit);
		}
	}

	private void HandleHit (RaycastHit hit)
	{
		if (hitObject == null) {
			hitObject = Instantiate (Resources.Load ("Interactable") as GameObject, hit.point, Quaternion.identity);
		} else {
			hitObject.transform.position = hit.point;
		}

		if (hit.transform.tag == "Interactable") {
			manipulatedObject = hit.collider.gameObject;
			hitObject.GetComponent<Renderer> ().material.color = Color.green;
		} else {
			manipulatedObject = null;
			hitObject.GetComponent<Renderer> ().material.color = Color.red;
		}
	}
}
