using UnityEngine;
using System.Collections;

public class jelly_jar_collision : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        //because debug
        Debug.Log("Collision Detected");
        //we useless now (or do we want to you know allow exploiting of the jelly?)
        Destroy(this.gameObject);
        //set properties of other
        jelly_power_up powerUp = other.gameObject.AddComponent<jelly_power_up>();
    }
}
