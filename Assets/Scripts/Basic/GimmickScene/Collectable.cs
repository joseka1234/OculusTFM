using UnityEngine;

public class Collectable : MonoBehaviour
{

	private const float ROTATION_INCREMENT = 1.0f;

	void Update ()
	{

		transform.Rotate (new Vector3 (0, 0, ROTATION_INCREMENT));
	}

	void OnCollisionEnter (Collision col)
	{
		if (col.transform.tag == "Player") {
			GameObject.Find ("GameManager").GetComponent<GameManager> ().IncrementScore ();
			Destroy (gameObject);
		}
	}
}
