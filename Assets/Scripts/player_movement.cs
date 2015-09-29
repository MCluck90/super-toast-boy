using UnityEngine;
using System.Collections;

public class player_movement : MonoBehaviour {
	
	public float moveSpeed;
	private Vector3 input;
	public Rigidbody player;
	private Vector3 spawn;

	
	// Use this for initialization
	void Start () {
		spawn = transform.position;
		player = GetComponent<Rigidbody> ();
		
	}
	
	public float turnSpeed = 50f;
	
	// Update is called once per frame
	void FixedUpdate () {
		
		input = new Vector3 (Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		
		if (Input.GetKey ("w"))
			player.AddRelativeForce (input * moveSpeed);
		if (Input.GetKey ("d"))
			transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
		//player.AddRelativeForce (input * moveSpeed);
		if (Input.GetKey ("a"))
			transform.Rotate (Vector3.up, -turnSpeed * Time.deltaTime);
		//player.AddRelativeForce (input * moveSpeed);
		if (Input.GetKey ("s"))
			player.AddRelativeForce (input * moveSpeed);
		
	}

	
	
	
}









