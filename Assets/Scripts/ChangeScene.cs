﻿using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {
	
	public void ChangeToScene (int i) {
		Application.LoadLevel ("Level_" + i);
	}
}