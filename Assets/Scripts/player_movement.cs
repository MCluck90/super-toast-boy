using UnityEngine;
using System.Collections;

public class player_movement : MonoBehaviour {
	
	public float moveSpeed;
	private Vector3 input;
	public Rigidbody player;

	
	// Use this for initialization
	void Start () {
		player = GetComponent<Rigidbody> ();
		
	}
	
	public float turnSpeed = 50f;
	public float jumpheight = 250f;
	bool grounded = false;
	
	// Update is called once per frame
	void FixedUpdate () 
		{
		
			input = new Vector3 (Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		
			if (Input.GetKey ("w"))
				player.AddRelativeForce (input * moveSpeed);
			if (Input.GetKey ("d"))
				player.AddRelativeForce (input * moveSpeed);
			if (Input.GetKey ("a"))
				player.AddRelativeForce (input * moveSpeed);
			if (Input.GetKey ("s"))
				player.AddRelativeForce (input * moveSpeed);
			if (Input.GetButton ("Jump"))
				Jump();
			//player.AddForce(Vector3.up * jumpheight);

		}

		void OnCollisionEnter(Collision collision) 
			{
				grounded = true;
			}

	    void Jump() 
		{
			if (grounded == true) 
				{
					GetComponent<Rigidbody>().AddForce (Vector3.up * jumpheight);
					grounded = false;
				}
		}
	
	
}









