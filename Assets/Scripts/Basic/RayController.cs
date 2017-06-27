using UnityEngine;
using System.Collections;
using System;
using UnityEngine.AI;
using UnityEditor;
using NewtonVR;

public class RayController : MonoBehaviour
{
	const int METODOS_MOVIMIENTO = 4;

	public GameObject destino;
	public GameObject interactable;

	private GameObject interactableGO;
	private GameObject destinoGO;

	public GameObject player;

	private int[] metodosMovimiento;
	private int auxInt;

	private NavMeshAgent agent;

	// Use this for initialization
	void Start ()
	{
		auxInt = 0;
		metodosMovimiento = new int[METODOS_MOVIMIENTO];

		// player = GameObject.Find ("OVRCameraRig");
		agent = player.GetComponent<NavMeshAgent> ();

		for (int i = 0; i < METODOS_MOVIMIENTO; i++) {
			metodosMovimiento [i] = i;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (destinoGO != null || interactableGO != null) {
			Destroy (destinoGO);
			Destroy (interactableGO);
		}

		if (IsPointing ()) {
			Ray ray = new Ray (this.transform.position, this.transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000)) {
				if (hit.collider.transform.tag == "Floor") {
					destinoGO = Instantiate (destino, hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
				} else if (hit.collider.transform.tag == "Interactable") {
					// interactableGO = Instantiate (interactable, hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
				}
			}

			Move (hit);	
		}

		ChangeMoveMode ();
	}

	private bool IsPointing ()
	{
		bool grip = (OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > 0.1f);
		bool point = (OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) <= 0.0f);
		bool indexNearTouch = OVRInput.Get (OVRInput.NearTouch.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

		return grip && point && !indexNearTouch;
	}

	private void Move (RaycastHit hit)
	{
		if (OVRInput.GetDown (OVRInput.Button.One, OVRInput.Controller.RTouch)) {

			if (destinoGO != null) {
				StopAllCoroutines ();
				switch (auxInt) {
				case 0:
					Teletransporte ();
					break;
				case 1:
					DesplazamientoSmooth ();
					break;
				case 2:
					TiroParabolico ();
					break;
				case 3:
					agent.destination = new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ());
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
	}

	void ChangeMoveMode ()
	{
		if (OVRInput.GetDown (OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
			auxInt++;
			if (auxInt > METODOS_MOVIMIENTO - 1) {
				auxInt = 0;
			}
		}
	}

	void Teletransporte ()
	{
		player.transform.position = new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ());
	}

	void TiroParabolico ()
	{
		StartCoroutine (TiroParabolicoCoroutine (player.transform.position, new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ())));
	}

	IEnumerator TiroParabolicoCoroutine (Vector3 posOrigen, Vector3 posDestino)
	{
		float aux = 0.0f;
		Vector3 lerpVector;
		while (transform.position != posDestino) {
			lerpVector = Vector3.Lerp (posOrigen, posDestino, aux);
			player.transform.position = new Vector3 (lerpVector.x, Mathf.Sin (Mathf.LerpAngle (0, Mathf.PI, aux)) * 2.0f + posDestino.y, lerpVector.z);
			aux += 0.005f;
			yield return new WaitForSeconds (0.01f);
		}
	}

	void DesplazamientoSmooth ()
	{
		StartCoroutine (DesplazamientoSmoothCoroutine (player.transform.position, new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ())));
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

	float GetYCoordinate ()
	{
		float y;
		try {
			y = destinoGO.transform.position.y + (player.GetComponent<CharacterController> ().height / 4);
		} catch (Exception e) {
			Debug.Log ("Usando modelo de NewtonVR");
			y = player.transform.position.y;
		}
		return y;
	}

	float GetXCoordinate ()
	{
		return destinoGO.transform.position.x;
	}

	float GetZCoordinate ()
	{
		return destinoGO.transform.position.z;
	}

}
