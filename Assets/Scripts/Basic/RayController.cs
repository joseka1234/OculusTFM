using UnityEngine;
using System.Collections;
using System;
using UnityEngine.AI;
using UnityEditor;
using NewtonVR;
using UnityEngine.UI;

public class RayController : MonoBehaviour
{
	const int METODOS_MOVIMIENTO = 4;

	public GameObject destino;
	public GameObject interactable;
	public GameObject player;

	public float AlturaSalto;

	private GameObject interactableGO;
	private GameObject destinoGO;

	private int[] metodosMovimiento;
	private int auxInt;

	private NavMeshAgent agent;

	private static bool isMoving;

	private Coroutine fadeCoroutine;
	private Coroutine moveCoroutine;

	// Use this for initialization
	void Start ()
	{
		isMoving = false;
		Vector3 cameraForward = player.transform.GetChild (0).forward;
		player.transform.forward = new Vector3 (cameraForward.x, player.transform.forward.y, cameraForward.z);
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
		if (HandIsPointing ()) {
			Ray ray = new Ray (this.transform.position, this.transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100)) {
				if (hit.collider.transform.tag == "Floor") {
					if (destinoGO == null) {
						Destroy (interactableGO);
						destinoGO = Instantiate (destino, hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
					} else {
						destinoGO.transform.position = hit.point;
					}
					try {
						destinoGO.GetComponent<ParticleSystem> ().Play ();
					} catch (Exception e) {
						// Debug.Log ("No se está usando un emisor de partículas como destino");
					}

				} else if (hit.collider.transform.tag == "Interactable") {
					Destroy (destinoGO);
					// interactableGO = Instantiate (interactable, hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
				}
			}

			Move (hit);	
		} else {
			Destroy (interactableGO);
			Destroy (destinoGO);
		}

		ChangeMoveMode ();
	}

	private bool HandIsPointing ()
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
				if (!isMoving) {
					if (moveCoroutine != null) {
						StopCoroutine (moveCoroutine);
					}
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
						Debug.Log ("Desplazamiento con NavMesh");
						// agent.destination = new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ());
						break;
					}
				}
			} else if (interactableGO != null) {
				// Hacer lo necesario para interactuar
				// TODO: Quitar la parte de interacción ya que NewtonVR se encarga solo
				try {
					Debug.Log ("Interactuando con " + hit.collider.name);
				} catch (Exception e) {
					return;
				}
			} else {
				Debug.LogError ("Situacion inesperada al intentar mover");
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
			switch (auxInt) {
			case 0:
				SetMoveName ("Teleport");
				break;
			case 1:
				SetMoveName ("Smooth");
				break;
			case 2:
				SetMoveName ("Jump");
				break;
			case 3:
				SetMoveName ("NavMesh");
				break;
			}
		}
	}

	void Teletransporte ()
	{
		player.transform.position = new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ());
		player.transform.forward = GetNewForward ();
	}

	void TiroParabolico ()
	{
		moveCoroutine = StartCoroutine (TiroParabolicoCoroutine (player.transform.position, new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ())));
	}

	IEnumerator TiroParabolicoCoroutine (Vector3 posOrigen, Vector3 posDestino)
	{
		isMoving = true;
		float aux = 0.0f;
		Vector3 lerpVector;
		Vector3 newForward = GetNewForward ();
		while (transform.position != posDestino) {
			lerpVector = Vector3.Lerp (posOrigen, posDestino, aux);
			player.transform.position = new Vector3 (lerpVector.x, Mathf.Sin (Mathf.LerpAngle (0, Mathf.PI, aux)) * AlturaSalto + posDestino.y, lerpVector.z);
			player.transform.forward = Vector3.Lerp (player.transform.forward, newForward, aux);
			aux += 0.005f;

			if (player.transform.position == posDestino) {
				isMoving = false;
			}

			yield return new WaitForSeconds (0.01f);
		}
	}

	void DesplazamientoSmooth ()
	{
		moveCoroutine = StartCoroutine (DesplazamientoSmoothCoroutine (player.transform.position, new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ())));
	}

	IEnumerator DesplazamientoSmoothCoroutine (Vector3 posOrigen, Vector3 posDestino)
	{
		isMoving = true;
		float aux = 0.0f;
		Vector3 newForward = GetNewForward ();
		while (transform.position != posDestino) {
			player.transform.position = Vector3.Lerp (posOrigen, posDestino, aux);
			player.transform.forward = Vector3.Lerp (player.transform.forward, newForward, aux);
			aux += 0.005f;

			if (player.transform.position == posDestino) {
				isMoving = false;
			}

			yield return new WaitForSeconds (0.01f);
		}
		isMoving = false;
	}

	float GetYCoordinate ()
	{
		float y;
		try {
			y = destinoGO.transform.position.y + (player.GetComponent<CharacterController> ().height / 4);
		} catch (Exception e) {
			// Debug.Log ("Usando modelo de NewtonVR");
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


	Vector3 GetNewForward ()
	{
		return new Vector3 (this.transform.forward.x, player.transform.forward.y, this.transform.forward.z);
	}

	void SetMoveName (string moveName)
	{	
		if (fadeCoroutine != null) {
			StopCoroutine (fadeCoroutine);
		}
		TextMesh auxText = GameObject.Find ("Head/Text").GetComponent<TextMesh> ();
		auxText.color = new Color (0, 0, 0, 1);
		auxText.text = moveName;
		fadeCoroutine = StartCoroutine (FadeText ());
		GameObject.Find ("Head/Text/Plane").GetComponent<MeshRenderer> ().material.color = new Color (1, 1, 1, 1);
	}

	IEnumerator FadeText ()
	{
		TextMesh auxText = GameObject.Find ("Head/Text").GetComponent<TextMesh> ();
		MeshRenderer plane = GameObject.Find ("Head/Text/Plane").GetComponent<MeshRenderer> ();
		float aux = 0.0f;
		while (auxText.color.a != 0) {
			auxText.color = Color.Lerp (new Color (0, 0, 0, 1), new Color (0, 0, 0, 0), aux);
			plane.material.color = Color.Lerp (new Color (1, 1, 1, 1), new Color (1, 1, 1, 0), aux);
			aux += 0.05f;
			yield return new WaitForSeconds (0.1f);
		}
	}
}