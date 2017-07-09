using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRecognition : MonoBehaviour
{
	private const float EPSILON = 0.5f;
	private const float EXTENSION = 10.0f;

	private bool dibujando;
	private List<Vector2> pattern;

	enum Orientation
	{
		COLINEAR,
		CLOCKWISE,
		COUNTER_CLOCKWISE
	}

	enum Geometry
	{
		LINE,
		CIRCLE,
		RECTANGLE,
		TRIANGLE,
		UNDEFINED
	}

	private struct Rectangle
	{
		public Vector2 center;
		public List<Vector2> axis;
		public float area;
		public Vector2 extent;
	}

	private struct Triangle
	{
		public Vector2 a, b, c;
	}

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
		switch (detectGeometry ()) {
		case Geometry.LINE:
			Debug.Log ("LINE DRAWED");
			break;
		case Geometry.CIRCLE:
			Debug.Log ("CIRCLE DRAWED");
			break;
		case Geometry.RECTANGLE:
			Debug.Log ("RECTANGLE DRAWED");
			break;
		case Geometry.TRIANGLE:
			Debug.Log ("TRIANGLE DRAWED");
			break;
		}
	}

	#region Geometry Detection

	private Geometry detectGeometry ()
	{
		List<Vector2> simplifiedPattern = douglasPeucker (pattern, EPSILON);
		List<Vector2> convexHullofPattern = convexHull (pattern);

		float convexHullPerimeter = perimeterOfPolygon (convexHullofPattern);
		float convexHullArea = areaOfPolygon (convexHullofPattern);
		float areaOfMaximumTriangle = area (maximumAreaEnclosedTriangle (pattern));
		Rectangle auxRectangle = minimumAreaEnclosingRectangle (pattern);
		float rectanglePerimeter = (auxRectangle.extent.x * 2) + (auxRectangle.extent.y * 2);

		if (Mathf.Pow (convexHullPerimeter, 2) / convexHullArea >= 55.0f) {
			return Geometry.LINE;
		}

		if (Mathf.Pow (convexHullPerimeter, 2) / convexHullArea < 14.0f) {
			return Geometry.CIRCLE;
		}

		if (areaOfMaximumTriangle / convexHullArea >= 0.8f) {
			return Geometry.TRIANGLE;
		}

		if (convexHullPerimeter / rectanglePerimeter >= 0.9f) {
			return Geometry.RECTANGLE;
		}

		return Geometry.UNDEFINED;
	}

	private float perimeterOfPolygon (List<Vector2> points)
	{
		float aux = 0.0f;
		for (int i = 0; i < points.Count - 1; i++) {
			aux += distanciaPuntoPunto (points [i], points [i + 1]);
		}
		return aux;
	}

	private float areaOfPolygon (List<Vector2> points)
	{
		float aux = 0.0f;
		for (int i = 0; i < points.Count - 1; i++) {
			aux += areaUnderSegment (points [i], points [i + 1]);
		}
		return aux;
	}

	private float areaUnderSegment (Vector2 a, Vector2 b)
	{
		float height = (a.y + b.y) / 2;
		float width = (a.x + b.x) / 2;
		return height * width;
	}

	private Triangle maximumAreaEnclosedTriangle (List<Vector2> Pattern)
	{
		Triangle auxTriangle;

		auxTriangle.a = Pattern [0];
		auxTriangle.b = Pattern [1];
		auxTriangle.c = Pattern [2];
		int a = 0, b = 1, c = 2;

		do {
			while (true) {
				while (area (Pattern [a], Pattern [b], Pattern [c + 1]) >= area (Pattern [a], Pattern [b], Pattern [c])) {
					c++;
				}
				if (area (Pattern [a], Pattern [b + 1], Pattern [c]) >= area (Pattern [a], Pattern [b], Pattern [c])) {
					b++;
				} else {
					break;
				}
			}

			if (area (Pattern [a], Pattern [b], Pattern [c]) > area (auxTriangle.a, auxTriangle.b, auxTriangle.c)) {
				auxTriangle.a = Pattern [a];
				auxTriangle.b = Pattern [b];
				auxTriangle.c = Pattern [c];
			}

			a++;
			b = (a == b) ? b + 1 : b;
			c = (b == c) ? c + 1 : c;
		} while (a == 0);

		return auxTriangle;
	}

	private float area (Triangle T)
	{
		return area (T.a, T.b, T.c);
	}

	private float area (Vector2 a, Vector2 b, Vector2 c)
	{
		Vector2 u = perp (getVectorFromPoints (a, b));
		Vector2 v = getVectorFromPoints (a, c);
		return 0.5f * Mathf.Abs (u.x * v.x + u.y * v.y);
	}

	private Vector2 getVectorFromPoints (Vector2 a, Vector2 b)
	{
		return new Vector2 (b.x - a.x, b.y - a.y);
	}

	private Rectangle minimumAreaEnclosingRectangle (List<Vector2> Pattern)
	{
		// Initialize aux rectangle
		Rectangle aux = new Rectangle ();
		aux.area = float.MaxValue;
		aux.axis = new List<Vector2> ();
		aux.axis.Add (new Vector2 (0, 0));
		aux.axis.Add (new Vector2 (0, 0));
		aux.extent.x = 0.0f;
		aux.extent.y = 0.0f;

		// Begin algorithm
		for (int i = Pattern.Count - 1, j = 0; j < Pattern.Count; i = j++) {
			
			Vector2 origin = Pattern [i];
			Vector2 U0 = Pattern [j] - origin;
			U0.Normalize ();
			Vector2 U1 = -perp (U0);
			float min0 = 0, max0 = 0, max1 = 0;

			for (int k = 0; k < Pattern.Count; ++k) {
				Vector2 D = Pattern [k] - origin;

				float dot = Vector2.Dot (U0, D);
				if (dot < min0) {
					min0 = dot;
				} else if (dot > max0) {
					max0 = dot;
				}

				dot = Vector2.Dot (U1, D);
				if (dot > max1) {
					max1 = dot;
				}
			}

			float rectangleArea = (max0 - min0) * max1;
			if (rectangleArea < aux.area) {
				aux.center = origin + ((min0 + max0) / 2.0f) * U0 + (max1 / 2.0f) * U1;
				aux.axis [0] = U0;
				aux.axis [1] = U1;
				aux.extent.x = (max0 - min0) / 2.0f;
				aux.extent.y = max1 / 2.0f;
				aux.area = rectangleArea;
			}
		}

		return aux;
	}

	private Vector2 perp (Vector2 vector)
	{
		return new Vector2 (vector.y, -vector.x);
	}

	private List<Vector2> convexHull (List<Vector2> Pattern)
	{
		List<Vector2> aux = new List<Vector2> ();
		if (Pattern.Count < 3) {
			return aux;
		}
		int p = getLeftmostIndexPoint (Pattern);
		int q;
		do {
			aux.Add (Pattern [p]);
			q = (p + 1) % Pattern.Count;
			for (int i = 0; i < Pattern.Count; i++) {
				if (getOrientation (Pattern [p], Pattern [i], Pattern [q]) == Orientation.COUNTER_CLOCKWISE) {
					q = i;
				}
			}
			p = q;
		} while (p != 1);

		return aux;
	}

	private int getLeftmostIndexPoint (List<Vector2> points)
	{
		int aux = 0;
		for (int i = 1; i < points.Count; i++) {
			if (points [i].x < points [aux].x) {
				aux = i;
			}
		}

		return aux;
	}

	private float cross (Vector2 A, Vector2 B, Vector2 C)
	{
		return ((A.x - C.x) * (B.y - C.y)) - ((A.y - C.y) * (B.x - C.x));
	}

	private Orientation getOrientation (Vector2 A, Vector2 B, Vector2 C)
	{
		float aux = (B.y - A.y) * (C.x - B.x) - (B.x - A.x) * (C.y - B.y);

		if (aux == 0.0f) {
			return Orientation.COLINEAR;
		}
		return (aux > 0.0f) ? Orientation.CLOCKWISE : Orientation.COUNTER_CLOCKWISE;
	}

	#endregion

	#region Doulgas Peucker Algorithm

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

	#endregion
}
