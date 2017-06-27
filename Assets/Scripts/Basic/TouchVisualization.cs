using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchVisualization : MonoBehaviour
{

	public Material notUsed;
	public Material used;

	public GameObject RTouch;
	public GameObject LTouch;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		ColorIndexTrigger ();
		ColorHandTrigger ();
		ColorButtonOne ();
		ColorButtonTwo ();
		ChangeMaterial ();
	}

	void ColorIndexTrigger ()
	{
		// Color index trigger
		if (OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0.01f) {
			RTouch.transform.GetChild (3).tag = "Used";
		} else {
			RTouch.transform.GetChild (3).tag = "NotUsed";
		}

		if (OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) > 0.01f) {
			LTouch.transform.GetChild (3).tag = "Used";
		} else {
			LTouch.transform.GetChild (3).tag = "NotUsed";
		}
	}

	void ColorHandTrigger ()
	{
		// Color hand trigger
		if (OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > 0.01f) {
			RTouch.transform.GetChild (4).tag = "Used";
		} else {
			RTouch.transform.GetChild (4).tag = "NotUsed";
		}

		if (OVRInput.Get (OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) > 0.01f) {
			LTouch.transform.GetChild (4).tag = "Used";
		} else {
			LTouch.transform.GetChild (4).tag = "NotUsed";
		}
	}

	void ColorButtonOne ()
	{
		// Color Button One
		if (OVRInput.GetDown (OVRInput.Button.One, OVRInput.Controller.RTouch)) {
			RTouch.transform.GetChild (0).tag = "Used";
		} 

		if (OVRInput.GetUp (OVRInput.Button.One, OVRInput.Controller.RTouch)) {
			RTouch.transform.GetChild (0).tag = "NotUsed";
		}

		if (OVRInput.GetDown (OVRInput.Button.One, OVRInput.Controller.LTouch)) {
			LTouch.transform.GetChild (0).tag = "Used";
		}
		if (OVRInput.GetUp (OVRInput.Button.One, OVRInput.Controller.LTouch)) {
			LTouch.transform.GetChild (0).tag = "NotUsed";
		}
	}

	void ColorButtonTwo ()
	{
		// Color Button Two
		if (OVRInput.GetDown (OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
			RTouch.transform.GetChild (1).tag = "Used";
		}
		if (OVRInput.GetUp (OVRInput.Button.Two, OVRInput.Controller.RTouch)) {
			RTouch.transform.GetChild (1).tag = "NotUsed";
		}

		if (OVRInput.GetDown (OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
			LTouch.transform.GetChild (1).tag = "Used";
		} 
		if (OVRInput.GetUp (OVRInput.Button.Two, OVRInput.Controller.LTouch)) {
			LTouch.transform.GetChild (1).tag = "NotUsed";
		}
	}

	void ChangeMaterial ()
	{
		GameObject[] usedObjects = GameObject.FindGameObjectsWithTag ("Used");
		GameObject[] notUsedObjects = GameObject.FindGameObjectsWithTag ("NotUsed");

		foreach (GameObject obj in usedObjects) {
			obj.GetComponent<Renderer> ().material = used;
		}
		foreach (GameObject obj in notUsedObjects) {
			obj.GetComponent<Renderer> ().material = notUsed;
		}
	}
}