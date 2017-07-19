using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WandAction : MonoBehaviour
{
	protected string Hand;

	protected abstract void ButtonTwoAction ();

	protected abstract void ButtonOneAction ();

	protected abstract void UpdateAction ();

	void LateUpdate ()
	{
		UpdateAction ();

		if ((Hand == "RTOUCH")
			? OVRInput.GetDown (OVRInput.Button.Two, OVRInput.Controller.RTouch)
			: OVRInput.GetDown (OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
			ButtonTwoAction ();
		}

		if ((Hand == "RTOUCH")
			? OVRInput.GetDown (OVRInput.Button.One, OVRInput.Controller.RTouch)
			: OVRInput.GetDown (OVRInput.Button.One, OVRInput.Controller.LTouch)) {
			ButtonOneAction ();
		}
	}
}
