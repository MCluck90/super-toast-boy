using UnityEngine;
using System.Collections;

public class jelly_jar_collision : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (this.gameObject.GetComponent<Renderer>().enabled)
        {
            //because debug
            Debug.Log("Collision Detected");
            //we useless now (or do we want to you know allow exploiting of the jelly?)
            //Destroy(this.gameObject);
            this.gameObject.GetComponent<Renderer>().enabled = false;
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            //set properties of other
            other.gameObject.AddComponent<jelly_power_up>();
        }
    }
}
