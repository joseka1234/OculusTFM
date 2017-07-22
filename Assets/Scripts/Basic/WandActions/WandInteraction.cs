using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandInteraction : WandAction
{
	private const float MANIPULATION_VELOCITY = 10.0f;
	private const float ENHANCED_MODIFICATOR = 10.0f;
	private const float SCALE_REDUCTOR = 1.0f;

	public GameObject manipulatedObject;
	private GameObject hitObject;

	private bool isManipulating = false;
	private bool enhancedMove = false;

	private Vector3 initialLeft;
	private Vector3 initialRight;
	private Vector3 initialScale;

	public void SetWandMovementData (HandType Hand)
	{
		this.Hand = Hand;
	}

	protected override void ButtonOnePressed ()
	{
		if (!isManipulating) {
			isManipulating = true;
			Debug.Log ("Catched " + manipulatedObject.name);
		} else {
			isManipulating = false;
			Debug.Log ("Released " + manipulatedObject.name);
		}
	}

	protected override void ButtonOneReleased ()
	{
		throw new System.NotImplementedException ();
	}

	protected override void ButtonTwoPressed ()
	{
		enhancedMove = true;
	}

	protected override void ButtonTwoReleased ()
	{
		enhancedMove = false;
	}

	private bool firstScale = true;

	protected override void UpdateAction ()
	{
		Ray ray = new Ray (this.transform.position, this.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 10000)) {
			HandleHit (hit);
		}

		if (isManipulating) {
			if (enhancedMove) {
				manipulatedObject.transform.Translate (getHandAcceleration ().normalized * MANIPULATION_VELOCITY * ENHANCED_MODIFICATOR);
				manipulatedObject.transform.Rotate (getHandAngularAcceleration ().normalized * MANIPULATION_VELOCITY * ENHANCED_MODIFICATOR / 2.0f);
			} else {
				manipulatedObject.transform.Translate (getHandAcceleration ().normalized * MANIPULATION_VELOCITY);
				manipulatedObject.transform.Rotate (getHandAngularAcceleration ().normalized * MANIPULATION_VELOCITY / 2.0f);
			}
			if (IsScalable ()) {
				if (firstScale) {
					initialLeft = GameObject.Find ("LeftHand").transform.position;
					initialRight = GameObject.Find ("RightHand").transform.position;
					initialScale = manipulatedObject.transform.localScale;
					firstScale = false;
				}
				ScaleObject ();
			} else {
				initialLeft = Vector3.zero;
				initialRight = Vector3.zero;
				initialScale = Vector3.one;
				firstScale = true;
			}
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

	private bool IsScalable ()
	{
		WandInteraction leftInteraction;
		WandInteraction rightInteraction;
		try {
			leftInteraction = GameObject.Find ("LeftHand/stick/PatternCreator").GetComponent<WandInteraction> ();
			rightInteraction = GameObject.Find ("RightHand/stick/PatternCreator").GetComponent<WandInteraction> ();
		} catch {
			return false;
		}

		return leftInteraction.manipulatedObject == rightInteraction.manipulatedObject;
	}

	private void ScaleObject ()
	{
		manipulatedObject.transform.localScale = initialScale * getDistanceBetweenHands ();
	}

	private float getDistanceBetweenHands ()
	{
		Vector3 leftHand = GameObject.Find ("LeftHand").transform.position;
		Vector3 rightHad = GameObject.Find ("RightHand").transform.position;
		return (distanceBetweenPoints (leftHand, rightHad) - distanceBetweenPoints (initialLeft, initialRight)) / SCALE_REDUCTOR;
	}

	private float distanceBetweenPoints (Vector3 A, Vector3 B)
	{
		return Mathf.Sqrt (Mathf.Pow (B.x - A.x, 2) + Mathf.Pow (B.y - A.y, 2) + Mathf.Pow (B.z - A.z, 2));
	}
}