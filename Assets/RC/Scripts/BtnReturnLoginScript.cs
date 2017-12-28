using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnReturnLoginScript : MonoBehaviour {

    public string LOGIN_SCENE_NAME = "LoginScene";

    void Start () {
        GetComponent<Button>().onClick.AddListener(OnClick);
	}

    private void OnClick()
    {
        SceneManager.LoadScene(LOGIN_SCENE_NAME);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
