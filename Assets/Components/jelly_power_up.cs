using UnityEngine;
using System.Collections;

public class jelly_power_up : MonoBehaviour {
    private float expireTime;

	// Use this for initialization
	void Start () {
        expireTime = Time.time + 5.0f;
        //this.gameObject.GetComponent<PlayerInput>().WallSlideRatio = 0;
        this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time > expireTime)
        {
            //this.gameObject.GetComponent<PlayerInput>().WallSlideRatio = 2;
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 6;
            Destroy(GetComponent<jelly_power_up>());
        }	
	}
}
