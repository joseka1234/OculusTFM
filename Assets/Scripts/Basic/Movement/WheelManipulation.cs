using UnityEngine;

public class WheelManipulation : MonoBehaviour
{

	public float Speed;
	private bool manipulating;
	private string handName;
	private const float ROTATION_SPEED = 3.0f;
	// Use this for initialization
	void Start ()
	{
		manipulating = false;
		handName = "";
		Speed = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (manipulating) {
			if (handName == "RightHand") {
				if (OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > 0.0f) {
					this.transform.Rotate (new Vector3 (0, 0, -ROTATION_SPEED));
					Speed = ROTATION_SPEED;
				} else {
					Speed = 0;
				}
			} else if (handName == "LeftHand") {
				if (OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) > 0.0f) {
					this.transform.Rotate (new Vector3 (0, 0, ROTATION_SPEED));
					Speed = ROTATION_SPEED;
				} else {
					Speed = 0;
				}
			}
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.tag == "Hand") {
			manipulating = true;
			handName = col.name;

			if (handName == "LeftHand") {
				OVRInput.SetControllerVibration (0.2f, 0.2f, OVRInput.Controller.LTouch);
			} else {
				OVRInput.SetControllerVibration (0.2f, 0.2f, OVRInput.Controller.RTouch);
			}
		}
	}

	void OnTriggerStay (Collider col)
	{
		if (col.tag == "Hand") {
			manipulating = true;
			handName = col.name;

			if (handName == "LeftHand") {
				OVRInput.SetControllerVibration (0.2f, 0.2f, OVRInput.Controller.LTouch);
			} else {
				OVRInput.SetControllerVibration (0.2f, 0.2f, OVRInput.Controller.RTouch);
			}
		}
	}

	void OnTriggerExit (Collider col)
	{
		if (col.tag == "Hand") {
			manipulating = false;
			handName = col.name;

			if (handName == "LeftHand") {
				OVRInput.SetControllerVibration (0.0f, 0.0f, OVRInput.Controller.LTouch);
			} else {
				OVRInput.SetControllerVibration (0.0f, 0.0f, OVRInput.Controller.RTouch);
			}
			Speed = 0.0f;
		}
	}
}
