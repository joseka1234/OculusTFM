using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TeleportMove : Movement
{

	private GameObject Fader;

	public void TeleportSetData (string Name, GameObject player)
	{
		SetData (Name, player);
		Fader = GameObject.Find ("Fader");
	}

	override public void Move ()
	{
		StartCoroutine (MoveCoroutine ());

	}

	private IEnumerator MoveCoroutine ()
	{
		float aux = 0.0f;

		while (aux < 1.0f) {
			Fader.GetComponent<Image> ().color = Color.Lerp (new Color (0, 0, 0, 0), new Color (0, 0, 0, 1), aux);
			aux += 0.1f;
			yield return new WaitForSeconds (0.05f);
		}

		player.transform.position = new Vector3 (GetXCoordinate (), GetYCoordinate (), GetZCoordinate ());

		aux = 0.0f;
		while (aux < 1.0f) {
			Fader.GetComponent<Image> ().color = Color.Lerp (new Color (0, 0, 0, 1), new Color (0, 0, 0, 0), aux);
			aux += 0.1f;
			yield return new WaitForSeconds (0.05f);
		}
	}

	override public void StopMove ()
	{
	}
}
