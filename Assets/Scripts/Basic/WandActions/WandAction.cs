using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WandAction : MonoBehaviour
{
	// protected string Hand;

	protected abstract void ButtonTwoPressed ();

	protected abstract void ButtonOnePressed ();

	protected abstract void ButtonTwoReleased ();

	protected abstract void ButtonOneReleased ();

	protected abstract void UpdateAction ();

	protected HandType Hand;

	public enum HandType
	{
		RIGHT,
		LEFT
	}

	void LateUpdate ()
	{
		UpdateAction ();

		if ((Hand == HandType.RIGHT)
			? OVRInput.GetDown (OVRInput.Button.One, OVRInput.Controller.RTouch)
			: OVRInput.GetDown (OVRInput.Button.One, OVRInput.Controller.LTouch)) {
			ButtonOnePressed ();
		}

		if ((Hand == HandType.RIGHT)
			? OVRInput.GetUp (OVRInput.Button.One, OVRInput.Controller.RTouch)
			: OVRInput.GetUp (OVRInput.Button.One, OVRInput.Controller.LTouch)) {
			ButtonOneReleased ();
		}

		if ((Hand == HandType.RIGHT)
			? OVRInput.GetDown (OVRInput.Button.Two, OVRInput.Controller.RTouch)
			: OVRInput.GetDown (OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
			ButtonTwoPressed ();
		}

		if ((Hand == HandType.RIGHT)
			? OVRInput.GetUp (OVRInput.Button.Two, OVRInput.Controller.RTouch)
			: OVRInput.GetUp (OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
			ButtonTwoReleased ();
		}
	}

	protected Vector3 getHandAcceleration ()
	{
		return (Hand == HandType.RIGHT)
			? OVRInput.GetLocalControllerAcceleration (OVRInput.Controller.RTouch)
			: OVRInput.GetLocalControllerAcceleration (OVRInput.Controller.LTouch);
	}

	protected Vector3 getHandAngularAcceleration ()
	{
		return (Hand == HandType.RIGHT)
			? OVRInput.GetLocalControllerAngularAcceleration (OVRInput.Controller.RTouch)
			: OVRInput.GetLocalControllerAngularAcceleration (OVRInput.Controller.LTouch);
	}
}
