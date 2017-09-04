using UnityEngine;

public class WheelMovement : MonoBehaviour
{
	private const float RADIUS = 1.0f;
	private const float ROTATION_REDUCTION = 0.2f;

	public GameObject LeftWheel;
	public GameObject RightWheel;
	public GameObject Player;

	private float LeftWheelSpeed;
	private float RightWheelSpeed;

	void Start ()
	{
		LeftWheelSpeed = 0.0f;
		RightWheelSpeed = 0.0f;
	}


	void Update ()
	{
		SetSpeeds ();
		if (LeftWheelSpeed == 0.0f && RightWheelSpeed != 0.0f) {
			
			Player.transform.Rotate (new Vector3 (0, -1, 0) * RightWheelSpeed * ROTATION_REDUCTION);

		} else if (LeftWheelSpeed != 0.0f && RightWheelSpeed == 0.0f) {

			Player.transform.Rotate (new Vector3 (0, 1, 0) * LeftWheelSpeed * ROTATION_REDUCTION);

		} else if (LeftWheelSpeed != 0.0f && RightWheelSpeed != 0.0f) {
			
			Player.transform.position += Player.transform.GetChild (0).forward * LeftWheelSpeed * RADIUS;

		}
	}

	private void SetSpeeds ()
	{
		LeftWheelSpeed = LeftWheel.GetComponent<WheelManipulation> ().Speed;
		RightWheelSpeed = RightWheel.GetComponent<WheelManipulation> ().Speed;
	}
}
