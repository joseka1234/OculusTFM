using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerOfScene : MonoBehaviour
{
	public GameObject ball;
	public Transform ballPosition;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (GameObject.Find ("Ball") == null) {
			GameObject ballObj = Instantiate (ball, ballPosition.position, Quaternion.identity) as GameObject;
			ballObj.name = "Ball";
		}
	}
}
