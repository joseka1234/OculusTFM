using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FocoDinamico : MonoBehaviour
{

	public float RadioCono;
	public float LongitudCono;

	public float PRM;
	public float PD;
	public float PV;

	const int RAYS_PER_LAYER = 50;
	const int LAYERS = 20;

	private List<RaycastHit> hits;
	private Dictionary<string,float> heuristicValues;

	// Use this for initialization
	void Start ()
	{
		hits = new List<RaycastHit> ();
		heuristicValues = new Dictionary<string, float> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag ("Interactable")) {
			obj.layer = 0;
		}
		this.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField> ().focalTransform = getObjectFocus ().transform;
		// focus.transform.localScale = Vector3.one / 2.0f;
	}

	void RayosCono (GameObject obj)
	{
		hits.Clear ();
		RaycastHit auxHit;
		Vector3 forward = Camera.main.transform.forward;
		float inc = (2 * Mathf.PI) / RAYS_PER_LAYER;
		float angle = 0;

		float incrementoRadio = RadioCono / LAYERS;
		float radio = 0.0f;

		for (int i = 0; i < LAYERS; i++) {
			for (int j = 0; j < RAYS_PER_LAYER; j++) {
				Vector3 pointCircle = (transform.right * Mathf.Cos (angle) * radio) +
				                      (transform.up * Mathf.Sin (angle) * radio);

				Vector3 direction = (pointCircle - transform.forward);

				Ray rayo = new Ray (transform.position, -direction);
				Physics.Raycast (rayo, out auxHit, LongitudCono);

				if (auxHit.collider != null) {
					if (auxHit.collider.name == obj.name) {
						hits.Add (auxHit);
					}
				}

				/*
				// Mostrar rayos de focus
				Debug.DrawRay (rayo.origin, rayo.direction, Color.red);
				if (auxHit.collider != null && auxHit.collider.tag == "Interactable") {
					Debug.DrawRay (rayo.origin, auxHit.point - rayo.origin, Color.red);
				}
				*/

				angle += inc;
			}
			angle = 0;
			radio += incrementoRadio;
		}
	}

	float RM (GameObject obj)
	{
		RayosCono (obj);
		float aux = 0.0f;
		foreach (RaycastHit hit in hits) {
			if (hit.collider.tag != obj.name) {
				aux += Vector3.Distance (hit.point, transform.forward * LongitudCono);
			}
		}
		return aux;
	}

	float D (GameObject obj)
	{
		return Vector3.Distance (obj.transform.position, transform.position);
	}

	float V (GameObject obj)
	{
		switch (obj.name) {
		case "Cube":
			return 1.0f;
			break;
		case "Sphere":
			return 1.0f;
			break;
		case "Default":
			return 0.0f;
			break;
		}

		return 0.0f;
	}

	void DefineHeuristicValues ()
	{
		heuristicValues.Clear ();
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag ("Interactable")) {
			heuristicValues.Add (obj.name, CalcularValorHeuristica (obj));
		}
	}

	float CalcularValorHeuristica (GameObject objetivo)
	{
		return (PRM * RM (objetivo)) + (PD * D (objetivo)) + (PV * V (objetivo));
	}

	GameObject getObjectFocus ()
	{
		DefineHeuristicValues ();
		string focusedName = heuristicValues.FirstOrDefault (x => x.Value == heuristicValues.Values.Max ()).Key;
		return GameObject.Find (focusedName);
	}
}
