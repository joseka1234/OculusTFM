using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityToolbag;
using System.Linq;

public class RunningMove : Movement
{
	private const int NUMBER_OF_SAMPLES = 10000;

	private List<float> walkingLinearSample;
	private List<float> walkingAngularSample;
	private List<float> runningLinearSample;
	private List<float> runningAngularSample;

	private OVRDisplay Display;
	private PannelController pannelController;
	private Coroutine trainingCoroutine;

	private bool BIsWalking;
	private bool BIsRunning;

	public RunningMove (string Name, GameObject player, PannelController pannelController) : base (Name, player)
	{	
		this.pannelController = pannelController;
		BIsRunning = false;
		BIsRunning = false;

		Display = new OVRDisplay ();
		Display.RecenterPose ();

		walkingLinearSample = new List<float> ();
		walkingAngularSample = new List<float> ();
		runningLinearSample = new List<float> ();
		runningAngularSample = new List<float> ();

		trainingCoroutine = StartCoroutine (TrainingWalking ());
	}

	public override void Move ()
	{

	}

	public override void StopMove ()
	{
		if (trainingCoroutine != null) {
			StopCoroutine (trainingCoroutine);
		}
	}

	private void isWalking ()
	{
		
	}

	private void isRunning ()
	{
		
	}

	#region Training Samples

	private IEnumerator TrainingWalking ()
	{
		pannelController.SetMoveName ("NOW\nWALK!");
		Debug.Log ("Training Walking");
		yield return new WaitForSeconds (5.0f);

		for (int i = 0; i < NUMBER_OF_SAMPLES; i++) {
			walkingLinearSample.Add (GetMergeAcceleration (Display.acceleration));
			walkingAngularSample.Add (GetMergeAcceleration (Display.angularAcceleration));
			yield return new WaitForSeconds (0.01f);
		}

		Debug.Log ("End Training Walking");
		pannelController.SetMoveName ("STOP\nWALKING");
		yield return new WaitForSeconds (5.0f);
		trainingCoroutine = StartCoroutine (TrainingRunning ());
	}

	private IEnumerator TrainingRunning ()
	{
		pannelController.SetMoveName ("NOW\nRUN!");
		Debug.Log ("Training Running");
		yield return new WaitForSeconds (5.0f);

		for (int i = 0; i < NUMBER_OF_SAMPLES; i++) {
			runningLinearSample.Add (GetMergeAcceleration (Display.acceleration));
			runningAngularSample.Add (GetMergeAcceleration (Display.angularAcceleration));
			yield return new WaitForSeconds (0.01f);
		}

		Debug.Log ("End Training Running");
		pannelController.SetMoveName ("STOP\nRUNNING");
	}

	private float GetMergeAcceleration (Vector3 rawAcceleration)
	{
		float powX = Mathf.Pow (rawAcceleration.x, 2);
		float powY = Mathf.Pow (rawAcceleration.y, 2);
		float powZ = Mathf.Pow (rawAcceleration.z, 2);

		return Mathf.Sqrt (powX + powY + powZ);
	}

	#endregion

}
