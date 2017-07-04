using UnityEngine;

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
		movimientoActual = Movimientos.TELEPORT;
		moveStrategy = gameObject.AddComponent<TeleportMove> ();
		gameObject.GetComponent<TeleportMove> ().TeleportSetData ("Teleport", Player);
	
		pannelController = GameObject.Find ("Text").GetComponent<PannelController> ();
		Vector3 cameraForward = Player.transform.GetChild (0).forward;
		Player.transform.forward = new Vector3 (cameraForward.x, Player.transform.forward.y, cameraForward.z);
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
				if (movimientoActual != Movimientos.RUNNING_MOVE) {
					moveStrategy.Move ();
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
				gameObject.GetComponent<RunningMove> ().RunningSetData ("Running", Player, pannelController, 2.0f);
				gameObject.GetComponent<RunningMove> ().Move ();
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