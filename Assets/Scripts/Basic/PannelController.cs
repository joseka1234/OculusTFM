using System.Collections;
using UnityEngine;

public class PannelController : MonoBehaviour
{
	public string TEXT;
	public Coroutine fadeCoroutine;

	public void SetMoveName ()
	{
		if (fadeCoroutine != null) {
			StopCoroutine (fadeCoroutine);
		}
		TextMesh auxText = GameObject.Find ("Head/Text").GetComponent<TextMesh> ();
		auxText.color = new Color (0, 0, 0, 1);
		auxText.text = TEXT;
		fadeCoroutine = StartCoroutine (FadeText ());
		GameObject.Find ("Head/Text/Plane").GetComponent<MeshRenderer> ().material.color = new Color (1, 1, 1, 1);
	}

	public void SetMoveName (string pannelText)
	{
		if (fadeCoroutine != null) {
			StopCoroutine (fadeCoroutine);
		}
		TextMesh auxText = GameObject.Find ("Head/Text").GetComponent<TextMesh> ();
		auxText.color = new Color (0, 0, 0, 1);
		auxText.text = pannelText;
		GameObject.Find ("Head/Text/Plane").GetComponent<MeshRenderer> ().material.color = new Color (1, 1, 1, 1);
		fadeCoroutine = StartCoroutine (FadeText ());
	}

	private IEnumerator FadeText ()
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

}
