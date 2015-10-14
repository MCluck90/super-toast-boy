using UnityEngine;
using System.Collections;

public class LevelNumberKeeper : MonoBehaviour {

	public int levelNumber = 1;


	function Awake () {
		DontDestroyOnLoad (transform.LevelNumberKeeper);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
