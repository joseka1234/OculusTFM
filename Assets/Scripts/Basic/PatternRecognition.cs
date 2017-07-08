using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRecognition : MonoBehaviour
{
	private const float EPSILON = 0.5f;

	private bool dibujando;
	private List<Vector2> pattern;

	// Use this for initialization
	void Start ()
	{
		pattern = new List<Vector2> ();
		dibujando = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (OVRInput.GetDown (OVRInput.Button.One, OVRInput.Controller.RTouch)) {
			StartCoroutine (getPattern ());
		}
	}

	private IEnumerator getPattern ()
	{
		pattern.Clear ();
		while (!OVRInput.GetUp (OVRInput.Button.One, OVRInput.Controller.RTouch)) {
			pattern.Add (new Vector2 (transform.position.x, transform.position.y));
			yield return new WaitForSeconds (0.01f);
		}
	}

	private void detectGeometry ()
	{
		List<Vector2> simplifiedPattern = douglasPeucker (pattern, EPSILON);
	}

	private List<Vector2> douglasPeucker (List<Vector2> Pattern, float tolerance)
	{
		if (Pattern.Count < 3) {
			return Pattern;
		}

		List<Vector2> Segmento = new List<Vector2> ();
		Segmento.Add (Pattern [0]);
		Segmento.Add (Pattern [Pattern.Count - 1]);
		Vector2 puntoLejano = new Vector2 (0, 0);
		float maxDistance = -1;
		float distancia;

		for (int i = 2; i < Pattern.Count; i++) {
			distancia = distanciaPuntoSegmento (Segmento [0], Segmento [1], Pattern [i]);
			if (distancia > maxDistance) {
				maxDistance = distancia;
				puntoLejano = Pattern [i];
			}
		}

		if (maxDistance < tolerance) {
			return Segmento;
		} else {
			List<Vector2> left = splitPattern (Pattern, puntoLejano);
			List<Vector2> right = splitPattern (puntoLejano, Pattern);
			return mergePatterns (left, right);
		}
	}

	private List<Vector2> splitPattern (List<Vector2> Pattern, Vector2 splitPoint)
	{

		List<Vector2> aux = new List<Vector2> ();
		for (int i = 0; i < Pattern.Count; i++) {
			aux.Add (Pattern [i]);
			if (Pattern [i] == splitPoint) {
				return aux;
			}
		}

		return aux;
	}

	private List<Vector2> splitPattern (Vector2 splitPoint, List<Vector2> Pattern)
	{
		List<Vector2> aux = new List<Vector2> ();
		bool startSplit = false;
		for (int i = 0; i < Pattern.Count; i++) {
			if (Pattern [i] == splitPoint) {
				startSplit = true;
			}

			if (startSplit) {
				aux.Add (pattern [i]);
			}
		}

		return aux;
	}

	private List<Vector2> mergePatterns (List<Vector2> A, List<Vector2> B)
	{
		List<Vector2> aux = A;
		for (int i = 1; i < B.Count; i++) {
			aux.Add (B [i]);
		}

		return aux;
	}

	private float distanciaPuntoSegmento (Vector2 A, Vector2 B, Vector2 C)
	{
		float U = getU (A, B, C);

		if (U >= 0 && U <= 1) {
			return distanciaPuntoPunto (getP (A, B, C), C);
		} else if (U > 1) {
			return distanciaPuntoPunto (B, C);
		} else if (U < 0) {
			return distanciaPuntoPunto (A, C);
		}

		return -1;
	}

	private Vector2 getP (Vector2 A, Vector2 B, Vector2 C)
	{
		float U = getU (A, B, C);
		float x = A.x + U * (B.x - A.x);
		float y = A.y + U * (B.y - A.y);

		return new Vector2 (x, y);
	}

	private float getU (Vector2 A, Vector2 B, Vector2 P)
	{
		float dividendo = ((P.x - A.x) * (B.x - A.x)) + ((P.y - A.y) * (B.y - A.y));
		float divisor = Mathf.Pow (B.x - A.x, 2) + Mathf.Pow (B.y - A.y, 2);
		return dividendo / divisor;
	}

	private float distanciaPuntoPunto (Vector2 A, Vector2 B)
	{
		return Mathf.Sqrt (Mathf.Pow (B.x - A.x, 2) + Mathf.Pow (B.y - A.y, 2));
	}
}
