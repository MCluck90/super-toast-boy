using UnityEngine;
using System.Collections;

public class milkSpit : MonoBehaviour
{
    private GameObject spit1;
    private Vector2 initialPos;
    private Quaternion initialRotation1;
    private GameObject spit2;
    private Vector2 initialPos2;
    private Quaternion initialRotation2;
    private GameObject spit3;
    private Vector2 initialPos3;
    private Quaternion initialRotation3;
    private GameObject spit4;
    private Vector2 initialPos4;
    private Quaternion initialRotation4;
    private GameObject spit5;
    private Vector2 initialPos5;
    private Quaternion initialRotation5;
    private GameObject spit6;
    private Vector2 initialPos6;
    private Quaternion initialRotation6;

    // Use this for initialization
    void Start()
    {
        spit1 = GameObject.Find("milkSpit1");
        initialPos = spit1.transform.position;
        initialRotation1 = spit1.transform.rotation;
        spit2 = GameObject.Find("milkSpit2");
        initialRotation2 = spit2.transform.rotation;
        spit3 = GameObject.Find("milkSpit3");
        initialRotation3 = spit3.transform.rotation;
        spit4 = GameObject.Find("milkSpit4");
        initialRotation4 = spit4.transform.rotation;
        spit5 = GameObject.Find("milkSpit5");
        initialRotation5 = spit5.transform.rotation;
        spit6 = GameObject.Find("milkSpit6");
        initialRotation6 = spit6.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        theSpit(spit1, initialRotation1);
        theSpit(spit2, initialRotation2);
        theSpit(spit3, initialRotation3);
        theSpit(spit4, initialRotation4);
        theSpit(spit5, initialRotation5);
        theSpit(spit6, initialRotation6);
    }
    void theSpit(GameObject aSpit, UnityEngine.Quaternion rot)
    {
        if (aSpit.transform.position.x > 13 && aSpit.transform.position.x < 220 && aSpit.transform.position.y > 0 && aSpit.transform.position.y < 95)
        {
            aSpit.transform.Translate((float)-.22, 0, Time.deltaTime);
        }
        else
        {
            aSpit.transform.position = initialPos;
            aSpit.transform.rotation = rot;
        }
    }
}
