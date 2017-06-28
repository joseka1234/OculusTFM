using UnityEngine;
using System.Collections;
using System;
using UnityEngine.AI;

public class RayController : MonoBehaviour
{
	public GameObject PrefabDestination;
	public GameObject PrefabInteractable;
	public GameObject Player;
	public float AlturaSalto;

	private GameObject InteractableGO;
	private GameObject DestinationGO;

	private Coroutine fadeCoroutine;
	private Movement moveStrategy;

	private enum Movimientos
	{
		TELEPORT,
		SMOOTH,
		JUMP,
		NAV_MESH
	}

	private Movimientos movimientoActual;

	// Use this for initialization
	void Start ()
	{
		Vector3 cameraForward = Player.transform.GetChild (0).forward;
		Player.transform.forward = new Vector3 (cameraForward.x, Player.transform.forward.y, cameraForward.z);
		movimientoActual = Movimientos.TELEPORT;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (HandIsPointing ()) {
			Ray ray = new Ray (this.transform.position, this.transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 100)) {
				switch (hit.collider.transform.tag) {
				case "Floor":
					MovementCasting (hit);
					break;
				case "Interactable":
					InteractableCasting (hit);
					break;
				}
			}

			Move (hit);
		} else {
			Destroy (InteractableGO);
			Destroy (DestinationGO);
		}

		ChangeMoveMode ();
	}

	#region Funciones para el movimiento y la interacción (TODO: Quitar sección de interacción)

	private void MovementCasting (RaycastHit hit)
	{
		if (DestinationGO == null) {
			Destroy (InteractableGO);
			DestinationGO = Instantiate (PrefabDestination, hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
			DestinationGO.name = "DestinoGO";
		} else {
			DestinationGO.transform.position = hit.point;
		}
		try {
			DestinationGO.GetComponent<ParticleSystem> ().Play ();
		} catch {
		}
	}

	private void InteractableCasting (RaycastHit hit)
	{
		Destroy (DestinationGO);
		// interactableGO = Instantiate (interactable, hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
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

			if (DestinationGO != null) {
				if (moveStrategy.playerIsMoving ()) {
					moveStrategy.StopMove ();
				}
				moveStrategy.Move ();
			} else if (InteractableGO != null) {
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
			switch (movimientoActual) {
			case Movimientos.TELEPORT:
				movimientoActual = Movimientos.SMOOTH;
				moveStrategy = new SmoothMove ("Smooth", Player);
				break;
			case Movimientos.SMOOTH:
				movimientoActual = Movimientos.JUMP;
				moveStrategy = new JumpMove (AlturaSalto, "Jump", Player);
				break;
			case Movimientos.JUMP:
				movimientoActual = Movimientos.NAV_MESH;
				moveStrategy = new NavMeshMove ("NavMesh", Player);
				break;
			case Movimientos.NAV_MESH:
				movimientoActual = Movimientos.TELEPORT;
				moveStrategy = new TeleportMove ("Teleport", Player);
				break;
			}
		}
	}

	#endregion

	#region GUI de movimiento

	void SetMoveName ()
	{
		if (fadeCoroutine != null) {
			StopCoroutine (fadeCoroutine);
		}
		TextMesh auxText = GameObject.Find ("Head/Text").GetComponent<TextMesh> ();
		auxText.color = new Color (0, 0, 0, 1);
		auxText.text = moveStrategy.Name;
		fadeCoroutine = StartCoroutine (FadeText ());
		GameObject.Find ("Head/Text/Plane").GetComponent<MeshRenderer> ().material.color = new Color (1, 1, 1, 1);
	}

	IEnumerator FadeText ()
	{
		TextMesh auxText = GameObject.Find ("Head/Text").GetComponent<TextMesh> ();
		MeshRenderer plane = GameObject.Find ("Head/Text/Plane").GetComponent<MeshRenderer> ();
		float aux = 0.0f;
		while (auxText.color.a > 0) {
			auxText.color = Color.Lerp (new Color (0, 0, 0, 1), new Color (0, 0, 0, 0), aux);
			plane.material.color = Color.Lerp (new Color (1, 1, 1, 1), new Color (1, 1, 1, 0), aux);
			aux += 0.05f;
			yield return new WaitForSeconds (0.1f);
		}
	}

	#endregion
}