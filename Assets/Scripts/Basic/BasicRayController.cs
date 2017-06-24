using UnityEngine;
using System.Collections;
using System;
using UnityEngine.AI;

public class BasicRayController : MonoBehaviour
{
	const int METODOS_MOVIMIENTO = 4;

	public GameObject destino;
	public GameObject interactable;

	private GameObject interactableGO;
	private GameObject destinoGO;

	private GameObject player;

	private int[] metodosMovimiento;
	private int auxInt;

	private NavMeshAgent agent;

	// Use this for initialization
	void Start ()
	{
		auxInt = 0;
		metodosMovimiento = new int[METODOS_MOVIMIENTO];

		player = GameObject.Find ("PlayerNoVR");
		agent = player.GetComponent<NavMeshAgent> ();

		for (int i = 0; i < METODOS_MOVIMIENTO; i++) {
			metodosMovimiento [i] = i;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 1000)) {
			Destroy (destinoGO);
			Destroy (interactableGO);
			if (hit.collider.transform.tag == "Floor") {
				destinoGO = Instantiate (destino, hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
			} else if (hit.collider.transform.tag == "Interactable") {
				// interactableGO = Instantiate (interactable, hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
			}
		}

		if (Input.GetMouseButtonDown (0)) {

			if (destinoGO != null) {
				StopAllCoroutines ();
				switch (auxInt) {
				case 0:
					Teletransporte (hit);
					break;
				case 1:
					DesplazamientoSmooth (hit);
					break;
				case 2:
					TiroParabolico (hit);
					break;
				case 3:
					agent.destination = hit.point;
					break;
				}
			} else if (interactableGO != null) {
				// Hacer lo necesario para interactuar
				try {
					Debug.Log ("Interactuando con " + hit.collider.name);
				} catch (Exception e) {
					return;
				}
			}
		}

		if (Input.GetMouseButtonDown (1)) {
			auxInt++;
			if (auxInt > METODOS_MOVIMIENTO - 1) {
				auxInt = 0;
			}
			switch (auxInt) {
			case 0:
				Debug.Log ("Teletransporte");
				break;
			case 1:
				Debug.Log ("Movimiento Constante");
				break;
			case 2:
				Debug.Log ("Salto");
				break;
			case 3:
				Debug.Log ("NavMesh Move");
				break;
			}
		}
	}

	void Teletransporte (RaycastHit hit)
	{
		player.transform.position = new Vector3 (hit.point.x, 1, hit.point.z);
	}

	void TiroParabolico (RaycastHit hit)
	{
		StartCoroutine (TiroParabolicoCoroutine (player.transform.position, new Vector3 (hit.point.x, 1, hit.point.z)));
	}

	IEnumerator TiroParabolicoCoroutine (Vector3 posOrigen, Vector3 posDestino)
	{
		float aux = 0.0f;
		Vector3 lerpVector;
		while (transform.position != posDestino) {
			lerpVector = Vector3.Lerp (posOrigen, posDestino, aux);
			player.transform.position = new Vector3 (lerpVector.x, Mathf.Sin (Mathf.LerpAngle (0, Mathf.PI, aux)) * 2.0f + 1.0f, lerpVector.z);
			aux += 0.005f;
			yield return new WaitForSeconds (0.01f);
		}
	}

	void DesplazamientoSmooth (RaycastHit hit)
	{
		StartCoroutine (DesplazamientoSmoothCoroutine (player.transform.position, new Vector3 (hit.point.x, 1, hit.point.z)));
	}

	IEnumerator DesplazamientoSmoothCoroutine (Vector3 posOrigen, Vector3 posDestino)
	{
		float aux = 0.0f;
		while (transform.position != posDestino) {
			player.transform.position = Vector3.Lerp (posOrigen, posDestino, aux);
			aux += 0.005f;
			yield return new WaitForSeconds (0.01f);
		}
	}

}
