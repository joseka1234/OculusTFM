using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WandAction : MonoBehaviour
{
	protected string Hand;

	protected abstract void ButtonTwoPressed ();

	protected abstract void ButtonOnePressed ();

	protected abstract void ButtonTwoReleased ();

	protected abstract void ButtonOneReleased ();

	protected abstract void UpdateAction ();

	void LateUpdate ()
	{
		UpdateAction ();

		if ((Hand == "RTOUCH")
			? OVRInput.GetDown (OVRInput.Button.One, OVRInput.Controller.RTouch)
			: OVRInput.GetDown (OVRInput.Button.One, OVRInput.Controller.LTouch)) {
			ButtonOnePressed ();
		}

		if ((Hand == "RTOUCH")
			? OVRInput.GetUp (OVRInput.Button.One, OVRInput.Controller.RTouch)
			: OVRInput.GetUp (OVRInput.Button.One, OVRInput.Controller.LTouch)) {
			ButtonOneReleased ();
		}

		if ((Hand == "RTOUCH")
			? OVRInput.GetDown (OVRInput.Button.Two, OVRInput.Controller.RTouch)
			: OVRInput.GetDown (OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
			ButtonTwoPressed ();
		}

		if ((Hand == "RTOUCH")
			? OVRInput.GetUp (OVRInput.Button.Two, OVRInput.Controller.RTouch)
			: OVRInput.GetUp (OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
			ButtonTwoReleased ();
		}
	}
}
