﻿using UnityEngine;

public class BasicRayController : MonoBehaviour
{
	public GameObject PrefabDestination;
	public GameObject Player;
	public float AlturaSalto;
	public float DistanciaRayo;
	public PannelController pannelController;

	private GameObject DestinationGO;
	private Movement moveStrategy;

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
		movimientoActual = Movimientos.TELEPORT;
		moveStrategy = gameObject.AddComponent<TeleportMove> ();
		gameObject.GetComponent<TeleportMove> ().TeleportSetData ("Teleport", Player);

		Vector3 cameraForward = Player.transform.GetChild (0).forward;
		Player.transform.forward = new Vector3 (cameraForward.x, Player.transform.forward.y, cameraForward.z);
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, DistanciaRayo)) {
			MovementCasting (hit);
		}
		Move ();
		ChangeMoveMode ();
	}

	#region Funciones para el movimiento

	private void MovementCasting (RaycastHit hit)
	{
		if (hit.collider.tag == "Floor") {
			if (DestinationGO == null) {
				DestinationGO = Instantiate (PrefabDestination, hit.point, Quaternion.LookRotation (hit.normal)) as GameObject;
				DestinationGO.name = "DestinoGO";
			} else {
				DestinationGO.transform.position = hit.point;
			}
			try {
				DestinationGO.GetComponent<ParticleSystem> ().Play ();
			} catch {
				// Do nothing
			}
		}
	}

	private void Move ()
	{
		if (Input.GetMouseButtonDown (0)) {
			if (DestinationGO != null) {
				if (moveStrategy.playerIsMoving ()) {
					moveStrategy.StopMove ();
				}
				moveStrategy.Move ();
			} else {
				Debug.LogError ("Situacion inesperada al intentar mover");
			}
		}

		if (movimientoActual == Movimientos.RUNNING_MOVE) {
			if (Input.GetMouseButtonUp (0)) {
				moveStrategy.StopMove ();
			}
		}
	}

	void ChangeMoveMode ()
	{
		if (Input.GetMouseButtonDown (1)) {
			moveStrategy.StopMove ();
			switch (movimientoActual) {
			case Movimientos.TELEPORT:
				movimientoActual = Movimientos.SMOOTH;
				Destroy (gameObject.GetComponent<TeleportMove> ());
				moveStrategy = gameObject.AddComponent<SmoothMove> ();
				gameObject.GetComponent<SmoothMove> ().SmoothSetData ("Smooth", Player);
				pannelController.SetPannelText ("SMOOTH");
				break;
			case Movimientos.SMOOTH:
				movimientoActual = Movimientos.JUMP;
				Destroy (gameObject.GetComponent<SmoothMove> ());
				moveStrategy = gameObject.AddComponent<JumpMove> ();
				gameObject.GetComponent<JumpMove> ().JumpSetData (AlturaSalto, "Jump", Player);
				pannelController.SetPannelText ("JUMP");
				break;
			case Movimientos.JUMP:
				movimientoActual = Movimientos.NAV_MESH;
				Destroy (gameObject.GetComponent<JumpMove> ());
				moveStrategy = gameObject.AddComponent<NavMeshMove> ();
				gameObject.GetComponent<NavMeshMove> ().NavMeshSetData ("NavMesh", Player);
				pannelController.SetPannelText ("NAV MESH");
				break;
			case Movimientos.NAV_MESH:
				movimientoActual = Movimientos.RUNNING_MOVE;
				Destroy (gameObject.GetComponent<NavMeshMove> ());
				moveStrategy = gameObject.AddComponent<RunningMove> ();
				pannelController.SetPannelText ("RUNNING");
				break;
			case Movimientos.RUNNING_MOVE:
				movimientoActual = Movimientos.TELEPORT;
				Destroy (gameObject.GetComponent<RunningMove> ());
				moveStrategy = gameObject.AddComponent<TeleportMove> ();
				gameObject.GetComponent<TeleportMove> ().TeleportSetData ("Teleport", Player);
				pannelController.SetPannelText ("TELEPORT");
				break;
			}
		}
	}

	#endregion
}