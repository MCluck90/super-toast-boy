using UnityEngine;
using System.Collections;

public class player_movement : MonoBehaviour {

	public float Toast_speed = 15f;
	public float jumpHeight;
	public bool isJumping = false;
	public Rigidbody2D toast;
	private Vector3 spawn;

	// Use this for initialization
	void Start () {
		toast = GetComponent<Rigidbody2D> ();
		spawn = transform.position;
	}

	// Update is called once per frame
	void FixedUpdate () 
		{
			if (Input.GetKey (KeyCode.D)) 
				{
					toast.transform.Translate(Vector2.right * Toast_speed * Time.deltaTime);
				}
			if (Input.GetKey (KeyCode.A)) 
				{
					toast.transform.Translate(Vector2.left * Toast_speed * Time.deltaTime);
				}
			if (Input.GetKey (KeyCode.Space) ) 
				{
					if(!isJumping)
					{
						//toast.transform.Translate(Vector2.up * jumpHeight);
						toast.AddForce (new Vector2(0,jumpHeight));
						isJumping = true;
					}
				}
		}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Ground") 
			{
				isJumping = false;
			}
		if (col.gameObject.tag == "Left_wall" || col.gameObject.tag == "Right_wall") 
			{
				isJumping = false;
			}
		if (col.gameObject.tag == "Enemy") 
			{
				transform.position = spawn;
			}
	}

	/*void  OnCollisionStay2D(Collision2D col)
	{

		if (col.gameObject.tag == "Left_wall" || col.gameObject.tag == "Right_wall") 
		{
			isJumping = false;
		}

	}
	*/

}


















