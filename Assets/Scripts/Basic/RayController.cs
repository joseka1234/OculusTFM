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

	private GameObject DestinationGO;

	private Movement moveStrategy;
	private PannelController pannelController;

	private enum Movimientos
	{
		TELEPORT,
		SMOOTH,
		JUMP,
		NAV_MESH,
		RUNNING_MOVE
	}

	private Movimientos movimientoActual;

	// Use this for initialization
	void Start ()
	{
		pannelController = new PannelController ();
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
				MovementCasting (hit);
			}

			Move (hit);
		} else {
			Destroy (DestinationGO);
		}

		ChangeMoveMode ();
	}

	#region Funciones para el movimiento y la interacción (TODO: Quitar sección de interacción)

	private void MovementCasting (RaycastHit hit)
	{
		if (DestinationGO == null) {
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
				pannelController.SetMoveName ("SMOOTH");
				break;
			case Movimientos.SMOOTH:
				movimientoActual = Movimientos.JUMP;
				moveStrategy = new JumpMove (AlturaSalto, "Jump", Player);
				pannelController.SetMoveName ("JUMP");
				break;
			case Movimientos.JUMP:
				movimientoActual = Movimientos.NAV_MESH;
				moveStrategy = new NavMeshMove ("NavMesh", Player);
				pannelController.SetMoveName ("NAV MESH");
				break;
			case Movimientos.NAV_MESH:
				movimientoActual = Movimientos.RUNNING_MOVE;
				moveStrategy = new RunningMove ("Running", Player, pannelController);
				pannelController.SetMoveName ("RUNNING");
				break;
			case Movimientos.RUNNING_MOVE:
				movimientoActual = Movimientos.TELEPORT;
				moveStrategy = new  TeleportMove ("Teleport", Player);
				pannelController.SetMoveName ("TELEPORT");
				break;
			}
		}
	}

	#endregion
}