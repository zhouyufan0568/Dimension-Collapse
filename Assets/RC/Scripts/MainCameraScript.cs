using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainCameraScript : MonoBehaviour {

    public GameObject dialog;

    void Start () {

	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            dialog.SetActive(true);
        }
	}

    public void isQuit(bool quit)
    {
        if (quit)
        {
            Application.Quit();
            //Debug.Log("quit");
        }
    }

}
