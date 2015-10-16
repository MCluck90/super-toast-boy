using UnityEngine;
using System.Collections;

public class reset_script : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void levelReset()
    {
        this.gameObject.GetComponent<Renderer>().enabled = true;
        this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }
}
