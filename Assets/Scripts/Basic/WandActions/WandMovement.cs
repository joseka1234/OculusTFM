using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandMovement : WandAction
{
	private const int DISTANCIA_RAYO = 5000;
	private const int ALTURA_SALTO = 100;

	private GameObject PrefabDestination;
	private GameObject DestinationGO;
	private GameObject Player;
	private PannelController pannelController;
	private Movement moveStrategy;
	private Movimientos movimientoActual;

	private enum Movimientos
	{
		TELEPORT,
		SMOOTH,
		JUMP,
		NAV_MESH,
		RUNNING_MOVE
	}

	public void SetWandMovementData (HandType Hand)
	{
		this.Hand = Hand;
		Player = GameObject.Find ("VRPlayer");
		PrefabDestination = Resources.Load ("Destino") as GameObject;
		pannelController = Player.GetComponentInChildren<PannelController> ();
		movimientoActual = Movimientos.TELEPORT;
		moveStrategy = gameObject.AddComponent<TeleportMove> ();
		Player.AddComponent<TeleportMove> ().TeleportSetData ("Teleport", Player);
		Vector3 cameraForward = Player.transform.GetChild (0).forward;
		Player.transform.forward = new Vector3 (cameraForward.x, Player.transform.forward.y, cameraForward.z);
	}

	protected override void ButtonTwoPressed ()
	{
		ChangeMoveMode ();
	}

	protected override void ButtonOnePressed ()
	{
		Move ();
	}

	protected override void ButtonTwoReleased ()
	{
		throw new System.NotImplementedException ();
	}

	protected override void ButtonOneReleased ()
	{
		throw new System.NotImplementedException ();
	}

	protected override void UpdateAction ()
	{
		Ray ray = new Ray (this.transform.position, this.transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, DISTANCIA_RAYO)) {
			MovementCasting (hit);
		}
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
			}
		}
	}

	private void Move ()
	{
		if (DestinationGO != null) {
			if (moveStrategy.playerIsMoving ()) {
				moveStrategy.StopMove ();
			}
			moveStrategy.Move ();
		} else {
			Debug.LogError ("Situacion inesperada al intentar mover");
		}

		if (movimientoActual == Movimientos.RUNNING_MOVE) {
			if (OVRInput.GetUp (OVRInput.Button.One, OVRInput.Controller.RTouch)) {
				moveStrategy.StopMove ();
			}
		}
	}

	void ChangeMoveMode ()
	{
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
			gameObject.GetComponent<JumpMove> ().JumpSetData (ALTURA_SALTO, "Jump", Player);
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
			gameObject.GetComponent<RunningMove> ().RunningSetData ("Running", Player, pannelController, 2.0f);
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

	#endregion
}
