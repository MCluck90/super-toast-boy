using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour
{
	public void Update() {
		if (Input.GetButtonDown("Jump")) {
			int nextLevel = Application.loadedLevel + 1;
			if (nextLevel >= Application.levelCount) {
				Application.Quit();
			} else {
				Application.LoadLevel(Application.loadedLevel + 1);
			}
		}
	}

    public void ChangeToScene(int i)
    {
        Application.LoadLevel("1-" + i);
    }
}
