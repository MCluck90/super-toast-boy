using UnityEngine;
using System.Collections;

public class jelly_power_up : MonoBehaviour {
    private float expireTime;

	// Use this for initialization
	void Start () {
        expireTime = Time.time + 15.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time > expireTime)
        {
            Destroy(GetComponent<jelly_power_up>());
        }	
	}

    void OnCollisionEnter(Collision c)
    {
        FixedJoint newJoint = gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = c.rigidbody;
        newJoint.breakForce = 50;
    }
}
