using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PatternRecognition : MonoBehaviour
{
	public float samplesPerSecond;
	protected Color magicColor;
	public WandAction wandAction;
	public HandType Hand;

	private const float EPSILON = 1.5f;
	private const float EXTENSION = 10.0f;
	private bool dibujando;
	private bool magicIntersection;
	private List<Vector2> pattern;
	private Coroutine patternCoroutine;
	private GameObject MagicRing;

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

	public enum HandType
	{
		LEFT,
		RIGHT
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
		magicIntersection = false;
		magicColor = Color.white;
		MagicRing = transform.GetChild (0).GetChild (1).gameObject;
	}

	/// <summary>
	///  Usamos el booleano start para corregir el bug del sistema por el cual se desasigna la textura del sistema de
	///  partículas de la punta de la varita.
	/// </summary>
	bool start = true;

	void Update ()
	{
		MagicRing.GetComponent<Renderer> ().material.color = magicColor;
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		VRPatternRecorder ();
		VRPatternAnalysis ();
	}

	#region Acciones de cada varita

	private void DeleteReferences ()
	{
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Reference")) {
			Destroy (obj.gameObject);
		}
		// Cada vez que cambiemos de acción destruimos la anterior para evitar tener más de una instancia del mismo script.
		Destroy (this.gameObject.GetComponent<WandAction> ());
	}

	private void ExecuteAction (Geometry gometria)
	{
		switch (gometria) {
		case Geometry.TRIANGLE:
			Debug.Log ("Nada implementado");
			break;
		case Geometry.CIRCLE:
			Debug.Log ("Nada implementado");
			break;
		case Geometry.LINE:
			wandAction = this.gameObject.AddComponent<WandMovement> ();
			gameObject.GetComponent<WandMovement> ().SetWandMovementData ((Hand == HandType.RIGHT) ? WandAction.HandType.RIGHT : WandAction.HandType.LEFT);
			break;
		case Geometry.RECTANGLE:
			wandAction = this.gameObject.AddComponent<WandInteraction> ();
			gameObject.GetComponent<WandInteraction> ().SetWandInteractionData ((Hand == HandType.RIGHT) ? WandAction.HandType.RIGHT : WandAction.HandType.LEFT);
			break;
		case Geometry.UNDEFINED:
			throw new Exception ("Geometría indefinida");
			break;
		}
	}

	private void ExecutueActionByColor ()
	{
		if (magicColor == Color.red) {
			wandAction = this.gameObject.AddComponent<WandMovement> ();
			gameObject.GetComponent<WandMovement> ().SetWandMovementData ((Hand == HandType.RIGHT) ? WandAction.HandType.RIGHT : WandAction.HandType.LEFT);
		} else if (magicColor == Color.green) {
			// No implementado
		} else if (magicColor == Color.blue) {
			wandAction = this.gameObject.AddComponent<WandInteraction> ();
			gameObject.GetComponent<WandInteraction> ().SetWandInteractionData ((Hand == HandType.RIGHT) ? WandAction.HandType.RIGHT : WandAction.HandType.LEFT);
		} else if (magicColor == new Color (1, 1, 0)) {
			// No implementado
		} else if (magicColor == new Color (1, 0, 1)) {
			// No implementado
		} else if (magicColor == new Color (0, 1, 1)) {
			// No implementado
		} else {
			// Color blanco o cualquier combinaión inesperada.
		}
	}

	#endregion

	#region Recogida de datos y reconocimiento de patrones (VR y NoVR)

	private void VRPatternRecorder ()
	{
		// Diferenciamos entre la mano derecha y la izquierda para hacer el análisis.
		if ((Hand == HandType.RIGHT)
			? OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0.2f
			: OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) > 0.2f) {
			if (!dibujando) {
				if (pattern.Count > 0) {
					pattern.Clear ();
				}
				dibujando = true;
				patternCoroutine = StartCoroutine (getPatternVR ());
			}
		}
	}

	private void VRPatternAnalysis ()
	{
		// Diferenciamos entre la mano derecha y la izquierda para hacer el análisis.
		// IMPORTANTE: Se usará el PrimaryIndexTrigger para crear el patrón para dejar libres los botones One y Two para
		// las acciones de la varita.
		if (((Hand == HandType.RIGHT)
			? OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) < 0.2f
			: OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) < 0.2f)
		    && dibujando) {

			try {
				StopCoroutine (patternCoroutine);
			} catch (Exception e) {
				// Do nothing
			}

			dibujando = false;
			Geometry geometria = detectGeometry ();
			if (pattern.Count > 3) {
				switch (geometria) {
				case Geometry.LINE:
					Debug.Log ("LINE DRAWED WITH " + Hand);
					magicColor = Color.red;
					break;
				case Geometry.CIRCLE:
					Debug.Log ("CIRCLE DRAWED WITH " + Hand);
					magicColor = Color.green;
					break;
				case Geometry.RECTANGLE:
					Debug.Log ("RECTANGLE DRAWED WITH " + Hand);
					magicColor = Color.blue;
					break;
				case Geometry.TRIANGLE:
					Debug.Log ("TRIANGLE DRAWED WITH " + Hand);
					break;
				case Geometry.UNDEFINED:
					Debug.Log ("UNDEFINED GEOMETRY WITH " + Hand);
					magicColor = Color.white;
					break;
				}
				DeleteReferences ();
				// ExecuteAction (geometria);
				ExecutueActionByColor ();
			} else {
				Debug.Log ("No hay suficientes muestras para detectar un patrón");
			}
		}
	}

	private IEnumerator getPatternVR ()
	{
		GameObject head = GameObject.Find ("Head");
		while (dibujando) {
			Vector3 auxVector = head.GetComponent<Camera> ().WorldToScreenPoint (transform.position);
			pattern.Add (new Vector2 (auxVector.x, auxVector.y));
			yield return new WaitForSeconds (1.0f / samplesPerSecond);
		}
	}

	#endregion

	#region Color Intersection

	private Color sumaColores (Color a, Color b)
	{
		float red = Mathf.Min (a.r + b.r, 1.0f);
		float green = Mathf.Min (a.g + b.g, 1.0f);
		float blue = Mathf.Min (a.b + b.b, 1.0f);

		return new Color (red, green, blue);
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.tag == "PatternCreator") {
			magicIntersection = true;
			Color colColor = col.GetComponent<PatternRecognition> ().magicColor;
			if (colColor != magicColor) {
				DeleteReferences ();
				magicColor = sumaColores (colColor, magicColor);
				ExecutueActionByColor ();
			}
		}
	}

	#endregion

	#region Geometry Detection

	private Geometry detectGeometry ()
	{
		List<Vector2> simplifiedPattern = douglasPeucker (pattern, EPSILON);
		List<Vector2> convexHullofPattern = QuickHull (simplifiedPattern);
		float convexHullPerimeter = perimeterOfPolygon (convexHullofPattern);
		float convexHullArea = areaOfPolygon (convexHullofPattern);
		float areaOfMaximumTriangle = area (maximumAreaEnclosedTriangle2 (convexHullofPattern));

		/*
		// Funciones usadas para reconcoer el resto de las figuras geométricas
		Rectangle auxRectangle = minimumAreaEnclosingRectangle (convexHullofPattern);
		float rectanglePerimeter = (auxRectangle.extent.x * 2) + (auxRectangle.extent.y * 2);
		*/

		if (Mathf.Pow (convexHullPerimeter, 2) / (convexHullArea + 0.0001f) > 500.0f) {
			return Geometry.LINE;
		} else {
			if (Mathf.Pow (convexHullPerimeter, 2) / (convexHullArea + 0.0001f) < 500 && Mathf.Pow (convexHullPerimeter, 2) / (convexHullArea + 0.0001f) > 100.0f) {
				return Geometry.CIRCLE;
			} else {
				return Geometry.RECTANGLE;
			}
		}
		return Geometry.UNDEFINED;
	}

	private float perimeterOfPolygon (List<Vector2> points)
	{
		float aux = 0.0f;
		for (int i = 0; i < points.Count - 1; i++) {
			aux += distanciaPuntoPunto (points [i], points [i + 1]);
		}
		aux += distanciaPuntoPunto (points [0], points [points.Count - 1]);
		return aux;
	}

	private float areaOfPolygon (List<Vector2> points)
	{
		float aux = 0.0f;
		Vector2 mainPoint = points [0];
		for (int i = 1; i < points.Count - 1; i++) {
			aux += area (mainPoint, points [i], points [i + 1]);	
		}
		return aux;
	}

	private float areaUnderSegment (Vector2 a, Vector2 b)
	{
		float height = (a.y + b.y) / 2;
		float width = (a.x + b.x) / 2;
		return height * width;
	}

	private Triangle maximumAreaEnclosedTriangle2 (List<Vector2> Pattern)
	{
		Triangle auxTriangle = new Triangle ();
		auxTriangle.a = Pattern [0];
		auxTriangle.b = Pattern [0];
		auxTriangle.c = Pattern [0];
		if (Pattern.Count < 3) {
			return auxTriangle;
		}
		int a = 0, b = 1, c = 2;

		for (int i = 0; i < Pattern.Count; i++) {
			for (int j = 0; j < Pattern.Count; j++) {
				for (int k = 0; k < Pattern.Count; k++) {
					if (i != j && i != k && j != k) {
						if (area (Pattern [a], Pattern [b], Pattern [c]) <= area (Pattern [i], Pattern [j], Pattern [k])) {
							a = i;
							b = j;
							c = k;
						}
					}
				}
			}
		}

		try {
			auxTriangle.a = Pattern [a];
			auxTriangle.b = Pattern [b];
			auxTriangle.c = Pattern [c];
		} catch (Exception e) {
			Debug.Log (e.Message);
		}

		return auxTriangle;
	}

	private Triangle maximumAreaEnclosedTriangle (List<Vector2> Pattern)
	{
		Triangle auxTriangle;
		int infiniteLoopContrller = 10000;
		auxTriangle.a = Pattern [0];
		auxTriangle.b = Pattern [1];
		auxTriangle.c = Pattern [2];
		int a = 0, b = 1, c = 2;

		while (true) {
			while (true) {
				while (area (Pattern [a], Pattern [b], Pattern [c]) <= area (Pattern [a], Pattern [b], Pattern [(c + 1) % Pattern.Count])) {
					c = (c + 1) % Pattern.Count;
				}
				if (area (Pattern [a], Pattern [b], Pattern [c]) <= area (Pattern [a], Pattern [(b + 1 % Pattern.Count)], Pattern [c])) {
					b = (b + 1) % Pattern.Count;
					continue;
				} else {
					break;
				}
				// Testeamos para que no haya un bucle infinito
				--infiniteLoopContrller;
				if (infiniteLoopContrller == 0) {
					break;
				}
			}

			if (area (Pattern [a], Pattern [b], Pattern [c]) > area (auxTriangle)) {
				auxTriangle.a = Pattern [a];
				auxTriangle.b = Pattern [b];
				auxTriangle.c = Pattern [c];
			}

			a = (a + 1) % Pattern.Count;
			if (a == b) {
				b = (b + 1) % Pattern.Count;
			}
			if (b == c) {
				c = (c + 1) % Pattern.Count;
			}
			if (a == 0) {
				break;
			}
		}	

		return auxTriangle;
	}

	private float area (Triangle T)
	{
		return area (T.a, T.b, T.c);
	}

	private float area (Vector2 a, Vector2 b, Vector2 c)
	{
		Matrix4x4 mat = new Matrix4x4 ();
		mat.m00 = a.x;
		mat.m01 = a.y;
		mat.m02 = 1;
		mat.m03 = 0;

		mat.m10 = b.x;
		mat.m11 = b.y;
		mat.m12 = 1;
		mat.m13 = 0;

		mat.m20 = c.x;
		mat.m21 = c.y;
		mat.m22 = 1;
		mat.m23 = 0;

		mat.m30 = 0;
		mat.m31 = 0;
		mat.m32 = 0;
		mat.m33 = 1;

		return Mathf.Abs (0.5f * mat.determinant);
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

	private List<Vector2> QuickHull (List<Vector2> Pattern)
	{

		List<Vector2> patternClone = Pattern;
		if (Pattern.Count < 3) {
			return patternClone;
		}
		int minimum = -1;
		int maximum = -1;
		float minX = float.MaxValue;
		float maxX = float.MinValue;

		for (int i = 0; i < Pattern.Count; i++) {
			if (Pattern [i].x < minX) {
				minX = Pattern [i].x;
				minimum = i;
			}
			if (Pattern [i].x > maxX) {
				maxX = Pattern [i].x;
				maximum = i;
			}
		}

		Vector2 PointA = Pattern [minimum];
		Vector2 PointB = Pattern [maximum];
		List<Vector2> ConvexHull = new List<Vector2> ();

		ConvexHull.Add (PointA);
		ConvexHull.Add (PointB);

		patternClone.Remove (PointA);
		patternClone.Remove (PointB);

		List<Vector2> left = new List<Vector2> ();
		List<Vector2> right = new List<Vector2> ();

		for (int i = 0; i < patternClone.Count; i++) {
			Vector2 auxPoint = patternClone [i];
			Orientation pointOrientation = getOrientation (PointA, PointB, auxPoint);
			if (pointOrientation == Orientation.COUNTER_CLOCKWISE) {
				left.Add (auxPoint);
			} else if (pointOrientation == Orientation.CLOCKWISE) {
				right.Add (auxPoint);
			}	
		}

		HullSet (PointA, PointB, right, ConvexHull);
		HullSet (PointB, PointA, left, ConvexHull);

		return ConvexHull;
	}

	private void HullSet (Vector2 PointA, Vector2 PointB, List<Vector2> sideSet, List<Vector2> hull)
	{
		int insertPos = hull.IndexOf (PointB);

		if (sideSet.Count == 0) {
			return;
		}

		if (sideSet.Count == 1) {
			Vector2 auxPoint = sideSet [0];
			sideSet.Remove (auxPoint);
			hull.Add (auxPoint);
			return;
		}

		float distance = float.MinValue;
		int farPoint = -1;

		for (int i = 0; i < sideSet.Count; i++) {
			Vector2 auxPoint = sideSet [i];
			float auxDistance = distanciaTresPuntos (PointA, PointB, auxPoint);

			if (auxDistance > distance) {
				distance = auxDistance;
				farPoint = i;
			}
		}

		Vector2 auxPoint2 = sideSet [farPoint];
		sideSet.Remove (auxPoint2);
		hull.Add (auxPoint2);

		List<Vector2> innerLeft = new List<Vector2> ();
		for (int i = 0; i < sideSet.Count; i++) {
			Vector2 innerAuxPoint = sideSet [i];
			if (getOrientation (PointA, auxPoint2, innerAuxPoint) == Orientation.CLOCKWISE) {
				innerLeft.Add (innerAuxPoint);
			}
		}

		List<Vector2> innerRight = new List<Vector2> ();
		for (int i = 0; i < sideSet.Count; i++) {
			Vector2 innerAuxPoint = sideSet [i];
			if (getOrientation (auxPoint2, PointB, innerAuxPoint) == Orientation.CLOCKWISE) {
				innerLeft.Add (innerAuxPoint);
			}
		}

		HullSet (PointA, auxPoint2, innerLeft, hull);
		HullSet (auxPoint2, PointB, innerRight, hull);
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
		float aux = (B.x - A.x) * (C.y - A.y) - (B.y - A.y) * (C.x - A.x);

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

	private float distanciaTresPuntos (Vector2 A, Vector2 B, Vector2 C)
	{
		float abx = B.x - A.x;
		float aby = B.y - A.y;
		float aux = abx * (A.y - C.y) - aby * (A.x - C.x);
		return Mathf.Abs (aux);
	}

	#endregion
}