using UnityEngine;

public class WandInteraction : WandAction
{
	private const float MANIPULATION_VELOCITY = 0.1f;
	private const float SCALE_REDUCTOR = 1.0f;

	public GameObject manipulatedObject;
	private GameObject hitObject;

	private bool isManipulating = false;
	private bool enhancedMove = false;

	private Vector3 initialLeft;
	private Vector3 initialRight;
	private Vector3 initialScale;

	public void SetWandInteractionData (HandType Hand)
	{
		this.Hand = Hand;
	}

	protected override void ButtonOnePressed ()
	{
		if (manipulatedObject != null) {
			if (!isManipulating) {	
				Debug.Log ("Manipulando");
				isManipulating = true;
				manipulatedObject.transform.parent = ((Hand == HandType.RIGHT)
					? GameObject.Find ("RightHand").transform
					: GameObject.Find ("LeftHand").transform);
			} else {
				Debug.Log ("NO Manipulando");
				isManipulating = false;
				manipulatedObject.transform.parent = GameObject.Find ("Piramide").transform;
				manipulatedObject = null;
			}
		}
	}

	protected override void ButtonOneReleased ()
	{
		
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
		if (!isManipulating) {
			Ray ray = new Ray (this.transform.position, this.transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 10000)) {
				HandleHit (hit);
			}
		}

		if (isManipulating && manipulatedObject != null) {
			DeleteReferences ();
			// TODO: Arreglar el escalado
			/*
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
			*/

			// Mover con joystic
			Vector2 aditionalMovement = getAditionalMovement ().normalized * MANIPULATION_VELOCITY;
			Vector3 localPosition = manipulatedObject.transform.localPosition;
			manipulatedObject.transform.localPosition = new Vector3 (localPosition.x + aditionalMovement.x, localPosition.y, localPosition.z + aditionalMovement.y);
		}
	}

	private void DeleteReferences ()
	{
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Reference")) {
			Destroy (obj.gameObject);
		}
	}

	private Vector2 getAditionalMovement ()
	{
		return (Hand == HandType.RIGHT) 
		? OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch)
		: OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
	}

	private void HandleHit (RaycastHit hit)
	{
		if (hitObject == null) {
			hitObject = Instantiate (Resources.Load ("Interactable") as GameObject, hit.point, Quaternion.identity);
		} else {
			hitObject.transform.position = hit.point;
		}

		if (hit.transform.tag == "InteractableObject") {
			manipulatedObject = hit.collider.gameObject;
			hitObject.GetComponent<Renderer> ().material.color = Color.green;
		} else {
			manipulatedObject = null;
			hitObject.GetComponent<Renderer> ().material.color = Color.red;
		}
	}

	private bool IsScalable ()
	{
		WandInteraction leftInteraction = null;
		WandInteraction rightInteraction = null;
		try {
			leftInteraction = GameObject.Find ("LeftHand/stick/PatternCreator").GetComponent<WandInteraction> ();
			rightInteraction = GameObject.Find ("RightHand/stick/PatternCreator").GetComponent<WandInteraction> ();
		} catch {
			return false;
		}

		if (leftInteraction == null || rightInteraction == null) {
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