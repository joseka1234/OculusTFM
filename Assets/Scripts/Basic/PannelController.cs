using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PannelController : MonoBehaviour
{
	public string TEXT;
	public Coroutine fadeCoroutine;

	public void SetPannelText (string pannelText)
	{
		if (fadeCoroutine != null) {
			StopCoroutine (fadeCoroutine);
		}
		Text auxText = transform.GetChild (1).GetComponent<Text> ();
		auxText.color = new Color (0.0f, 0.0f, 0.0f, 1.0f);
		auxText.text = pannelText;

		transform.GetChild (0).GetComponent<Image> ().color = new Color (0.67f, 1.0f, 1.0f, 1.0f);
		fadeCoroutine = StartCoroutine ("FadeText");
	}

	private IEnumerator FadeText ()
	{
		Text auxText = transform.GetChild (1).GetComponent<Text> ();
		Image plane = transform.GetChild (0).GetComponent<Image> ();
		float aux = 0.0f;
		while (auxText.color.a > 0) {
			auxText.color = Color.Lerp (new Color (0.0f, 0.0f, 0.0f, 1.0f), new Color (0.0f, 0.0f, 0.0f, 0.0f), aux);
			plane.color = Color.Lerp (new Color (0.67f, 1.0f, 1.0f, 1.0f), new Color (0.67f, 1.0f, 1.0f, 0.0f), aux);
			aux += 0.05f;
			yield return new WaitForSeconds (0.1f);
		}
	}
}