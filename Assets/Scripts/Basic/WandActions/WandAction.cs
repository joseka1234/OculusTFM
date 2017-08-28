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

	protected Vector3 getHandVelocity ()
	{
		return (Hand == HandType.RIGHT)
			? OVRInput.GetLocalControllerVelocity (OVRInput.Controller.RTouch)
			: OVRInput.GetLocalControllerVelocity (OVRInput.Controller.LTouch);
		
	}

	protected Vector3 getHandAngularVelocity ()
	{
		return (Hand == HandType.RIGHT)
			? OVRInput.GetLocalControllerAngularVelocity (OVRInput.Controller.RTouch)
			: OVRInput.GetLocalControllerAngularVelocity (OVRInput.Controller.LTouch);
	}
}
