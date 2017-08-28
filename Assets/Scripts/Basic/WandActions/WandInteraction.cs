using UnityEngine;

public class WandInteraction : WandAction
{
	private const float MANIPULATION_VELOCITY = 1.0f;
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
			JoysticMovement ();
		}
	}

	private void JoysticMovement ()
	{
		Vector2 aditionalMovement = getAditionalMovement () * MANIPULATION_VELOCITY;
		Vector3 movementVector = (aditionalMovement.x * transform.right) + (aditionalMovement.y * transform.forward);

		// manipulatedObject.transform.Translate ((transform.right * aditionalMovement.x) + (transform.forward * aditionalMovement.y));
		manipulatedObject.transform.position += movementVector;
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

	private float distanceBetweenPoints (Vector3 A, Vector3 B)
	{
		return Mathf.Sqrt (Mathf.Pow (B.x - A.x, 2) + Mathf.Pow (B.y - A.y, 2) + Mathf.Pow (B.z - A.z, 2));
	}
}