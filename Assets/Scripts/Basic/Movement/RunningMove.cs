using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class RunningMove : Movement
{
	private const int NUMBER_OF_SAMPLES = 1000;
	private const float MAX_DEVIATION = 0.2f;

	private List<float> walkingSample;
	private PannelController pannelController;
	private Coroutine trainingCoroutine;
	private float Velocity;
	private Transform headTransform;


	private Coroutine moveCoroutine;

	private bool isTraining;

	public void RunningSetData (string Name, GameObject player, PannelController pannelController, float Velocity)
	{
		SetData (Name, player);
		isTraining = true;
		this.pannelController = pannelController;
		this.Velocity = Velocity;

		headTransform = player.transform.GetChild (0);
		walkingSample = new List<float> ();

		trainingCoroutine = StartCoroutine (TrainingWalking ());
	}

	public override void Move ()
	{
		moveCoroutine = StartCoroutine (MoveCoroutine ());
	}

	private IEnumerator MoveCoroutine ()
	{
		// Se obtiene el tiempo que se tarda en pasar de un estado de paso bajo a un estado de paso alto.
		float timeBetweenSteps = getTimeBetweenSteps ();

		// Captamos de forma continua el patrón de pasos
		while (true) {
			if (!isTraining) {
				if (ZonaBaja ()) {
					yield return new WaitForSeconds (timeBetweenSteps - 0.01f);
					if (ZonaAlta ()) {
						Step ();
					} else {
						// En caso de que no se detecte un paso saltamos una iteración del bucle
						continue;
					}
				} else {
					yield return new WaitForSeconds (timeBetweenSteps - 0.01f);
					if (ZonaBaja ()) {
						Step ();
					} else {
						// En caso de que no se detecte un paso saltamos una iteración del bucle
						continue;
					}
				}
			}
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

	private void Step ()
	{
		player.transform.Translate (player.transform.GetChild (0).forward * Velocity);
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

	private bool ZonaBaja ()
	{
		return (headTransform.position.z <= Media (walkingSample));
	}

	private bool ZonaBaja (float z)
	{
		return z <= Media (walkingSample);
	}

	private bool ZonaAlta ()
	{
		return (headTransform.position.z > Media (walkingSample));
	}

	private bool ZonaAlta (float z)
	{
		return z > Media (walkingSample);
	}

	float getTimeBetweenSteps ()
	{
		float aux = 0.0f;
		int i = 0;
		if (ZonaBaja (walkingSample [0])) {
			while (!ZonaAlta (walkingSample [i])) {
				aux += 0.01f;
				i++;
			}
		} else {
			while (!ZonaBaja (walkingSample [i])) {
				aux += 0.01f;
				i++;
			}
		}

		return aux;
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
			walkingSample.Add (headTransform.position.z);
			yield return new WaitForSeconds (0.01f);
		}

		Debug.Log ("End Training Walking");
		pannelController.SetPannelText ("STOP\nWALKING");
		yield return new WaitForSeconds (2.0f);
		pannelController.SetPannelText ("NOW YOU\nCAN WALK");
		isTraining = false;
	}

	#endregion
}
