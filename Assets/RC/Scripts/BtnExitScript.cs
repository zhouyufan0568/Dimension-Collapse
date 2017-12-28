using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnExitScript : MonoBehaviour {

    void Start () {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
	}

    private void OnClick()
    {
        //Application.Quit();
    }

    // Update is called once per frame
    void Update () {
		
	}

}
