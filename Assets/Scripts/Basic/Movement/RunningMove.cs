using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RunningMove : Movement
{
	private const int NUMBER_OF_SAMPLES = 1000;
	private const float MAX_DEVIATION = 0.2f;

	private List<float> walkingLinearSample;
	private List<float> walkingAngularSample;

	private OVRDisplay Display;
	private PannelController pannelController;
	private Coroutine trainingCoroutine;
	private float Velocity;


	private Coroutine moveCoroutine;

	private bool isTraining;

	public void RunningSetData (string Name, GameObject player, PannelController pannelController, float Velocity)
	{
		SetData (Name, player);
		isTraining = true;
		this.pannelController = pannelController;
		this.Velocity = Velocity;

		Display = new OVRDisplay ();
		Display.RecenterPose ();

		walkingLinearSample = new List<float> ();
		walkingAngularSample = new List<float> ();

		trainingCoroutine = StartCoroutine (TrainingWalking ());
	}

	public override void Move ()
	{
		moveCoroutine = StartCoroutine (MoveCoroutine ());
	}

	private IEnumerator MoveCoroutine ()
	{
		List<float> LinearSamples = new List<float> ();
		List<float> AngularSamples = new List<float> ();
		while (true) {
			if (!isTraining) {
				for (int i = 0; i < NUMBER_OF_SAMPLES / 100; i++) {
					LinearSamples.Add (GetMergeAcceleration (Display.acceleration));
					AngularSamples.Add (GetMergeAcceleration (Display.angularAcceleration));
					yield return new WaitForSeconds (0.01f);
				}
				if (isWalking (LinearSamples, AngularSamples)) {
					player.transform.Translate (player.transform.GetChild (0).forward * Velocity);
				}
				LinearSamples.Clear ();
				AngularSamples.Clear ();
			}
			yield return new WaitForSeconds (0.1f);
		}
	}

	public override void StopMove ()
	{
		if (trainingCoroutine != null) {
			StopCoroutine (trainingCoroutine);
		}
		if (moveCoroutine != null) {
			StopCoroutine (moveCoroutine);
		}
	}

	private bool isWalking (List<float> LinearSamples, List<float> AngularSamples)
	{

		// Estudio con la desviación típica
		//bool linearMove = Mathf.Abs (DesviacionTipica (LinearSamples) - DesviacionTipica (walkingLinearSample)) < MAX_DEVIATION;
		//bool angularMove = Mathf.Abs (DesviacionTipica (AngularSamples) - DesviacionTipica (walkingAngularSample)) < MAX_DEVIATION;

		// Estudio de máximos y mínimos
		float minSample, maxSample, minWalkingSample, maxWalkingSample;

		// Aceleración Lineal
		minSample = LinearSamples.Min (x => x);
		maxSample = LinearSamples.Max (x => x);
		minWalkingSample = walkingLinearSample.Min (x => x);
		maxWalkingSample = walkingLinearSample.Max (x => x);
		bool linearMove = (minSample >= minWalkingSample) && (maxSample <= maxWalkingSample);

		// Aceleración angular
		minSample = AngularSamples.Min (x => x);
		maxSample = AngularSamples.Max (x => x);
		minWalkingSample = walkingAngularSample.Min (x => x);
		maxWalkingSample = walkingAngularSample.Max (x => x);
		bool angularMove = (minSample >= minWalkingSample) && (maxSample <= maxWalkingSample);

		return linearMove && angularMove;
	}

	#region Medidas estadísticas para detección del patrón

	private float Media (List<float> Sample)
	{
		float aux = 0.0f;
		foreach (float data in Sample) {
			aux += data;
		}
		return aux / (float)Sample.Count;
	}

	private float Varianza (List<float> Sample)
	{
		float aux = 0.0f;
		float media = Media (Sample);
		foreach (float data in Sample) {
			aux += Mathf.Pow (data - media, 2);
		}
		return aux / (float)Sample.Count;
	}

	private float DesviacionTipica (List<float> Sample)
	{
		return Mathf.Sqrt (Varianza (Sample));
	}

	#endregion

	#region Training Samples

	private IEnumerator TrainingWalking ()
	{
		isTraining = true;
		yield return new WaitForSeconds (2.0f);
		pannelController.SetPannelText ("NOW\nWALK!");
		Debug.Log ("Training Walking");
		yield return new WaitForSeconds (2.0f);

		for (int i = 0; i < NUMBER_OF_SAMPLES; i++) {
			walkingLinearSample.Add (GetMergeAcceleration (Display.acceleration));
			walkingAngularSample.Add (GetMergeAcceleration (Display.angularAcceleration));
			yield return new WaitForSeconds (0.01f);
		}

		Debug.Log ("End Training Walking");
		pannelController.SetPannelText ("STOP\nWALKING");
		yield return new WaitForSeconds (2.0f);
		pannelController.SetPannelText ("YOU CAN\nWALK");
		isTraining = false;
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
